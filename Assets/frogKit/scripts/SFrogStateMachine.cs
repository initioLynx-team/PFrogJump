using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(STongueComponent), typeof(Rigidbody2D))]
public class SFrogStateMachine : MonoBehaviour
{
    public enum FrogState { Idle, Throw,Throwing, PickUp,OnAir, Charging, Jump, Jumping }

    [Header("Current State")]
    [SerializeField] private FrogState currentState = FrogState.Idle;

    [Header("Physics Settings")]
    [SerializeField] private float pickUpForce = 15f;
    [SerializeField] private float minJumpForce = 15f;
    [SerializeField] private float maxJumpForce = 15f;
    [SerializeField] private float maxChargeTime = 15f;
    [SerializeField] private float defaultGravity = 1f;

    [Header("Ground Detection")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private Transform groundCheckPoint; 

    [Header("Input References")]
    public InputActionReference tongueAction;
    public InputActionReference directionAction;
    public InputActionReference jumpAction;

    public Animator animator;
    public SpriteRenderer spriteRenderer;

    private bool isTonguePressed;
    private bool isJumpHeld;
    private float chargeTimer;
    private float CurrentChargePct => Mathf.Clamp01(chargeTimer / maxChargeTime);
    private Vector2 lookDirection;

    private STongueComponent tongueComponent;
    private Rigidbody2D rb;
    private Vector2 stickyTarget;

    private float progressTrowing = 0;
    public  float throwingSegments = 10;
    private void OnEnable() => ToggleActions(true);
    private void OnDisable() => ToggleActions(false);

    bool doobleJump = false;

    public int maxThrows = 1;
    private int throwCont = 1;
    
    void Start()
    {
        tongueComponent = this.GetComponent<STongueComponent>();
        rb = this.GetComponent<Rigidbody2D>();
        throwCont = maxThrows;
    }
    void Update()
    {
        Sense();
        Think();
        React();
    }

    void Sense()
    {
        isTonguePressed = tongueAction.action.IsPressed();
        isJumpHeld = jumpAction.action.IsPressed();

        Vector2 rawInput = directionAction.action.ReadValue<Vector2>();

        // Check if the input is a screen position (Mouse) or a direction (Stick)
        // Mouse positions are usually large numbers (e.g., 1920x1080)
        if (rawInput.magnitude > 2f)
        {
            // Convert Mouse Position to World Direction
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(new Vector3(rawInput.x, rawInput.y, -Camera.main.transform.position.z));
            lookDirection = ((Vector2)mouseWorldPos - (Vector2)transform.position).normalized;
        }
        else
        {
            // Use Stick direction directly
            lookDirection = rawInput.normalized;
        }
    }
    void Think()
    {
        if (currentState == FrogState.PickUp || currentState == FrogState.Throwing) return;
        
        if (currentState == FrogState.Jumping)
        {   
            if (isTonguePressed && throwCont > 0)
            {
                currentState = FrogState.Throw;
                Debug.Log("TongueThrow");
            }

            if (isJumpHeld && doobleJump)
            {
                Debug.Log("DobleJump");
                currentState = FrogState.Jump;
                return;
            }
            bool isFalling = rb.linearVelocity.y <= 0.1f;
            if (isFalling && IsGrounded())
            {
                ResetToIdle();
            }
        }
        ;



        if (currentState == FrogState.Idle)
        {
            if (isTonguePressed)
            {
                currentState = FrogState.Throw;
                Debug.Log("TongueThrow");
            }
            else if (isJumpHeld)
            {
                currentState = FrogState.Charging;
                chargeTimer = 0f;
                Debug.Log("Charge Jump");
            }
        }
        else if (currentState == FrogState.Charging)
        {
            if (isJumpHeld)
            {
                chargeTimer += Time.deltaTime;
                chargeTimer = Mathf.Min(chargeTimer, maxChargeTime);
                if (chargeTimer == maxChargeTime)
                {
                    Debug.Log("Its alredy Max");
                } 
            }
            else
            {
                currentState = FrogState.Jump;
            }
        }
    }
    void React()
    {
        switch (currentState)
        {
            case FrogState.Idle:
                // add efect to know where is the frog looking at
                HandleFacingDirection();
                break;
            case FrogState.Jumping:
                HandleFacingDirection();
                break;
            case FrogState.Throw:
                HandleFacingDirection();
                ExecuteThrow();
                throwCont -=1;
                break;
            case FrogState.Throwing:
                ExecuteThrowing();
                break;
            case FrogState.PickUp:
                pickUpPoint();
                break;
            case FrogState.Jump:
                HandleFacingDirection();
                animator.SetBool("charge",false);
                ExecuteJump();
                doobleJump = false;
                break;
            case FrogState.Charging:
                HandleFacingDirection();
                animator.SetBool("charge",true);
                // add effect to know how much force is being charged at
                break;
        }
    }

    void HandleFacingDirection()
    {
        if (lookDirection.x > 0.1f)
        {
            spriteRenderer.flipX = true;
        } else if (lookDirection.x > - 0.1f) {
            spriteRenderer.flipX = false;
        }
    }
    private bool IsGrounded(){
        return Physics2D.OverlapCircle(groundCheckPoint.position, groundCheckRadius, groundLayer);
    }

    void ExecuteThrow()
    {
        Vector2? hitPoint = tongueComponent.FlickTongue(lookDirection);
        if (hitPoint != null)
        {
            stickyTarget = hitPoint.Value;
            rb.gravityScale = 0; // Turn off gravity while zipping
            rb.linearVelocity = Vector2.zero; // Stop existing jump velocity
            animator.SetBool("throw",true);
            currentState = FrogState.Throwing;
            progressTrowing = 0;
        }
        else
        {
            Debug.Log("No Hit");
            currentState = FrogState.Idle;
        }
    }
    void ExecuteJump()
    {
        float finalForce = Mathf.Lerp(minJumpForce, maxJumpForce, CurrentChargePct);
        rb.linearVelocity = Vector2.zero;
        rb.gravityScale = defaultGravity;
        rb.AddForce(lookDirection * finalForce, ForceMode2D.Impulse);
        Debug.Log("Frog Jumped!");
        chargeTimer = 0f;
        currentState = FrogState.Jumping;
    }

    void ExecuteThrowing()
    {

        float interpolation = (float)progressTrowing / throwingSegments;
        Vector2 currentPos = (Vector2)transform.position;
        Vector2 targetPos = Vector2.Lerp(currentPos, stickyTarget, interpolation);
        tongueComponent.SetTongueData(targetPos);
        progressTrowing+=1;
        if (progressTrowing >= throwingSegments)
        {
            currentState = FrogState.PickUp;
        }
    }

    void pickUpPoint()
    {
        transform.position = Vector2.MoveTowards(transform.position, stickyTarget, pickUpForce * Time.deltaTime);
        if (Vector2.Distance(transform.position, stickyTarget) < 0.05f)
        {
            tongueComponent.Visible(false);
            Debug.Log("Picking up");
            animator.SetBool("throw",false);
            currentState = FrogState.Jumping;
            doobleJump = true;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (currentState == FrogState.PickUp )
        {
            tongueComponent.Visible(false);
            rb.gravityScale = defaultGravity;
            currentState = FrogState.Jumping;
            doobleJump = true;
            animator.SetBool("throw",false);
            Debug.Log("Impact: Reset gravity");
        }
        if (currentState == FrogState.Jumping)
        {
            tongueComponent.Visible(false);
            rb.gravityScale = defaultGravity;
            currentState = FrogState.Jumping;
            animator.SetBool("throw",false);
            Debug.Log("Impact: Reset gravity");
        }
    }

    private void ResetToIdle()
    {
        rb.gravityScale = defaultGravity;
        currentState = FrogState.Idle;
        animator.SetTrigger("jumpEnd");
        throwCont = maxThrows;
    }


    private void ToggleActions(bool enabled)
    {
        if (enabled)
        {
            tongueAction.action.Enable();
            directionAction.action.Enable();
            jumpAction.action.Enable();
        }
        else
        {
            tongueAction.action.Disable();
            directionAction.action.Disable();
            jumpAction.action.Disable();
        }
    }

    private void OnDrawGizmos()
    {
        if (currentState == FrogState.Idle || currentState == FrogState.Charging)
        {

            Gizmos.color = Color.yellow;
            Vector3 startPos = transform.position;
            float indicatorLength = 0.5f;
            if (currentState == FrogState.Charging)
            {
                float chargePct = CurrentChargePct;
                bool isMax = chargePct >= 1.0f;
                Color chargingColor = Color.Lerp(Color.green, Color.red, chargePct);
                indicatorLength += chargePct;
                if (isMax)
                {
                    // Flash between Red and White using Sine wave for "Danger" feel
                    chargingColor = Mathf.Sin(Time.time * 20) > 0 ? Color.white : Color.red;
                }
                Gizmos.color = chargingColor;
            }

            Vector3 endPos = startPos + (Vector3)lookDirection * indicatorLength;
            // Draw the main aim line
            Gizmos.DrawLine(startPos, endPos);
            // Draw a small solid sphere at the tip to represent the "aim" point
            Gizmos.DrawSphere(endPos, 0.2f);
        }
    }


}

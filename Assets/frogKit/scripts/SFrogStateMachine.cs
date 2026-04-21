using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(STongueComponent), typeof(Rigidbody2D))]
public class SFrogStateMachine : MonoBehaviour
{
    public enum FrogState { Idle, Throw, PickUp, Jump, Jumping }

    [Header("Current State")]
    [SerializeField] private FrogState currentState = FrogState.Idle;

    [Header("Physics Settings")]
    [SerializeField] private float pickUpForce = 15f;
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private float defaultGravity = 3f;

    [Header("Input References")]
    public InputActionReference tongueAction;
    public InputActionReference directionAction;
    public InputActionReference jumpAction;


    private bool isTonguePressed;
    private bool isJumpPressed;
    private Vector2 lookDirection;

    private STongueComponent tongueComponent;
    private Rigidbody2D rb;
    private Vector2 stickyTarget;


    private void OnEnable() => ToggleActions(true);
    private void OnDisable() => ToggleActions(false);


    void Start()
    {
        tongueComponent = this.GetComponent<STongueComponent>();
        rb = this.GetComponent<Rigidbody2D>();
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
        isJumpPressed = jumpAction.action.WasPressedThisFrame();
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
        if (currentState == FrogState.PickUp) return;


        if (currentState == FrogState.Idle)
        {
            if (isTonguePressed)
            {
                currentState = FrogState.Throw;
                Debug.Log("TongueThrow");
            }
            else if (isJumpPressed)
            {
                currentState = FrogState.Jump;
                Debug.Log("TongueThrow");
            }
        }
    }
    void React()
    {
        switch (currentState)
        {
            case FrogState.Idle:
            case FrogState.Jumping:
                break;
            case FrogState.Throw:
                ExecuteThrow();
                break;
            case FrogState.PickUp:
                pickUpPoint();
                break;
            case FrogState.Jump:
                ExecuteJump();
                break;
        }
    }

    void ExecuteThrow()
    {
        Vector2? hitPoint = tongueComponent.FlickTongue(lookDirection);
        //TODO: add throwing state with animation
        if (hitPoint != null)
        {
            stickyTarget = hitPoint.Value;
            rb.gravityScale = 0; // Turn off gravity while zipping
            rb.linearVelocity = Vector2.zero; // Stop existing jump velocity
            currentState = FrogState.PickUp;
        }
        else
        {
            Debug.Log("No Hit");
            currentState = FrogState.Idle;
        }
    }
    void ExecuteJump()
    {
        rb.AddForce(lookDirection * jumpForce, ForceMode2D.Impulse);
        Debug.Log("Frog Jumped!");
        currentState  = FrogState.Jumping;
    }

    void pickUpPoint()
    {
        transform.position = Vector2.MoveTowards(transform.position, stickyTarget, pickUpForce * Time.deltaTime);
        // If we reached the point exactly, stop zipping
        if (Vector2.Distance(transform.position, stickyTarget) < 0.05f)
        {
            currentState = FrogState.Idle;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (currentState == FrogState.PickUp || currentState == FrogState.Jumping)
        {
            ResetToIdle();
            Debug.Log("Impact: State Reset to Idle");
        }
    }

    private void ResetToIdle()
    {
        rb.gravityScale = defaultGravity; 
        currentState = FrogState.Idle;
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
}

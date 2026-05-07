using UnityEngine;
using UnityEngine.InputSystem;


[RequireComponent(typeof(STongueComponent), typeof(Rigidbody2D))]
public class SFrogController : MonoBehaviour
{

    public static readonly IFrogState Idle = new IdleState();
    public static readonly IFrogState Moving = new MovingState();
    public static readonly IFrogState Throw = new ThrowState();
    public static readonly IFrogState Throwign = new ThrowingState();
    public static readonly IFrogState PickUp = new PickUpState();
    public static readonly IFrogState Jump = new JumpState();
    public static readonly IFrogState OnAir = new OnAirState();
    public static readonly IFrogState Charging = new ChargingState();

    [Header("Current State (Debug)")]
    public string currentStateName;
    private IFrogState _currentState;

    [Header("Physics Settings")]

    public float pickUpForce = 15f;
    public float minJumpForce = 15f;
    public float maxJumpForce = 15f;
    public float maxChargeTime = 15f;
    public float defaultGravity = 1f;
    public float moveSpeed = 5f;
    public float speedThreshold = 0.2f;

    [Header("Materials")]
    public PhysicsMaterial2D ogPMat;
    public PhysicsMaterial2D slipperyPMat;


    [Header("Ground Detection")]
    public LayerMask groundLayer;
    public float groundCheckRadius = 0.2f;
    public Transform groundCheckPoint;


    [Header("Animation/Visuals")]
    public Animator animator;
    public SpriteRenderer spriteRenderer;


    public int maxThrows = 1;
    public float throwingSegments = 10;


    [Header("StateVar")]

    public Vector2 lookDirection;
    public Vector2 movement;
    public bool isTonguePressed;
    public bool isOnSlipperyFloor;
    public bool isJumpHeld;
    public bool doubleJump;
    public int throwCount;
    public float progressTrowing = 0;
    public Vector2 stickyTarget;
    public float CurrentChargePct = 0;


    public Rigidbody2D Rb { get; private set; }
    public STongueComponent Tongue { get; private set; }

    [Header("Components")]
    public InputActionReference tongueAction, directionAction, moveAction, jumpAction;


    [Header("Audio")]
    public AudioSource sfxSource;
    public AudioClip jumpSound;
    public AudioClip landSound;
    public AudioClip throwSound;

    private void OnEnable() => ToggleActions(true);
    private void OnDisable() => ToggleActions(false);
    private void Awake()
    {
        Rb = GetComponent<Rigidbody2D>();
        Tongue = GetComponent<STongueComponent>();
        throwCount = 1; // Default
    }
    private void Start() => TransitionTo(Idle);
    private void Update()
    {
        ReadInputs();
        IFrogState nextState = _currentState.Tick(this);
        if (nextState != null && nextState != _currentState)
        {
            TransitionTo(nextState);
        }
    }

    private void FixedUpdate() => _currentState.FixedTick(this);

    private void ReadInputs()
    {
        isTonguePressed = tongueAction.action.IsPressed();
        isJumpHeld = jumpAction.action.IsPressed();
        movement = moveAction.action.ReadValue<Vector2>();

        Vector2 rawLook = directionAction.action.ReadValue<Vector2>();
        if (rawLook.magnitude > 2f) // Mouse Logic
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(new Vector3(rawLook.x, rawLook.y, -Camera.main.transform.position.z));
            lookDirection = ((Vector2)mousePos - (Vector2)transform.position).normalized;
        }
        else
        {
            lookDirection = rawLook.normalized;
        }
    }
    public void TransitionTo(IFrogState newState)
    {
        _currentState?.Exit(this);
        _currentState = newState;
        _currentState.Enter(this);
        currentStateName = newState.GetType().Name;
        Debug.Log($"State: {newState.GetType().Name}");
    }

    public bool IsGrounded() => Physics2D.OverlapCircle(groundCheckPoint.position, groundCheckRadius, groundLayer);
    public void HandleFacingDirection()
    {
        if (lookDirection.x > 0.1f)
        {
            spriteRenderer.flipX = true;
        }
        else if (lookDirection.x > -0.1f)
        {
            spriteRenderer.flipX = false;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (_currentState == PickUp)
        {
            Debug.Log("Impact: Reset gravity");
            TransitionTo(OnAir);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Slippery"))
        {
            isOnSlipperyFloor = true;
            Rb.sharedMaterial = slipperyPMat;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Slippery"))
        {
            Rb.sharedMaterial = ogPMat;
            isOnSlipperyFloor = false;
        }
    }


    public void PlaySFX(AudioClip clip, bool randomizePitch = true)
    {
        if (clip == null || sfxSource == null) return;

        if (randomizePitch)
            sfxSource.pitch = Random.Range(0.9f, 1.1f);
        else
            sfxSource.pitch = 1f;

        sfxSource.PlayOneShot(clip);
    }
    private void OnDrawGizmos()
    {
        if (_currentState == Charging)
        {
            float chargePct = CurrentChargePct;
            float indicatorLength = Mathf.Lerp(0, 3, CurrentChargePct);
            Color chargingColor = Color.Lerp(Color.green, Color.red, CurrentChargePct);
            if (chargePct >= 1.0f)
            {
                chargingColor = Mathf.Sin(Time.time * 20) > 0 ? Color.white : Color.red;
            }

            Gizmos.color = chargingColor;
            Vector3 endPos = transform.position + (Vector3)lookDirection * indicatorLength;
            Gizmos.DrawLine(transform.position, endPos);
            Gizmos.DrawSphere(endPos, 0.2f);
        }
    }


    private void ToggleActions(bool enabled)
    {
        if (enabled)
        {
            tongueAction.action.Enable();
            directionAction.action.Enable();
            jumpAction.action.Enable();
            moveAction.action.Enable();
        }
        else
        {
            tongueAction.action.Disable();
            directionAction.action.Disable();
            jumpAction.action.Disable();
            moveAction.action.Disable();
        }
    }

}

using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class SFallingPlatform : MonoBehaviour
{

    [Header("Settings")]
    public float timeBeforeFall = 2f;
    public float verticalFallDistance = 5f;
    public float fallSpeed = 5f;
    public float recoverySpeed = 3f;
    public float waitBeforeRecovery = 3f;


    [Header("State")]
    private bool isPlayerOn = false;
    private bool isFalling = false;
    private float fallTimer;
    private float recoveryTimer;

    private Vector3 ogPosition;
    private Vector3 destination;
    private Collider2D coll;

    private SpriteRenderer sprite;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ogPosition = transform.position;
        destination = ogPosition - new Vector3(0f, verticalFallDistance, 0f);
        coll = GetComponent<Collider2D>();
    }


    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (collision.contacts[0].normal.y < -0.5f)
            {
                isPlayerOn = true;
            }
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            isPlayerOn = false;
        }
    }
    void MoveToDestination()
    {
        transform.position = Vector3.MoveTowards(transform.position, destination, fallSpeed * Time.deltaTime);

        if (transform.position == destination)
        {
            recoveryTimer -= Time.deltaTime;
            if (recoveryTimer <= 0)
            {
                SetPlatformActive(false);
                isFalling = false;
            }
        }
    }
    void MoveBackHome()
    {
        transform.position = Vector3.MoveTowards(transform.position, ogPosition, recoverySpeed * Time.deltaTime);

        if (transform.position == ogPosition)
        {
            ResetPlatform();
        }
    }

    void SetPlatformActive(bool state)
    {
        coll.enabled = state;
        if (sprite) sprite.enabled = state;
    }

    void ResetPlatform()
    {
        SetPlatformActive(true);
        fallTimer = timeBeforeFall;
        recoveryTimer = waitBeforeRecovery;
    }

    void Update()
    {
        if (isPlayerOn && !isFalling)
        {
            fallTimer -= Time.deltaTime;
            if (fallTimer <= 0)
            {
                isFalling = true;
            }
        }

        if (isFalling)
        {
            MoveToDestination();
        }
        else if (!isPlayerOn && transform.position != ogPosition)
        {
            MoveBackHome();
        }
    }
}

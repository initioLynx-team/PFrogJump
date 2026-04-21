using UnityEngine;

public class SMovingPlatform : MonoBehaviour
{

    [Header("MovementSettings")]
    public Vector2 moveDistance = new Vector2(5f,0f);
    public float speed = 2f;

    private Vector2 startPosition;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        // Mathf.PingPong(time, length) returns a value that moves 0 -> length -> 0
        float moveFactor = Mathf.PingPong(Time.time * speed, 1f);

        // Interpolate between start and start + distance
        transform.position = Vector3.Lerp(startPosition, startPosition + moveDistance, moveFactor);
    }


    // This runs only in the Unity Editor
    private void OnDrawGizmos()
    {
        // 1. Determine the start point
        // If the game is running, use startPosition. If not, use current transform.
        Vector3 gizmoStart = Application.isPlaying ? (Vector3)startPosition : transform.position;
        Vector3 gizmoEnd = gizmoStart + (Vector3)moveDistance;

        // 2. Draw the path line
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(gizmoStart, gizmoEnd);

        // 3. Draw "handles" at the start and end points
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(gizmoStart, transform.localScale); // Start box
        
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(gizmoEnd, transform.localScale);   // End box
    }
}

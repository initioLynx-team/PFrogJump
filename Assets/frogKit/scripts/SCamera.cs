using UnityEngine;

public class SCamera : MonoBehaviour
{
    public Transform target;
    public float smoothSpeed = 5f;
    public float yOffset = 2f;

    private float fixedX;

    void Start()
    {
        fixedX = transform.position.x;
    }

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 desiredPosition = new Vector3(
            fixedX,
            target.position.y + yOffset,
            transform.position.z
        );

        transform.position = Vector3.Lerp(
            transform.position,
            desiredPosition,
            smoothSpeed * Time.deltaTime
        );
    }
}

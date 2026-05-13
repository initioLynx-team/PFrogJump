using UnityEngine;

public class Parallax : MonoBehaviour
{
    public Transform cameraTransform;
    public float parallaxFactor = 0.2f;

    private Vector3 lastCameraPosition;

    void Start()
    {
        if (cameraTransform == null)
            cameraTransform = Camera.main.transform;

        lastCameraPosition = cameraTransform.position;
    }

    void LateUpdate()
    {
        Vector3 delta = cameraTransform.position - lastCameraPosition;

        transform.position += new Vector3(
            delta.x * parallaxFactor,
            delta.y * parallaxFactor,
            0f
        );

        lastCameraPosition = cameraTransform.position;
    }
}

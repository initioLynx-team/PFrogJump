using UnityEngine;

public class Parallax : MonoBehaviour
{
    public Transform cameraTransform;
    public float parallaxFactor = 0.05f;

    private Vector3 startPosition;
    private float startCameraY;

    void Start()
    {
        if (cameraTransform == null)
            cameraTransform = Camera.main.transform;

        startPosition = transform.position;
        startCameraY = cameraTransform.position.y;
    }

    void LateUpdate()
    {
        float deltaY= cameraTransform.position.y - startCameraY;
        transform.position = new Vector3(startPosition.x,
           startPosition.y+deltaY*parallaxFactor,
           startPosition.z);

    }
}

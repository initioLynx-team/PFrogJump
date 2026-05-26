using UnityEngine;

public class Parallax2 : MonoBehaviour
{
public Transform cameraTransform;
    public float parallaxEffect = 0.5f;

    private float startPosX;
    private float spriteLength;

    void Start()
    {
        startPosX = transform.position.x;

        SpriteRenderer sr = GetComponent<SpriteRenderer>();

        if (sr != null)
        {
            spriteLength = sr.bounds.size.x;
        }
    }

    void Update()
    {
        float distance = cameraTransform.position.x * parallaxEffect;

        transform.position = new Vector3(
            startPosX + distance,
            transform.position.y,
            transform.position.z
        );
    }
}

using UnityEngine;

public class SCamera : MonoBehaviour
{

    [Header("Target Settings")]
    public Transform target;
    public float followSpeed = 5f;
    [Header("offset Settings")]
    public Vector3 offset = new Vector3(0,0,-10);
    void Start(){}
    void Update(){}
    void LateUpdate()
    {
        if (target == null) return;
        Vector3 targetPosition = target.position + offset;
        transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);
    }
}

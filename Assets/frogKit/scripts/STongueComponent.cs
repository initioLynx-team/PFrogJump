using UnityEditor.Search;
using UnityEngine;
using UnityEngine.Rendering;

public class STongueComponent : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float range = 5f;
    [SerializeField] private LayerMask stickyLayer;
    [SerializeField] private float tongueWidth = 0.3f;
    [SerializeField] private float displayDuration = 0.15f;

    [Header("Visuals ")]
    [SerializeField] private LineController tongueVisual;
    [SerializeField] private GameObject tip;

    [Header("Offsets")]
    [SerializeField] private float horizontalOffset = 0.5f; // Distance from center to mouth
    [SerializeField] private float verticalOffset = 0.2f;   // Height of the mouth


    private Vector2 currentTarget;
    private bool isTongueActive = false;


    private Vector3 mouthOffset;
    private void Start()
    {
        tongueVisual.setWidth(tongueWidth);
        Visible(false);
    }


    public Vector2? FlickTongue(Vector2 direction)
    {
        float side = direction.normalized.x >= 0 ? 1 : -1;
        mouthOffset = new Vector3(horizontalOffset * side, verticalOffset, 0);
        Vector3 origin = transform.position + mouthOffset;

        RaycastHit2D hit = Physics2D.Raycast(origin, direction, range, stickyLayer);
      //  Color debugColor = hit.collider != null ? Color.green : Color.red;
     //   Debug.DrawRay(origin, direction * range, debugColor, 0.5f);

        return hit.collider != null ? hit.point : (Vector2?)null;
    }

    public void SetTongueData(Vector2 target)
    {
        currentTarget = target;
        Visible(true);
    }

    public void Visible(bool vis)
    {
        isTongueActive = vis;
        tongueVisual.ToggleVisibility(vis);
        tip.SetActive(vis);
    }

    private void LateUpdate()
    {
        if (!isTongueActive) return;

        float side = currentTarget.x >= 0 ? 1 : -1;
        Vector3 mouthOffset = new Vector3(horizontalOffset * side, verticalOffset, 0);
        Vector3 origin = transform.position + mouthOffset;

        Vector3 endPoint = new Vector3(currentTarget.x, currentTarget.y, transform.position.z);

        tongueVisual.SetTonguePositions(origin, endPoint);

        Vector2 dir = (Vector2)endPoint - (Vector2)origin;

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        // 3. Place the tip exactly at endPoint
        tip.transform.SetPositionAndRotation(
            new Vector3(endPoint.x, endPoint.y, endPoint.z - 1.0f),
            Quaternion.Euler(0, 0, angle)
        );

        tip.SetActive(true);
    }
}
using UnityEngine;

public class STongueComponent : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float range = 5f;
    [SerializeField] private LayerMask stickyLayer;

    
    [Header("Visuals (Optional)")]
    [SerializeField] private LineRenderer lineRenderer;

    private void Start()
    {
        if (lineRenderer != null) lineRenderer.enabled = false;
    }

    public Vector2? FlickTongue(Vector2 direction)
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, range, stickyLayer);
       
        // 2. Quick Debug Ray (Visible in Scene and Game view with Gizmos ON)
        Color debugColor = hit.collider != null ? Color.green : Color.red;
        Debug.DrawRay(transform.position, direction * range, debugColor, 0.5f);

        if (hit.collider != null) return hit.point;

        UpdateVisuals(transform.position + (Vector3)(direction * range));
        return null;
    }

    private void UpdateVisuals(Vector3 endPoint)
    {
        if (lineRenderer == null) return;
        
        lineRenderer.enabled = true;
        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, endPoint);
        
        // Hide tongue after a short delay
        Invoke(nameof(HideTongue), 0.2f);
    }

    private void HideTongue() => lineRenderer.enabled = false;

    // Debugging range in the editor
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}
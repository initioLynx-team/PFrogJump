using UnityEngine;

[RequireComponent(typeof(LineRenderer))]

public class LineController : MonoBehaviour
{
    private LineRenderer lineRenderer;

    [Header("Settings")]
    [SerializeField] private float textureRepeatUnit = 1f;
    
    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.textureMode = LineTextureMode.Tile;
        lineRenderer.alignment = LineAlignment.View;
    }

    public void setWidth(float width)
    {
        lineRenderer.startWidth = width;
        lineRenderer.startWidth = width;
    }
    public void SetTonguePositions(Vector3 start, Vector3 end)
    {
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, start);
        lineRenderer.SetPosition(1, end);
        float distance = Vector3.Distance(start, end);
        lineRenderer.material.mainTextureScale = new Vector2(distance / textureRepeatUnit, 1);
    }
    public void ToggleVisibility(bool visible)
    {
        lineRenderer.enabled = visible;
    }
    void Update()
    {

        
        
    }
}

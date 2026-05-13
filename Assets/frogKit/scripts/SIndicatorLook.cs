using UnityEngine;

public class SIndicatorLook : MonoBehaviour
{

    public GameObject indicator;

    public float distance = 2f;
    private float actualDist = 0f;
    public float appearSpeed = 0.01f;

    [Range(0f, 1f)]
    public float deadzone = 0.15f;

    public void SetVisible(bool visible)
    {
        indicator.SetActive(visible);
    }
    public void UpdateLookDirection(Vector2 direction)
    {
        if (direction.sqrMagnitude < deadzone)
        {
            actualDist = 0f;
            SetVisible(false);
            return;
        }
        else
        {
            SetVisible(true);
        }

        actualDist = Mathf.MoveTowards(actualDist, distance,  appearSpeed);

        Vector2 normDir = direction.normalized;

        indicator.transform.localPosition = (Vector3)normDir * actualDist;

        float angle = Mathf.Atan2(normDir.y, normDir.x) * Mathf.Rad2Deg;
        indicator.transform.localRotation = Quaternion.Euler(0, 0, angle - 90f);
    }

}

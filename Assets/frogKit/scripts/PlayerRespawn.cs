using UnityEngine;

public class PlayerRespawn : MonoBehaviour
{
    public Transform currentRespawnPoint;

    public void SetRespawnPoint(Transform newRespawnPoint)
    {
        currentRespawnPoint = newRespawnPoint;
    }

    public void Respawn()
    {
        transform.position = currentRespawnPoint.position;

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }
    }
}

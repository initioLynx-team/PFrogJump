using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public Transform respawnPoint;

    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerRespawn playerRespawn = other.GetComponent<PlayerRespawn>();

        if (playerRespawn != null)
        {
            if(respawnPoint == null)
            {
                Debug.Log("Checkpoint activado");
            }
            playerRespawn.SetRespawnPoint(respawnPoint);
        }
    }
}

using UnityEngine;

public class CheckpointTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Abigail"))
        {
            Debug.Log("Checkpoint reached.");
            CheckpointManager.Instance.SetRespawnPoint(transform.position);
        }
    }
}

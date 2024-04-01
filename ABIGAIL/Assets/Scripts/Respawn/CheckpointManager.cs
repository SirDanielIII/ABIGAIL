using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    public static CheckpointManager Instance { get; private set; }

    private Vector2 currentRespawnPoint;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject); 
        }
    }

    public void SetRespawnPoint(Vector2 newRespawnPoint)
    {
        currentRespawnPoint = newRespawnPoint;
    }

    public Vector2 GetRespawnPoint()
    {
        return currentRespawnPoint;
    }
}

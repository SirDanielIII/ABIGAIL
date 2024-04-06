using UnityEngine;
using UnityEngine.SceneManagement;

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
        SavePlayerStateAtCheckpoint();
    }

    public Vector2 GetRespawnPoint()
    {
        return currentRespawnPoint;
    }

    public void SavePlayerStateAtCheckpoint()
    {
        var player = FindObjectOfType<Abigail.Movement>();
        var healthComponent = player.GetComponent<Health>();

        bool hasKey = LevelManager.Instance != null && LevelManager.Instance.hasKey;        
        PlayerPrefs.SetInt("PlayerHealth", healthComponent.currentHealth);
        PlayerPrefs.SetFloat("PlayerStamina", player.stamina);
        PlayerPrefs.SetInt("HasKey", hasKey ? 1 : 0);
        PlayerPrefs.SetFloat("CheckpointPositionX", currentRespawnPoint.x);
        PlayerPrefs.SetFloat("CheckpointPositionY", currentRespawnPoint.y);
        PlayerPrefs.SetInt("SavedScene", SceneManager.GetActiveScene().buildIndex);
    
        PlayerPrefs.Save();
    }

    public void LoadPlayerState()
    {
        if (PlayerPrefs.HasKey("PlayerHealth"))
        {
            var player = FindObjectOfType<Abigail.Movement>();
            if (player != null)
            {
                var healthComponent = player.GetComponent<Health>();
                int health = PlayerPrefs.GetInt("PlayerHealth");
                float stamina = PlayerPrefs.GetFloat("PlayerStamina");
                Vector2 checkpointPosition = new Vector2(PlayerPrefs.GetFloat("CheckpointPositionX"), PlayerPrefs.GetFloat("CheckpointPositionY"));
                bool hasKey = PlayerPrefs.GetInt("HasKey", 0) == 1;

                healthComponent.currentHealth = health;
                player.stamina = stamina;
                player.transform.position = checkpointPosition;
                if (LevelManager.Instance != null)
                {
                    LevelManager.Instance.hasKey = hasKey;
                    UIManager.Instance.UpdateKeyIndicator(hasKey);
                }
            }
        
            else
            {
                // Debug.LogWarning("Player object not found, cannot load state.");
            }
        }
    }
}

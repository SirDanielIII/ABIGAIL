using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagement : MonoBehaviour
{
    public static SceneManagement Instance { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            SceneManager.sceneLoaded += OnSceneLoaded;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }
    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void SaveCurrentScene()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        PlayerPrefs.SetInt("SavedScene", currentSceneIndex);
        PlayerPrefs.Save();
    }

    public void LoadNextScene()
    {
        SaveGameState();
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = currentSceneIndex + 1;

        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            ResetPlayerStateForNewLevel();
            SceneManager.LoadScene(nextSceneIndex);
        }
        else
        {
            SceneManager.LoadScene(0);
        }
    }

    public void ResetPlayerStateForNewLevel()
    {
        PlayerPrefs.DeleteKey("PlayerHealth");
        PlayerPrefs.DeleteKey("PlayerStamina");
        PlayerPrefs.DeleteKey("CheckpointPositionX");
        PlayerPrefs.DeleteKey("CheckpointPositionY");
        PlayerPrefs.DeleteKey("HasKey");
        PlayerPrefs.Save();
    }

    public void SaveGameState()
    {
        if (CheckpointManager.Instance != null)
        {
            CheckpointManager.Instance.SavePlayerStateAtCheckpoint();
        }
        else
        {
            // Debug.LogWarning("CheckpointManager is not found. Player state may not be saved.");
        }
        SaveCurrentScene();
    }
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.buildIndex == 4 || scene.buildIndex == 8) 
        {
            CheckpointManager.Instance.LoadPlayerState();
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    public bool hasKey = false;
    public GateController gateController;
    public Transform playerTransform;

    public SceneManagement sceneManager;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }
    void Start()
    {
        LoadKeyState();
    }

    void LoadKeyState()
    {
        if (PlayerPrefs.HasKey("HasKey"))
        {
            hasKey = PlayerPrefs.GetInt("HasKey") == 1;
        }
        else
        {
            hasKey = false;
        }

        if (UIManager.Instance != null)
        {
            UIManager.Instance.UpdateKeyIndicator(hasKey);
        }
    }

    void Update()
    {
        if (gateController != null && gateController.IsGateOpened())
        {
            if (playerTransform.position.x > gateController.GetGatePosition().x + 6f)
            {
                sceneManager.LoadNextScene();
            }
        }
    }

    public void CollectKey()
    {
        hasKey = true;
        SaveKeyState();
        if (UIManager.Instance != null)
        {
            UIManager.Instance.UpdateKeyIndicator(hasKey);
        }    
    }

    void SaveKeyState()
    {
        PlayerPrefs.SetInt("HasKey", hasKey ? 1 : 0);
        PlayerPrefs.Save();
    }
    
    public void ResetKeyState()
    {
        hasKey = false;
        SaveKeyState();

        if (UIManager.Instance != null)
        {
            UIManager.Instance.UpdateKeyIndicator(hasKey);
        }
    }
}

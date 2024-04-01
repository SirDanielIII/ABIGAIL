using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    public bool hasKey = false;
    public GateController gateController;
    public Transform playerTransform;

    public GameManager gameManager;

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
        hasKey = false;
    }

    void Update()
    {
        if (gateController != null && gateController.IsGateOpened())
        {
            if (playerTransform.position.x > gateController.GetGatePosition().x + 7f)
            {
                gameManager.LoadNextScene();
            }
        }
    }

    public void CollectKey()
    {
        hasKey = true;
        if (UIManager.Instance != null)
        {
            UIManager.Instance.UpdateKeyIndicator(hasKey);
        }    
    }
}

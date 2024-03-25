using UnityEngine;
public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;
    public bool hasKey = false;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        hasKey = false;
    }

    public void CollectKey()
    {
        Debug.Log("Key collected!");
        hasKey = true;
        if (UIManager.instance != null)
        {
            UIManager.instance.UpdateKeyIndicator(hasKey);
        }    
    }
}

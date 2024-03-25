using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    public Image keyIndicator;

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
    }

    void Start()
    {
        SetKeyTransparency(0.3f);
    }

    public void UpdateKeyIndicator(bool hasKey)
    {
        if (hasKey)
        {
            SetKeyTransparency(1.0f);
        }
        else
        {
            SetKeyTransparency(0.3f);
        }
    }

    public void SetKeyTransparency(float alpha)
    {
        Color color = keyIndicator.color;
        color.a = alpha;
        keyIndicator.color = color;
    }
}

using UnityEngine;

public class HideCanvasOnSpace : MonoBehaviour
{
    public GameObject canvasToHide;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (canvasToHide != null)
            {
                canvasToHide.SetActive(false);
            }
        }
    }
}

using UnityEngine;
using UnityEngine.UI; // Required for UI

public class HideCanvasOnClick : MonoBehaviour
{
    public Button buttonToHideCanvas; // The button that will hide the canvas
    public Canvas canvasToHide; // Assign the canvas you want to hide in the inspector

    void Start()
    {
        if (buttonToHideCanvas != null)
        {
            // Subscribe the HideCanvas method to the button's onClick event
            buttonToHideCanvas.onClick.AddListener(HideCanvas);
        }
    }

    void HideCanvas()
    {
        if (canvasToHide != null)
        {
            canvasToHide.gameObject.SetActive(false); // Deactivate the canvas
        }
    }

    private void OnDestroy()
    {
        // Always good practice to unsubscribe when the object is destroyed
        if (buttonToHideCanvas != null)
        {
            buttonToHideCanvas.onClick.RemoveListener(HideCanvas);
        }
    }
}

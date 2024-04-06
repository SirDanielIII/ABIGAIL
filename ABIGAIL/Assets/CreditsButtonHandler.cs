using UnityEngine;

public class CreditsButtonHandler : MonoBehaviour
{
    public GameObject creditsPanel; 

    public void Start()
    {
        // Hide the credits panel when the game starts
        creditsPanel.SetActive(false);
    }

    public void ToggleCredits()
    {
        // Toggle the active state of the credits panel
        creditsPanel.SetActive(!creditsPanel.activeSelf);
    }
}

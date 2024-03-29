using UnityEngine;

public class TransitionZone : MonoBehaviour
{
    private bool playerIsInside = false;

    void Update()
    {
        // Check if the player is inside the transition zone and presses "R"
        if (playerIsInside && Input.GetKeyDown(KeyCode.R))
        {
            GameManager.Instance.SwitchPerspective(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Abigail"))
        {
            playerIsInside = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Abigail"))
        {
            playerIsInside = false;
        }
    }
}

using UnityEngine;

public class TransitionZone : MonoBehaviour
{
    private bool playerIsInside = false;
    public AudioSource transitionSound;
    public bool musicPlaying = false;

    void Update()
    {
        // Audio
        if (playerIsInside && !musicPlaying)
        {
            transitionSound.Play();
            musicPlaying = true;
        }
        if (!playerIsInside && musicPlaying)
        {
            transitionSound.Stop();
            musicPlaying = false;
        }
        
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

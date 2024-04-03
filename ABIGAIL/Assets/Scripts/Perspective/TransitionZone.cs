using UnityEngine;

public class TransitionZone : MonoBehaviour
{
    private bool playerIsInside = false;
    public AudioSource transition;
    public bool songPlaying = false; // Check if the music is playing

    void Update()
    {

        // Music when standing in transition zone
        if (playerIsInside && !songPlaying)
        {
            transition.Play();
            songPlaying = true;
        }
        if (!playerIsInside && songPlaying)
        {
            transition.Stop();
            songPlaying = false;
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

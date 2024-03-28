using UnityEngine;

public class RespawnPad : MonoBehaviour
{
    public Vector3 respawnPosition;

    private bool playerIsOnPad = false; 

    void Update()
    {
        Debug.Log("Player is on pad: " + playerIsOnPad);
        if (playerIsOnPad && Input.GetKeyDown(KeyCode.R))
        {
            RespawnPlayer();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Abigail"))
        {
            playerIsOnPad = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Abigail"))
        {
            playerIsOnPad = false;
        }
    }

    private void RespawnPlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Abigail");
        player.transform.position = respawnPosition;
    }
}

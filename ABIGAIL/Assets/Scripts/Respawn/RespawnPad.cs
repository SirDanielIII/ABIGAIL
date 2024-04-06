using UnityEngine;
using System.Collections;

public class RespawnPad : MonoBehaviour
{
    public Vector3 respawnPosition;

    private bool playerIsOnPad = false; 

    void Update()
    {
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
                GameObject[] tumbleweeds = GameObject.FindGameObjectsWithTag("Tumbleweed");
        foreach (GameObject tumbleweed in tumbleweeds)
        {
            Destroy(tumbleweed);
        }
        TumbleweedSpawner spawner = FindObjectOfType<TumbleweedSpawner>();
        if (spawner != null)
        {
            spawner.ResetSpawnPoints();
            spawner.DestroyAllTumbleweeds();
        }
        StartCoroutine(RespawnTraps());
    }

    IEnumerator RespawnTraps()
    {
        yield return new WaitForSeconds(0.5f);
        DynamiteTrapManager.Instance.RespawnAllTraps();
    }
}

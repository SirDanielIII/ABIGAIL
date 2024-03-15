using UnityEngine;

public class VultureSpawner : MonoBehaviour
{
    public GameObject vulturePrefab;
    public GameObject[] spawnPoints; // You can assign specific points or generate dynamically
    public float checkInterval = 0.01f; // Adjust based on how often you want to check for spawning
    private Camera mainCamera;
    private float nextCheckTime;

    private void Start()
    {
        mainCamera = Camera.main;
        nextCheckTime = Time.time;
    }

    private void Update()
    {
        if (Time.time >= nextCheckTime)
        {
            SpawnVulture();
            nextCheckTime = Time.time + checkInterval;
        }
    }

    private void SpawnVulture()
    {
        float spawnAheadDistance = 13.0f; // Distance ahead of the camera to spawn, adjust as needed

        foreach (var spawnPoint in spawnPoints)
        {
            if (spawnPoint.activeSelf)
            {
                // Considering vultures might spawn above the player
                float distanceToCamera = spawnPoint.transform.position.x - mainCamera.transform.position.x;
                float heightDifference = spawnPoint.transform.position.y - mainCamera.transform.position.y;

                // Check if spawn point is within a reasonable distance ahead of the camera and above it
                if (distanceToCamera > 0 && distanceToCamera < spawnAheadDistance && heightDifference > 0)
                {
                    Debug.Log("Spawning vulture at " + spawnPoint.transform.position);
                    Instantiate(vulturePrefab, spawnPoint.transform.position, Quaternion.identity);
                    spawnPoint.SetActive(false); // Prevent re-spawning, you can re-activate these points under certain conditions if needed
                }
            }
        }
    }
}

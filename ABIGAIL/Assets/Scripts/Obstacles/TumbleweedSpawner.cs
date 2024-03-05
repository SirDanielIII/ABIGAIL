using UnityEngine;

public class TumbleweedSpawner : MonoBehaviour
{
    public GameObject tumbleweedPrefab;
    public GameObject[] spawnPoints;
    public float checkInterval = 1f; // How often to check spawn points (in seconds)
    private Camera mainCamera;
    private float nextCheckTime;

    private void Start()
    {
        mainCamera = Camera.main; // Cache the main camera
        nextCheckTime = Time.time; // Initialize the next check time
    }

    private void Update()
    {
        // Periodically check if it's time to spawn tumbleweeds
        if (Time.time >= nextCheckTime)
        {
            SpawnTumbleweed();
            nextCheckTime = Time.time + checkInterval; // Set the time for the next check
        }
    }

    private void SpawnTumbleweed()
    {
        foreach (var spawnPoint in spawnPoints)
        {
            if (spawnPoint.activeSelf)
            {
                Vector3 screenPoint = mainCamera.WorldToViewportPoint(spawnPoint.transform.position);
                bool isOutside = screenPoint.x < 0 || screenPoint.x > 1 || screenPoint.y < 0 || screenPoint.y > 1;
                if (isOutside)
                {
                    Instantiate(tumbleweedPrefab, spawnPoint.transform.position, Quaternion.identity);
                    spawnPoint.SetActive(false);
                }
            }
        }
    }
}

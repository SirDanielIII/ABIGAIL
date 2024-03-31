using UnityEngine;

public class TumbleweedSpawner : MonoBehaviour
{
    public GameObject tumbleweedPrefab;
    public GameObject[] spawnPoints;
    public float checkInterval = 0.01f;
    private Camera mainCamera;
    private float nextCheckTime;

    private void Start()
    {
        mainCamera = Camera.main;
        nextCheckTime = Time.time;
    }

    private void Update()
    {
        // Periodically check if it's time to spawn tumbleweeds
        if (Time.time >= nextCheckTime)
        {
            SpawnTumbleweed();
            nextCheckTime = Time.time + checkInterval;
        }
    }

    private void SpawnTumbleweed()
    {
        float spawnAheadDistance = 14.0f; // Adjust this value based on your level design and camera speed

        foreach (var spawnPoint in spawnPoints)
        {
            if (spawnPoint.activeSelf)
            {
                float distanceToCamera = spawnPoint.transform.position.x - mainCamera.transform.position.x;
                float verticalDistanceToCamera = spawnPoint.transform.position.y - mainCamera.transform.position.y;
                
                // For a right-moving camera
                if (distanceToCamera > 0 && distanceToCamera < spawnAheadDistance && verticalDistanceToCamera > 0.0f)
                {
                    Instantiate(tumbleweedPrefab, spawnPoint.transform.position, Quaternion.identity);
                    spawnPoint.SetActive(false); // Prevent re-spawning
                }
                // For a left-moving camera (if applicable)
                else if (distanceToCamera < 0 && Mathf.Abs(distanceToCamera) < spawnAheadDistance)
                {
                    Instantiate(tumbleweedPrefab, spawnPoint.transform.position, Quaternion.identity);
                    spawnPoint.SetActive(false);
                }
            }
        }
    }

    public void ResetSpawnPoints()
    {
        foreach (var spawnPoint in spawnPoints)
        {
            spawnPoint.SetActive(true); // Reactivate all spawn points
        }
    }

}

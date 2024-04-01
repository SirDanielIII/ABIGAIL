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
        if (Time.time >= nextCheckTime)
        {
            SpawnTumbleweed();
            nextCheckTime = Time.time + checkInterval;
        }
    }

    private void SpawnTumbleweed()
    {
        float spawnAheadDistance = 15.0f;

        foreach (var spawnPoint in spawnPoints)
        {
            if (spawnPoint.activeSelf)
            {
                float distanceToCamera = spawnPoint.transform.position.x - mainCamera.transform.position.x;
                float verticalDistanceToCamera = spawnPoint.transform.position.y - mainCamera.transform.position.y;
                
                if (distanceToCamera > 0 && distanceToCamera < spawnAheadDistance && verticalDistanceToCamera > -1.0f)
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
            spawnPoint.SetActive(true);
        }
    }

}

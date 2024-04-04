using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    public int maxHealth = 10;
    private int currentHealth;
    public HealthBar healthBar; // This can now be left unassigned/null
    public float fallThreshold = -5f; // The y-coordinate that triggers a respawn if the player falls below it
    public AudioSource damageSound;

    private void Start()
    {
        currentHealth = maxHealth;
        // Only set max health if healthBar is not null
        if (healthBar != null)
        {
            healthBar.SetMaxHealth(maxHealth);
        }
    }

    private void Update()
    {
        // Check for falling below the threshold
        if (GameManager.Instance.isSideView() && transform.position.y < fallThreshold)
        {
            Respawn();
        }
    }
    public void TakeDamage(int damageAmount)
    {
        currentHealth -= damageAmount;
        damageSound.Play();
        if (healthBar != null)
        {
            healthBar.SetHealth(currentHealth);
        }
        if (currentHealth <= 0)
        {
            Respawn();
        }
    }

    private void Respawn()
    {
        currentHealth = maxHealth;
        if (healthBar != null)
        {
            healthBar.SetMaxHealth(maxHealth);
        }
        if (!GameManager.Instance.isSideView())
        {
            GameManager.Instance.topDownPlayer.SetActive(false);
            GameManager.Instance.sideViewPlayer.SetActive(true);
            GameManager.Instance.sideViewCamera.enabled = true;
            GameManager.Instance.topDownCamera.enabled = false;
            GameManager.Instance.isSideViewActive = true;
            // StartCoroutine(GameManager.Instance.EnableCamerasAfterDelay(0.1f));
        }
        transform.position = CheckpointManager.Instance.GetRespawnPoint();
        GameObject[] tumbleweeds = GameObject.FindGameObjectsWithTag("Tumbleweed");
        foreach (GameObject tumbleweed in tumbleweeds)
        {
            Destroy(tumbleweed);
        }
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
        }
        TumbleweedSpawner spawner = FindObjectOfType<TumbleweedSpawner>();
        if (spawner != null)
        {
            spawner.ResetSpawnPoints();
        }
        DynamiteTrapManager.Instance.RespawnAllTraps();
    }
}
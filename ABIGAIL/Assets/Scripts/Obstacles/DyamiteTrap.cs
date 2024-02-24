using UnityEngine;

public class DynamiteTrap : MonoBehaviour
{
    public float delayBeforeExplosion = 2f; // Time in seconds before the trap explodes
    public int damageAmount = 10; // Amount of damage dealt by the explosion
    public float explosionRadius = 1f; // Radius within which the explosion affects objects

    private void Start()
    {
        // Optionally, start a countdown immediately or wait for a specific trigger
        // For example, using OnTriggerEnter2D to detect the player's approach
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Abigail")) // Assuming the player triggers the explosion
        {
            Invoke("Explode", delayBeforeExplosion); // Wait and then explode
        }
    }

    void Explode()
    {
        // Use Physics2D.OverlapCircleAll to find all objects within the explosion radius
        Collider2D[] objectsInRadius = Physics2D.OverlapCircleAll(transform.position, explosionRadius);

        foreach (var obj in objectsInRadius)
        {
            if (obj.CompareTag("Abigail"))
            {
                Health health = obj.GetComponent<Health>(); // Try to get the Health component
                if (health != null)
                {
                    health.TakeDamage(damageAmount); // Apply damage
                    Debug.Log("Damage applied to " + obj.name);
                }
            }
        }

        // Here, you could  instantiate an explosion effect or play a sound

        Destroy(gameObject); // Destroy the dynamite trap GameObject after the explosion
    }

    // Visualize the explosion radius in the Unity Editor
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}

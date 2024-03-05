using UnityEngine;

public class Knockback : MonoBehaviour
{
    public float bounceForce = 20f; // Adjustable force for knockback

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Tumbleweed"))
        {
            // Calculate direction from the tumbleweed to the player
            Vector2 knockbackDir = (transform.position - other.transform.position).normalized;
            // Apply knockback force
            rb.AddForce(knockbackDir * bounceForce, ForceMode2D.Impulse);
        }
    }
}

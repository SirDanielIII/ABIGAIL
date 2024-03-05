using UnityEngine;

public class TumbleweedMovement : MonoBehaviour
{
    public float horizontalSpeed = 5f;
    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        // Apply an initial horizontal force to simulate wind pushing the tumbleweed.
        rb.AddForce(Vector2.left * horizontalSpeed, ForceMode2D.Force);
    }

    void FixedUpdate()
    {
        // Ensure the tumbleweed keeps moving left with a consistent velocity
        if (rb.velocity.x > -horizontalSpeed)
        {
            rb.AddForce(Vector2.left * horizontalSpeed, ForceMode2D.Force);
        }
    }
}

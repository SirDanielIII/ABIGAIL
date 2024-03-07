using UnityEngine;
using Abigail;

public class TumbleweedMovement : MonoBehaviour
{
    public float horizontalSpeed = 6f;
    public LayerMask Player;
    public float checkRadius = 0.75f;
    public float bounceForce = 300f; // Adjust based on testing
    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.AddForce(Vector2.left * horizontalSpeed, ForceMode2D.Force);
    }

    void Update()
    {
        Collider2D playerCollider = Physics2D.OverlapCircle(transform.position, checkRadius, Player);
        if (playerCollider != null && !playerCollider.GetComponent<Movement>().isKnockedBack)
        {
            Movement playerMovement = playerCollider.GetComponent<Movement>();
            if (playerMovement != null && !playerMovement.isKnockedBack)
            {
                float direction = Mathf.Sign(playerCollider.transform.position.x - transform.position.x);
                Vector2 knockbackVelocity = new Vector2(direction * bounceForce, 1f); // No vertical component
                playerMovement.ApplyKnockback(knockbackVelocity, 0.05f); // Adjust duration as needed
            }
        }
    }

    void FixedUpdate()
    {
        if (rb.velocity.x > -horizontalSpeed)
        {
            rb.AddForce(Vector2.left * horizontalSpeed, ForceMode2D.Force);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, checkRadius);
    }
}

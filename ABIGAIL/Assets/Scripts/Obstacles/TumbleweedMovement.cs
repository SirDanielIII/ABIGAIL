using UnityEngine;
using Abigail;

public class TumbleweedMovement : MonoBehaviour
{
    public float horizontalSpeed = 7f;
    public LayerMask Player;
    public float checkRadius = 0.75f;
    public float bounceForce = 10000f;
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
                Vector2 knockbackVelocity = new Vector2(-4.00f * bounceForce, 1.20f);

                playerMovement.ApplyKnockback(knockbackVelocity, 0.2f);
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
}

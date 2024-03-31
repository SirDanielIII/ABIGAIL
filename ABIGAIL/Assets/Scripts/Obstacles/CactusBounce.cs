using UnityEngine;
using Abigail;

public class CactusBounce : MonoBehaviour
{
    public float bounceForce; 

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Abigail"))
        {
            Movement playerMovement = collision.GetComponent<Movement>();
            Rigidbody2D playerRigidbody = collision.GetComponent<Rigidbody2D>();


            if (playerMovement != null && playerRigidbody != null && !playerMovement.isKnockedBack)
            {
                Vector2 incomingVelocityDirection = playerRigidbody.velocity.normalized;
                
                Vector2 knockbackVelocity = -incomingVelocityDirection * bounceForce;
                Debug.Log(knockbackVelocity);

                playerMovement.ApplyKnockback(knockbackVelocity, 0.1f);
            }
        }
    }
}

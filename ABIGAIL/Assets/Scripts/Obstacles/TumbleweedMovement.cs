using UnityEngine;
using Abigail;

public class TumbleweedMovement : MonoBehaviour
{
    public float horizontalSpeed = 6f;
    public LayerMask Player;
    public float checkRadius = 0.75f;
    public float bounceForce = 10000f;
    private Rigidbody2D rb;
    public float rotationSpeed = 100f;

    private Vector2 lastPosition;
    private float stationaryTime = 0f;
    private bool isRotating = true;




    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.AddForce(Vector2.left * horizontalSpeed, ForceMode2D.Force);
        lastPosition = rb.position;
    }

    void Update()
    {
        Collider2D playerCollider = Physics2D.OverlapCircle(transform.position, checkRadius, Player);
        if (playerCollider != null)
        {
            if (!playerCollider.GetComponent<Movement>().isKnockedBack)
            {
                Movement playerMovement = playerCollider.GetComponent<Movement>();
                if (playerMovement != null && !playerMovement.isKnockedBack)
                {
                    Vector2 knockbackVelocity = new Vector2(-3.50f * bounceForce, 1.30f);

                    playerMovement.ApplyKnockback(knockbackVelocity, 0.2f);
                }
            }
        }
        if ((rb.position - lastPosition).magnitude < 0.01)
        {
            stationaryTime += Time.deltaTime;
        }
        else
        {
            stationaryTime = 0.0f;
        }

        lastPosition = rb.position;

        if (stationaryTime > 0.3f)
        {
            isRotating = false;
        }
    }

    void FixedUpdate()
    {
        if (rb.velocity.x > -horizontalSpeed)
        {
            rb.AddForce(Vector2.left * horizontalSpeed, ForceMode2D.Force);
        }
        if (isRotating)
        {
            transform.Rotate(0, 0, rotationSpeed * Time.fixedDeltaTime);
        }
    }
}

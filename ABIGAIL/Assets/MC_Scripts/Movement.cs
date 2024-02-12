using UnityEngine;

public class Movement : MonoBehaviour
{
    public float speed = 25f;
    public float decelerationFactor = 0.95f; // Adjust this for slower deceleration
    private Rigidbody2D rb;
    private bool isMoving;

    public bool collide = false;

    public Vector2 jumpV = new Vector2(0, 10f);

    public KeyCode k_w = KeyCode.W;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Floor"))
        {
            collide = false;
            Debug.Log("Collided with the specific 2D object!");
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Floor"))
        {
            collide = true;
        }
    }
}

    void Update()
    
    {
        float moveH = Input.GetAxis("Horizontal");
        isMoving = Mathf.Abs(moveH) > 0.01f;


        if (isMoving)
        {
            // Apply movement
            rb.velocity = new Vector2(moveH * speed, rb.velocity.y);
        }
        else
        {
            // Apply custom deceleration
            rb.velocity = new Vector2(rb.velocity.x * decelerationFactor, rb.velocity.y);
        }

        if (collide)
        {
            if (Input.GetKeyDown(k_w))
            {
                Debug.Log("JUMPIN");
                rb.AddForce(Vector2.up * speed, ForceMode2D.Impulse);
            }
        }
    }
}


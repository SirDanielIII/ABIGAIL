using UnityEngine;

public class SpiderChase : MonoBehaviour
{
    public GameObject player;
    public float speed = 13f;
    private float distance;
    public float chaseDistance = 40f;
    public float startChaseDistance = 20f;
    private bool isChasing = false;
    private Rigidbody2D rb;
    private Health playerHealth;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        distance = Vector2.Distance(transform.position, player.transform.position);
        Vector2 direction = (player.transform.position - transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        if (distance < startChaseDistance)
        {
            isChasing = true;
        }
        else if (distance > chaseDistance)
        {
            isChasing = false;
        }

        if (isChasing)
        {
            rb.velocity = direction * speed;
            transform.rotation = Quaternion.Euler(Vector3.forward * angle);
        }
        else
        {
            rb.velocity = Vector2.zero;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Abigail")) // Assuming "Abigail" is the tag of the player
        {
            playerHealth = collision.gameObject.GetComponent<Health>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(1);
            }
        }
    }
}

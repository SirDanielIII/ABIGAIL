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
    public LayerMask obstacleLayer;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        distance = Vector2.Distance(transform.position, player.transform.position);
        Vector2 direction = (player.transform.position - transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        
        Vector2 rayStart = (Vector2)transform.position + direction * 0.1f;
        RaycastHit2D hit = Physics2D.Raycast(rayStart, direction, chaseDistance, obstacleLayer);
        Debug.DrawRay(rayStart, direction * chaseDistance, Color.red);

        if (hit.collider != null)
        {
            if (hit.collider.gameObject == player && distance < startChaseDistance)
            {
                isChasing = true;
                Debug.Log("Player spotted - chase on!");
            }
            else
            {
                isChasing = false;
                Debug.Log("Lost sight of the player - chase off.");
            }
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
                Debug.Log("Player hit by spider.");
            }
        }
    }
}

using UnityEngine;

public class SpiderChase : MonoBehaviour
{
    public GameObject player;
    public float speed = 13f;
    private float distance;
    public float chaseDistance = 40f; // Max distance to continue chase
    public float startChaseDistance = 20f; // Distance to start chasing
    private bool isChasing = false; // Tracks whether the spider is currently chasing the player

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>(); // Ensure there's a Rigidbody2D component attached
    }

    void Update()
    {
        distance = Vector2.Distance(transform.position, player.transform.position);
        Vector2 direction = player.transform.position - transform.position;
        direction.Normalize();
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Check if player is within start chase distance to initiate chase
        if (distance < startChaseDistance)
        {
            isChasing = true;
        }
        // If player is out of max chase distance, stop chasing
        else if (distance > chaseDistance)
        {
            isChasing = false;
        }

        // If currently chasing, move towards the player
        if (isChasing)
        {
            transform.position = Vector2.MoveTowards(this.transform.position, player.transform.position, speed * Time.deltaTime);
            transform.rotation = Quaternion.Euler(Vector3.forward * angle);
        }
    }
}

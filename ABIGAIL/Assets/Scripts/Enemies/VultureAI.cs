using System.Collections;
using UnityEngine;

public class VultureAI : MonoBehaviour
{
    // Inspector variables for setting up the behavior
    public float swoopCooldown = 5f;
    public float patrolSpeed = 3f;
    public float swoopSpeed = 4f;
    public float YDistanceAbovePlayer = 5f;
    public int maxSwoopAttempts = 2;
    public float swoopDuration = 3f;
    public float patrolWidth = 4f;
    public float groundLevel = -4f;

    // Runtime variables
    private float swoopTimer = 3f;
    private int damage = 1;
    private int swoopCount = 0;
    private Animator animator;
    private Transform playerTransform;
    private bool isSwooping = false;
    private Vector3 patrolStartPosition;
    private Vector3 patrolEndPosition;
    private bool hasDamagedPlayer = false;

    void Start()
    {
        swoopTimer = Random.Range(3, swoopCooldown);
        animator = GetComponent<Animator>();
        playerTransform = GameObject.FindGameObjectWithTag("Abigail").transform;
        //Debug.Log("Vulture initialized - patrol from: " + patrolStartPosition + " to: " + patrolEndPosition);
    }

    void PatrolBehavior() {
        // Update patrol positions based on the player's current position
        patrolStartPosition = new Vector3(playerTransform.position.x - patrolWidth, playerTransform.position.y + YDistanceAbovePlayer, 0);
        patrolEndPosition = new Vector3(playerTransform.position.x + patrolWidth, playerTransform.position.y + YDistanceAbovePlayer, 0);

        // Calculate the ping pong value over 3 seconds for a round trip
        float lerpTime = Mathf.PingPong(Time.time * (1f / 3f), 1); // Adjust to 1/3 since PingPong goes from 0 to 1 to 0 in given duration, we want it in 3 seconds

        // Lerp between the start and end positions
        Vector3 patrolPosition = Vector3.Lerp(patrolStartPosition, patrolEndPosition, lerpTime);

        // Apply the calculated position
        transform.position = patrolPosition;

        //Debug.Log("Vulture is patrolling at: " + transform.position);
    }

    void Update()
    {
        Debug.Log(isSwooping);
        Debug.Log(animator.GetBool("IsPlayerDetected"));
        if (!isSwooping)
        {
            PatrolBehavior();
            if (swoopTimer <= 0)
            {
                swoopTimer = swoopCooldown;
                TriggerSwoopAttack();
            }
            else
            {
                swoopTimer -= Time.deltaTime;
            }
        }
        else
        {
            if (swoopCount >= maxSwoopAttempts && animator.GetCurrentAnimatorStateInfo(0).IsName("Swoop"))
            {
                TriggerFlyOffScreen();
            }
        }
    }

    void TriggerSwoopAttack()
    {
        if (!isSwooping && swoopCount < maxSwoopAttempts)
        {
            swoopTimer = swoopCooldown; // Reset the swoop timer
            isSwooping = true;
            animator.SetBool("IsPlayerDetected", true);
            Debug.Log("Vulture is swooping down, swoop count: " + swoopCount);
            StartSwoop();
        }
    }

    void StartSwoop()
    {
        Vector3 startPosition = new Vector3(playerTransform.position.x - patrolWidth, playerTransform.position.y + YDistanceAbovePlayer, 0);
        Vector3 endPosition = new Vector3(playerTransform.position.x + patrolWidth, playerTransform.position.y + YDistanceAbovePlayer, 0);
        Vector3 controlPoint = new Vector3(startPosition.x + patrolWidth, playerTransform.position.y - YDistanceAbovePlayer, 0);
        StartCoroutine(PerformSwoop(startPosition, controlPoint, endPosition));
    }

    IEnumerator PerformSwoop(Vector3 start, Vector3 cp, Vector3 end)
    {
        float t = 0f;
        while (t < 1)
        {
            t += Time.deltaTime / swoopDuration;
            transform.position = CalculateBezierPoint(t, start, cp, end);
            yield return null;
        }
        // At this point, the swoop is complete, so we set the vulture back to patrolling
        OnSwoopComplete();
    }

    private Vector3 CalculateBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2) 
    {
        // Quadratic Bezier curve formula
        float u = 1 - t;
        return u * u * p0 + 2 * u * t * p1 + t * t * p2;
    }

    public void OnSwoopComplete()
    {
        animator.SetBool("IsPlayerDetected", false);
        hasDamagedPlayer = false;
        isSwooping = false;
        swoopCount++;
        if (swoopCount >= maxSwoopAttempts)
        {
            TriggerFlyOffScreen();
        }
        else
        {
            animator.SetBool("ReturnToPatrol", true);
        }
    }

    public void TriggerFlyOffScreen()
    {
        animator.SetTrigger("FlyOffScreen");
        StartCoroutine(FlyOffScreen());
    }

    private IEnumerator FlyOffScreen()
    {
        animator.SetBool("IsPlayerDetected", false);
        animator.SetBool("ReturnToPatrol", false);
        Vector3 startPt = new Vector3(playerTransform.position.x + patrolWidth, playerTransform.position.y + YDistanceAbovePlayer, 0);
        Vector3 endPt = startPt + Vector3.up * 10f;

        float flyOffTimer = 0f;
        while (flyOffTimer < 1.5f)
        {
            flyOffTimer += Time.deltaTime / swoopDuration;
            transform.position = Vector3.Lerp(startPt, endPt, flyOffTimer);
            yield return null;
        }

        Destroy(gameObject);
    }


    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Abigail") && !hasDamagedPlayer)
        {
            other.GetComponent<Health>()?.TakeDamage(damage);
            Debug.Log("Vulture damaged the player.");
            hasDamagedPlayer = true;
        }
    }
}

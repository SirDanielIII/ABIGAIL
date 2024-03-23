using System.Collections;
using UnityEngine;

public class VultureAI : MonoBehaviour
{
    // Inspector variables for setting up the behavior
    public float YDistanceAbovePlayer = 5f;
    public int maxSwoopAttempts = 2;
    public float swoopDuration = 3f;
    public float patrolWidth = 4f;

    // Runtime variables
    private float swoopTimer;
    private int damage = 1;
    private int swoopCount = 0;
    private Animator animator;
    private Transform playerTransform;
    private bool isSwooping = false;
    private Vector3 patrolStartPosition;
    private Vector3 patrolEndPosition;
    private bool hasDamagedPlayer = false;
    private bool swooped = false;
    private bool isReturning = false;
    private bool patrolDirection = true;
    private Vector3 startingPosition;
    public float activationRange = 10f;
    private bool inRange = false;

    void Start()
    {
        startingPosition = transform.position;
        int randomInt = Random.Range(1, 4);    
        swoopTimer = randomInt * 3;
        animator = GetComponent<Animator>();
        playerTransform = GameObject.FindGameObjectWithTag("Abigail").transform;
    }

    void PatrolBehavior() {
        if (swooped && !isReturning) {
            StartCoroutine(ReturnToStartPosition());
        }
        else if (!isReturning) {
            if (patrolDirection) {
                patrolStartPosition = new Vector3(playerTransform.position.x - patrolWidth, playerTransform.position.y + YDistanceAbovePlayer, 0);
                patrolEndPosition = new Vector3(playerTransform.position.x + patrolWidth, playerTransform.position.y + YDistanceAbovePlayer, 0);
            } else {
                patrolStartPosition = new Vector3(playerTransform.position.x + patrolWidth, playerTransform.position.y + YDistanceAbovePlayer, 0);
                patrolEndPosition = new Vector3(playerTransform.position.x - patrolWidth, playerTransform.position.y + YDistanceAbovePlayer, 0);
            }

            float lerpTime = Mathf.PingPong(Time.time, 1.5f) / 1.5f;
            Vector3 patrolPosition = Vector3.Lerp(patrolStartPosition, patrolEndPosition, lerpTime);
            transform.position = patrolPosition;
        }
    }


    IEnumerator ReturnToStartPosition() {
        isReturning = true;
        Vector3 returnStart = patrolDirection ? patrolEndPosition : patrolStartPosition;
        Vector3 returnEnd = patrolDirection ? patrolStartPosition : patrolEndPosition;

        float startTime = Time.time;
        while (Time.time - startTime < 1.5f) {
            float lerpTime = (Time.time - startTime) / 1.5f;
            Vector3 patrolReturn = Vector3.Lerp(returnStart, returnEnd, lerpTime);
            transform.position = patrolReturn;
            yield return null;
        }
        patrolDirection = !patrolDirection;
        swooped = false; // Reset swooped to false after returning
        isReturning = false;
    }

    void Update()
    {
        Debug.Log(swoopTimer);
        float distanceToPlayer = Vector3.Distance(playerTransform.position, startingPosition);
        if (distanceToPlayer <= activationRange || inRange)
        {
            inRange = true;
            if (!isSwooping)
            {
                PatrolBehavior();
                if (swoopTimer <= 0)
                {
                    int randomInt2 = Random.Range(1, 6);    
                    randomInt2 = randomInt2 % 2 == 0 ? randomInt2 + 1 : randomInt2;
                    swoopTimer = randomInt2 * 1.5f;
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
    }

    void TriggerSwoopAttack()
    {
        if (!isSwooping && swoopCount < maxSwoopAttempts)
        {
            isSwooping = true;
            animator.SetBool("IsPlayerDetected", true);
            StartSwoop();
        }
    }

    void StartSwoop()
    {
        animator.SetBool("ReturnToPatrol", false);
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
        float u = 1 - t;
        return u * u * p0 + 2 * u * t * p1 + t * t * p2;
    }

    public void OnSwoopComplete()
    {
        animator.SetBool("IsPlayerDetected", false);
        hasDamagedPlayer = false;
        isSwooping = false;
        swooped = true;
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
            hasDamagedPlayer = true;
        }
    }
}

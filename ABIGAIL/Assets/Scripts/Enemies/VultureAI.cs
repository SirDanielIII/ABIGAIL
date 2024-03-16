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
    private bool swooped = false;
    private bool isReturning = false;
    private bool patrolDirection = true;

    void Start()
    {
        int randomInt = (int)Random.Range(1, swoopCooldown);    
        if (!swooped)   
        {
            swoopTimer = randomInt * 3;
        }
        else
        {
            swoopTimer = randomInt * 1.5f;
        }
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

        // Use the current patrolDirection to determine return start and end positions
        Vector3 returnStart = patrolDirection ? patrolEndPosition : patrolStartPosition;
        Vector3 returnEnd = patrolDirection ? patrolStartPosition : patrolEndPosition;

        float startTime = Time.time;
        while (Time.time - startTime < 1.5f) {
            float lerpTime = (Time.time - startTime) / 1.5f;
            Vector3 patrolReturn = Vector3.Lerp(returnStart, returnEnd, lerpTime);
            transform.position = patrolReturn;
            yield return null;
        }

        // Flip the patrol direction after returning
        patrolDirection = !patrolDirection;
        swooped = false; // Reset swooped to false after returning
        isReturning = false;
    }

    void Update()
    {
        Animator animator = GetComponent<Animator>();
        // Assuming you're interested in the base layer, which is layer 0.
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        // Log the name hash for debugging purposes
        Debug.Log("Current State Hash: " + stateInfo.fullPathHash);

        // If you know the name hash of specific states, you can compare them like this:
        if (stateInfo.fullPathHash == Animator.StringToHash("Base Layer.Patrol"))
        {
            Debug.Log("Currently in Patrol state");
        }
        else if (stateInfo.fullPathHash == Animator.StringToHash("Base Layer.FlyOff"))
        {
            Debug.Log("Currently in FlyOff state");
        }
        else if (stateInfo.fullPathHash == Animator.StringToHash("Base Layer.Swoop"))
        {
            Debug.Log("Currently in Swoop state");
        }


        if (!isSwooping)
        {
            PatrolBehavior();
            if (swoopTimer <= 0)
            {
                int randomInt = (int)Random.Range(1, swoopCooldown);    
                if (!swooped)   
                {
                    swoopTimer = randomInt * 3;
                }
                else
                {
                    swoopTimer = randomInt * 1.5f;
                }
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
            Debug.Log("Vulture damaged the player.");
            hasDamagedPlayer = true;
        }
    }
}

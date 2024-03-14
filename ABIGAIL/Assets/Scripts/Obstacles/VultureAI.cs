using System.Collections;
using UnityEngine;

public class VultureAI : MonoBehaviour
{
    public float swoopCooldown = 3f;
    public GameObject heartPrefab; // Placeholder
    public float YDistance = 4.8f;
    public int maxSwoopAttempts = 2;

    private float swoopTimer;
    private int damage = 4;
    private int swoopCount = 0;
    private Animator animator;
    private Transform playerTransform;
    private bool isSwooping = false;

    void Start()
    {
        swoopTimer = Random.Range(0, swoopCooldown);
        animator = GetComponent<Animator>();
        playerTransform = GameObject.FindGameObjectWithTag("Abigail").transform;
    }

    void Update()
    {
        if (!isSwooping)
        {
            float targetX = playerTransform.position.x;
            float targetY = playerTransform.position.y + YDistance;
            transform.position = new Vector3(targetX, targetY, transform.position.z);
        }

        if (swoopTimer <= 0 && !isSwooping)
        {
            TriggerSwoopAttack();
        }
        else
        {
            swoopTimer -= Time.deltaTime;
        }

        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Patrol") && isSwooping)
        {
            if (++swoopCount >= maxSwoopAttempts)
            {
                animator.SetTrigger("FlyOff");
                animator.SetBool("FlyOffScreen", true);
            }
            else
            {
                ResetSwoop();
            }
        }

        if (animator.GetCurrentAnimatorStateInfo(0).IsName("FlyOff"))
        {
            StartCoroutine(ResetVultureAfterFlyOff());
        }
    }

    IEnumerator ResetVultureAfterFlyOff()
    {
        yield return new WaitForSeconds(3f); // Adjust based on animation length
        swoopCount = 0;
        animator.SetBool("FlyOffScreen", false);
        ResetSwoop();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Abigail") && isSwooping)
        {
            other.GetComponent<Health>()?.TakeDamage(damage);
        }
    }

    void TriggerSwoopAttack()
    {
        animator.SetTrigger("Swoop");
        animator.SetBool("IsPlayerDetected", true);
        isSwooping = true;
    }

    private void ResetSwoop()
    {
        isSwooping = false;
        swoopTimer = Random.Range(0, swoopCooldown);
        animator.SetBool("IsPlayerDetected", false);
    }
}

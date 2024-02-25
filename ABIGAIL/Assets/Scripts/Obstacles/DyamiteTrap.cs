using UnityEngine;
using System.Collections;

public class DynamiteTrap : MonoBehaviour
{
    public float delayBeforeExplosion = 1f;
    public int damageAmount = 10;
    public float explosionRadius = 1f;
    public GameObject explosionRadiusPrefab;

    private SpriteRenderer spriteRenderer;
    private GameObject explosionRadiusIndicator;
    private bool hasExploded = false;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        explosionRadiusIndicator = Instantiate(explosionRadiusPrefab, transform.position, Quaternion.identity, transform);
        explosionRadiusIndicator.SetActive(false);
        AdjustIndicatorScale();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Abigail") && !hasExploded)
        {
            TriggerExplosion();
        }
    }

    private void TriggerExplosion()
    {
        hasExploded = true;
        spriteRenderer.color = Color.red;
        explosionRadiusIndicator.SetActive(true);
        StartCoroutine(ExplosionCountdown());
    }

    IEnumerator ExplosionCountdown()
    {
        yield return new WaitForSeconds(delayBeforeExplosion);
        Explode();
    }

    void Explode()
    {
        if (!this) return; // Check if the GameObject has already been destroyed

        Collider2D[] objectsInRadius = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
        foreach (var obj in objectsInRadius)
        {
            if (obj.CompareTag("Abigail"))
            {
                Health health = obj.GetComponent<Health>();
                if (health != null)
                {
                    health.TakeDamage(damageAmount);
                }
            }
        }

        explosionRadiusIndicator.SetActive(false);
        Destroy(gameObject);
    }

    private void AdjustIndicatorScale()
    {
        float inverseParentScaleX = 1 / transform.localScale.x;
        float inverseParentScaleY = 1 / transform.localScale.y;
        explosionRadiusIndicator.transform.localScale = new Vector3(inverseParentScaleX * explosionRadius * 2, inverseParentScaleY * explosionRadius * 2, 1);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}

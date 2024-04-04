using UnityEngine;

public class KeyPickup : MonoBehaviour
{
    public AudioSource keySound;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Abigail"))
        {
            LevelManager.Instance.CollectKey();
            Destroy(gameObject);
            keySound.Play();
        }
    }
}

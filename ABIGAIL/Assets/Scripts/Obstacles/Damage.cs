using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damage : MonoBehaviour
{
    public int damageAmount = 10;
     private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Triggered");
        if (other.CompareTag("Abigail")){

        
        // Check if the collider that entered is a player or an object that can take damage
        Health health = other.GetComponent<Health>();

        // If the object has a health component, apply damage
        if (health != null)
        {
            health.TakeDamage(damageAmount);
            Debug.Log("damage");
        }
        }
    }
}

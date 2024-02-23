using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    public int maxHealth = 10;
    private int currentHealth;

    private void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damageAmount)
    {
        currentHealth -= damageAmount;
        
        // Example: If health drops to or below 0, destroy the object
        if (currentHealth <= 0)
        {
            Debug.Log("GAME OVER");
        }
    }
}


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Silo : MonoBehaviour, IDamageable
{
    public float health = 100;

    public void TakeDamage(float amount)
    {
        health -= amount;

        if (health <= 0)
        {
            Die();
        }

    }

    public void Die()
    {
        if (!gameObject) return;
        Debug.Log("I die");
        Destroy(gameObject);

    }
}

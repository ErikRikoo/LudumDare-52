using System;
using System.Collections;
using System.Collections.Generic;
using General;
using UnityEngine;

public class Silo : MonoBehaviour, IDamageable
{
    public event InformAttackersAboutDeath InformAboutDeath;
    [SerializeField] private GameState gameState;

    private float health;
    
    private void Awake()
    {
        gameState.silo = gameObject;
        health = gameState.defaultSiloHealth;
    }

    public void TakeDamage(float amount)
    {
        health -= amount;
        GameEvents.OnSiloGotHit?.Invoke(health);

        if (health <= 0)
        {
            Die();
        }

    }


    public void Die()
    {
        if (!gameObject) return;
        Debug.Log("I die");
        InformAboutDeath?.Invoke(gameObject);
        Destroy(gameObject);

    }
}

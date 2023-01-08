using System;
using System.Collections;
using System.Collections.Generic;
using General;
using UnityEngine;

public class Silo : MonoBehaviour, IDamageable
{
    public float health = 100;
    public event InformAttackersAboutDeath InformAboutDeath;
    [SerializeField] private GameState gameState;

    private void Awake()
    {
        gameState.silo = gameObject;
    }

    public void TakeDamage(float amount)
    {
        health -= amount;
        GameEvents.OnSiloGotHit?.Invoke();

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

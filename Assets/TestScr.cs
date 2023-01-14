using System.Collections;
using System.Collections.Generic;
using General;
using UnityEngine;

public class TestScr : MonoBehaviour, IDamageable
{   
    public float hp = 1;

    public void TakeDamage(float amount)
    {
        hp -= amount;
        if (hp <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        InformAboutDeath?.Invoke(gameObject);
        Destroy(gameObject);
    }

    public event InformAttackersAboutDeath InformAboutDeath;
}

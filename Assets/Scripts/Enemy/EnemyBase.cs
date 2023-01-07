using System.Collections;
using System.Collections.Generic;
using Enemy;
using UnityEngine;


public interface IDamageable
{
    public void TakeDamage();
}

public abstract class EnemyBase : MonoBehaviour
{
    [SerializeField] protected EnemyStatsHolder stats;
    [SerializeField] protected GameState gameState;
    
    protected GameObject target;
    protected IDamageable targetIDamageaeble;

    protected float currentMoveSpeed;
    protected float currentHealth;
    protected float currentDamage;
    protected float currentAttackSpeed;


    public abstract void Spawn();
    public abstract void DetermineTarget();
    public abstract void MoveToTarget();
    public abstract void AttackTarget();

}

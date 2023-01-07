using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class Enemy : MonoBehaviour
{
    [SerializeField] private EnemyStatsHolder stats;
    private Transform target;

    private float currentMoveSpeed;
    private float currentHealth;
    private float currentDamage;
    private float currentAttackSpeed;


    public abstract void Spawn();
    public abstract void DetermineTarget();
    public abstract void MoveToTarget();
    public abstract void AttackTarget();

}

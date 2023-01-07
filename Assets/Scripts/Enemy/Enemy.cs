using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace Enemy
{
    public class Enemy : MonoBehaviour
    {
        [SerializeField] protected EnemyStatsHolder stats;
        [SerializeField] protected GameState gameState;
    
        protected GameObject target;
        protected IDamageable targetIDamageaeble;

        protected float currentMoveSpeed;
        protected float currentHealth;
        protected float currentDamage;
        protected float currentAttackSpeed;
        protected float currentAttackRange;
        
        [SerializeField] private float m_RefreshRate;
    
        private YieldInstruction m_RefreshInstruction;
        private NavMeshAgent m_Agent;
        private SphereCollider m_AttackRangeCollider;
        

        private Coroutine attackLoop = null;

        void Start()
        {
            Spawn();
            MoveToTarget();
        }
    
    
        protected virtual void Awake()
        {
            if (m_RefreshRate <= 0)
            {
                m_RefreshInstruction = new WaitForEndOfFrame();
            }
            else
            {
                m_RefreshInstruction = new WaitForSeconds(m_RefreshRate);
            }

            m_Agent = GetComponent<NavMeshAgent>();
            m_AttackRangeCollider = GetComponent<SphereCollider>();
            
            m_AttackRangeCollider.radius = stats.Range;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("EnemyAttackableObject"))
            {
                AttackTarget();
            }

        }

        private void OnTriggerExit(Collider other)
        {
            if (attackLoop != null)
            {
                StopCoroutine(attackLoop);
            }
        }


        public void Spawn()
        {
            target = gameState.silo;
            targetIDamageaeble = gameState.silo.GetComponent<IDamageable>();
            
            m_Agent.destination = target.transform.position;
            m_Agent.speed = stats.Speed;
            m_Agent.stoppingDistance = stats.Range;

            currentDamage = stats.Damage;
            currentHealth = stats.MaxHealth;
            currentAttackSpeed = stats.AttackSpeed;
            currentMoveSpeed = stats.Speed;
        }

        public void DetermineTarget()
        {

        }

        public void MoveToTarget()
        {
        
        }

        IEnumerator AttackLoop()
        {
            while (true)
            {

                if (target != null)
                {
                    targetIDamageaeble?.TakeDamage(stats.Damage);
                    Debug.Log("Attacked!");
                    yield return new WaitForSeconds(10/currentAttackSpeed);
                }
                else
                {
                    break;
                }
                
            }
        }

        public void AttackTarget()
        {
            attackLoop ??= StartCoroutine(AttackLoop());
        }


        public void Die()
        {
            if (attackLoop != null)
            {
                StopCoroutine(attackLoop);
            }
        }
    }
}

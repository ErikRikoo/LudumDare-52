using System;
using System.Collections;
using System.Collections.Generic;
using General;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.AI;

namespace Enemy
{
    
 
    
    [RequireComponent(typeof(SphereCollider))]
    public class EventBubbleComponent: MonoBehaviour
    {
        public Action<Collider> onTriggerEnterEvent;
        public Action<Collider> onTriggerExitEvent;
        public Color gizmoColor = Color.green;

        private SphereCollider sphereCollider;


        private void Awake()
        {
            sphereCollider = GetComponent<SphereCollider>();
        }


        private void OnTriggerEnter(Collider other)
        {
            onTriggerEnterEvent(other);

        }

        private void OnTriggerExit(Collider other)
        {
            onTriggerExitEvent(other);

        }
        
        protected void OnDrawGizmosSelected()
        {
            Gizmos.color = gizmoColor;
            Gizmos.DrawWireSphere(transform.position, sphereCollider.radius);
            
        }
        
    }
    
    public class Enemy : MonoBehaviour, IDamageable
    {
        [SerializeField] protected EnemyStatsHolder stats;
        [SerializeField] protected GameState gameState;
        public event InformAttackersAboutDeath InformAboutDeath;

        public GameObject target;
        protected IDamageable targetIDamageaeble;

        private Stack<GameObject> potentialTargets = new Stack<GameObject>();

        protected float currentMoveSpeed;
        protected float currentHealth;
        protected float currentDamage;
        protected float currentAttackSpeed;
        protected float currentAttackRange;
        private float squareRange;
        
        [SerializeField] private float m_RefreshRate;
    
        private YieldInstruction m_RefreshInstruction;
        private NavMeshAgent m_Agent;
        

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
            

            var attackRange = new GameObject("AttackRangeGO")
            {
                transform =
                {
                    parent = transform,
                    localPosition = Vector3.zero
                }
            };
            var attackRangeCollider  = attackRange.AddComponent<SphereCollider>();
            attackRangeCollider.isTrigger = true;
            attackRangeCollider.radius = stats.Range;
            var attackBubbleComponent = attackRange.AddComponent<EventBubbleComponent>();
            attackBubbleComponent.onTriggerEnterEvent += OnAttackRangeEnter;
            attackBubbleComponent.onTriggerExitEvent += OnAttackRangeExit;
            attackBubbleComponent.gizmoColor = Color.red;
            
            
            var visionRange = new GameObject("visionRangeGO")
            {
                transform =
                {
                    parent = transform,
                    localPosition = Vector3.zero
                }
            };
            var visionRangeCollider  = visionRange.AddComponent<SphereCollider>();
            visionRangeCollider.isTrigger = true;
            visionRangeCollider.radius = stats.VisionRange;
            var visionBubbleComponent = visionRange.AddComponent<EventBubbleComponent>();
            visionBubbleComponent.onTriggerEnterEvent += OnVisionRangeEnter;
            visionBubbleComponent.onTriggerExitEvent += OnVisionRangeExit;
            visionBubbleComponent.gizmoColor = Color.green;
            

        }

        private void OnAttackRangeEnter(Collider other)
        {
            if (other.CompareTag("EnemyAttackableObject"))
            {
                if (attackLoop == null)
                {
                    AttackTarget();
                }
                else if (other.gameObject != target)
                {
                    DetermineTarget(null);
                    AttackTarget();
                }
            }

        }

        private void OnAttackRangeExit(Collider other)
        {
            if (other.CompareTag("EnemyAttackableObject"))
            {
                if (attackLoop != null && other.gameObject != target)
                {
                    Debug.Log("Stopping attack");
                    StopCoroutine(attackLoop);
                }
                 
                DetermineTarget(null);
            }
        }


        private void OnVisionRangeEnter(Collider other)
        {
            if (other.CompareTag("EnemyAttackableObject"))
            {
                GameObject go = other.gameObject;
                if (go == target) return;
                 potentialTargets.Push(go);
                 DetermineTarget(null);
                     
            }
        }

        private void OnVisionRangeExit(Collider other)
        {
            if (other.CompareTag("EnemyAttackableObject")) Debug.Log("I am now out of vision range");
        }
        
        
        public void DetermineTarget([CanBeNull] GameObject obj)
        {
            GameObject checkTarget;
            do
            {
                checkTarget = potentialTargets.Peek();
                if (checkTarget.gameObject == null || (obj && checkTarget.gameObject == obj))
                {
                    if (potentialTargets.Count > 1) potentialTargets.Pop();
                    continue;
                }

                if (target && !(Vector3.Distance(transform.position, checkTarget.transform.position) 
                      < Vector3.Distance(transform.position, target.transform.position))) continue;
                
                m_Agent.destination = checkTarget.GetComponent<Collider>().ClosestPoint(transform.position);
                target = checkTarget;
                targetIDamageaeble = checkTarget.GetComponent<IDamageable>();
                targetIDamageaeble.InformAboutDeath += DetermineTarget;

            } while (checkTarget == null);

        }



        public void Spawn()
        {
            var closestPoint = gameState.silo.GetComponent<Collider>().ClosestPoint(transform.position);
            
            targetIDamageaeble = gameState.silo.GetComponent<IDamageable>();

            potentialTargets.Push(target);

            m_Agent.destination = closestPoint;
            m_Agent.speed = stats.Speed;
            m_Agent.stoppingDistance = stats.Range;

            currentDamage = stats.Damage;
            currentHealth = stats.MaxHealth;
            currentAttackSpeed = stats.AttackSpeed;
            currentMoveSpeed = stats.Speed;
            squareRange = Mathf.Pow(stats.Range, 2);
            
            GameEvents.OnEnemySpawned?.Invoke();

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
            if (attackLoop != null) StopCoroutine(attackLoop);
            attackLoop = StartCoroutine(AttackLoop());
        }


        public void Die()
        {
            if (attackLoop != null)
            {
                StopCoroutine(attackLoop);
            }
            GameEvents.OnEnemyKilled?.Invoke();
            InformAboutDeath?.Invoke(gameObject);
            Destroy(gameObject);
        }

        public void TakeDamage(float amount)
        {
            Debug.Log("Enemy taking damage");
            currentHealth -= amount;
            
            if (currentHealth <= 0)
            {
                Die();
            }
        }

    }
}

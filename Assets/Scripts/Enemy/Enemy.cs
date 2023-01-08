using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using General;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

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
    
    [RequireComponent(typeof(AudioSource))]
    public class Enemy : MonoBehaviour, IDamageable
    {
        [SerializeField] protected EnemyStatsHolder stats;
        [SerializeField] protected GameState gameState;
        [Header("Audio")] 
        [SerializeField] private AudioClip m_GetHitSound;
        [SerializeField] private AudioClip m_DeathSound;
        [SerializeField] private AudioClip m_AttackSound;
        [SerializeField] private AudioClip m_FootstepSound;
        [SerializeField] private ParticleSystem m_HitVFX;

        [Header("Animations")] [SerializeField]
        private Animator m_Animator;
        public event InformAttackersAboutDeath InformAboutDeath;

        public GameObject target;
        protected IDamageable targetIDamageaeble;

        private Stack<GameObject> potentialTargets = new Stack<GameObject>();

        protected float currentMoveSpeed;
        protected float currentHealth;
        protected float currentDamage;
        protected float currentAttackSpeed;
        protected float currentAttackRange;
        
        [SerializeField] private float m_RefreshRate;
    
        private YieldInstruction m_RefreshInstruction;
        private NavMeshAgent m_Agent;

        private AudioSource _audioSource;
        private Rigidbody _rigidbody;
        private BoxCollider _collider;
        private SkinnedMeshRenderer _skinnedMeshRenderer;
        private GameObject visionRange;
        private GameObject attackRange;

        private Coroutine attackLoop = null;

        void Start()
        {
            Spawn();
        }
        
        private void FixedUpdate()
        {
            m_Animator.SetFloat("Speed", m_Agent.velocity.magnitude);
           
        }
    
    
        protected virtual void Awake()
        {

            _audioSource = GetComponent<AudioSource>();
            m_Agent = GetComponent<NavMeshAgent>();
            _rigidbody = GetComponent<Rigidbody>();
            _collider = GetComponent<BoxCollider>();
            
            if (m_RefreshRate <= 0)
            {
                m_RefreshInstruction = new WaitForEndOfFrame();
            }
            else
            {
                m_RefreshInstruction = new WaitForSeconds(m_RefreshRate);
            }
            

            attackRange = new GameObject("AttackRangeGO")
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
            
            
            visionRange = new GameObject("visionRangeGO")
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

            _skinnedMeshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
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
            // if (other.CompareTag("EnemyAttackableObject")) Debug.Log("I am now out of vision range");
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
            
            GameEvents.OnEnemySpawned?.Invoke();
            gameState.numberOfEnemiesAlive++;

        }

        IEnumerator AttackLoop()
        {
            while (true)
            {

                if (target != null)
                {
                    m_Animator.SetTrigger("Punch");
                    targetIDamageaeble?.TakeDamage(stats.Damage);
                    yield return new WaitForSeconds(1/currentAttackSpeed);
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
            Destroy(gameObject);
        }

        IEnumerator DeathVFX()
        {
            if (attackLoop != null)
            {
                StopCoroutine(attackLoop);
            }
            
            m_Agent.destination = transform.position;
            m_Agent.stoppingDistance = 0;
            _audioSource.pitch = Random.Range(0.6f, 1.1f);
            _audioSource.PlayOneShot(m_DeathSound);
            _rigidbody.isKinematic = true;
            _collider.enabled = false;
            m_Animator.SetTrigger("Death");
            
            gameState.numberOfEnemiesAlive--;
            GameEvents.OnEnemyKilled?.Invoke();
            InformAboutDeath?.Invoke(gameObject);

            
            yield return new WaitForSeconds(4);
            Die();
        }

        public void TakeDamage(float amount)
        {
            currentHealth -= amount;

            _skinnedMeshRenderer.material.DOFloat(0, "_HitEffect", 0.2f).From(1);
            m_HitVFX.Play();
            DOTween.To(() => Time.timeScale, x => Time.timeScale = x, 1, 0.1f).From(0.1f);

            if (currentHealth <= 0)
            {
                StartCoroutine(DeathVFX());
            }
            else
            {
                // _audioSource.pitch = Random.Range(0.6f, 1.1f);
                // _audioSource.PlayOneShot(m_GetHitSound);
            }
        }

    }
}

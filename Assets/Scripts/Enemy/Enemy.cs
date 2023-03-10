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
        [SerializeField] private GameObject m_DeathVFX;
        [SerializeField] private GameObject m_CollectablePrefab;
        [Range(0, 1)]
        [SerializeField] private float m_CollectableLuck;
        
        

        [Header("Animations")] [SerializeField]
        private Animator m_Animator;
        public event InformAttackersAboutDeath InformAboutDeath;

        public GameObject finalTargetTarget;
        private IDamageable finalTargetIDamageaeble;

        private GameObject currentTarget;
        private IDamageable currentTargetIDamageaeble;


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
        private SphereCollider visionRangeCollider;
        private GameObject attackRange;
        private SphereCollider attackRangeCollider;

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
            attackRangeCollider  = attackRange.AddComponent<SphereCollider>();
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
            
            visionRangeCollider  = visionRange.AddComponent<SphereCollider>();
            visionRangeCollider.isTrigger = true;
            visionRangeCollider.radius = stats.VisionRange;
            var visionBubbleComponent = visionRange.AddComponent<EventBubbleComponent>();
            visionBubbleComponent.onTriggerEnterEvent += OnVisionRangeEnter;
            visionBubbleComponent.onTriggerExitEvent += OnVisionRangeExit;
            visionBubbleComponent.gizmoColor = Color.green;

            _skinnedMeshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
        }


        void SetNewTarget(GameObject newTarget)
        {
            currentTargetIDamageaeble.InformAboutDeath -= DetermineTarget;
            currentTarget = newTarget;
            m_Agent.destination = newTarget.GetComponent<Collider>().ClosestPoint(transform.position);
            currentTarget = newTarget;
            currentTargetIDamageaeble = newTarget.GetComponent<IDamageable>();
            currentTargetIDamageaeble.InformAboutDeath += DetermineTarget;
        }

        private void OnAttackRangeEnter(Collider other)
        {
            if (!other.CompareTag("EnemyAttackableObject")) return;
            
            if (attackLoop == null)
            {
                Debug.Log($"Attacking new target: {other.gameObject.name}");
                AttackTarget();
            }
            else if (other.gameObject != currentTarget)
            {
                if (attackLoop != null)
                {
                    StopCoroutine(attackLoop);
                }
                SetNewTarget(other.gameObject);
            }
            AttackTarget();

        }

        private void OnAttackRangeExit(Collider other)
        {
            if (!other.CompareTag("EnemyAttackableObject")) return;
            
            if (attackLoop != null && other.gameObject != currentTarget)
            {
                Debug.Log("Stopping attack");
                StopCoroutine(attackLoop);
            }
                 
            DetermineTarget(null);
        }


        private void OnVisionRangeEnter(Collider other)
        {
            if (!other.CompareTag("EnemyAttackableObject")) return;
            
            GameObject go = other.gameObject;
            if (go == currentTarget) return;
            // potentialTargets.Push(go);
            DetermineTarget(null);
        }

        private void OnVisionRangeExit(Collider other)
        {
            // if (other.CompareTag("EnemyAttackableObject")) Debug.Log("I am now out of vision range");
        }
        
        
        private void DetermineTarget([CanBeNull] GameObject obj)
        {
            if (attackLoop != null)
            {
                StopCoroutine(attackLoop);
            }
            // Spherecast melee range
            RaycastHit[] hits = Physics.SphereCastAll(transform.position, attackRangeCollider.radius, transform.forward, 0);
            foreach (var hit in hits)
            {
                if (obj && hit.collider.gameObject == obj || !hit.collider.gameObject.CompareTag("EnemyAttackableObject")) continue;
                SetNewTarget(hit.collider.gameObject);
                AttackTarget();
                return;

            }
            

            // Spherecast vision range
            hits = Physics.SphereCastAll(transform.position, visionRangeCollider.radius, transform.forward, 0);
            foreach (var hit in hits)
            {
                if (obj && hit.collider.gameObject == obj || !hit.collider.gameObject.CompareTag("EnemyAttackableObject")) continue;
                SetNewTarget(hit.collider.gameObject);
                return;

            }
            // Final target
            if (finalTargetTarget)
            {
                SetNewTarget(finalTargetTarget);
            }
        }


        private void Spawn()
        {
            var closestPoint = gameState.silo.GetComponent<Collider>().ClosestPoint(transform.position);
            
            finalTargetIDamageaeble = gameState.silo.GetComponent<IDamageable>();
            finalTargetTarget = gameState.silo;
            
            
            currentTarget = finalTargetTarget;
            currentTargetIDamageaeble = finalTargetIDamageaeble;
            currentTargetIDamageaeble.InformAboutDeath += DetermineTarget;
            
            m_Agent.destination = closestPoint;
            m_Agent.speed = stats.Speed + Random.Range(-1, 0.5f);
            m_Agent.stoppingDistance = stats.Range;

            currentDamage = stats.Damage;
            currentHealth = stats.MaxHealth;
            currentAttackSpeed = stats.AttackSpeed;
            currentMoveSpeed = stats.Speed;
            gameState.numberOfEnemiesAlive++;
            GameEvents.OnEnemySpawned?.Invoke();
        }

        IEnumerator AttackLoop()
        {
            while (true)
            {

                if (currentTarget != null)
                {
                    m_Animator.SetTrigger("Punch");
                    currentTargetIDamageaeble?.TakeDamage(stats.Damage);
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
            
            _collider.enabled = false;
            attackRangeCollider.enabled = false;
            visionRangeCollider.enabled = false;
            m_Agent.enabled = false;
            _audioSource.pitch = Random.Range(0.6f, 1.1f);
            _audioSource.PlayOneShot(m_DeathSound);
            _rigidbody.isKinematic = true;
            m_Animator.SetTrigger("Death");
            
            gameState.numberOfEnemiesAlive--;
            GameEvents.OnEnemyKilled?.Invoke();
            InformAboutDeath?.Invoke(gameObject);

            yield return new WaitForSeconds(4);
            var pouf = Instantiate(m_DeathVFX, transform.position, Quaternion.Euler(-90, 0, 0));
            Destroy(pouf, 5f);
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
                SpawnCollectable();
            }
        }

        private void SpawnCollectable()
        {
            var randomValue = Random.Range(0f, 1f); 
            if (randomValue < m_CollectableLuck)
            {
                Instantiate(m_CollectablePrefab, transform.position, Quaternion.identity);
            }
        }
    }
}

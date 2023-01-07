using UnityEngine;
using UnityEngine.AI;

namespace Enemy
{
    public class MeleeEnemy : EnemyBase
    {
    
        [Tooltip("<= 0 means every frame")]
        [SerializeField] private float m_RefreshRate;
    
        private YieldInstruction m_RefreshInstruction;
        private NavMeshAgent m_Agent;

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
        }

    

        public override void Spawn()
        {
            target = gameState.silo;
            m_Agent.destination = target.transform.position;
            m_Agent.stoppingDistance = stats.Range;
            m_Agent.speed = stats.Speed;
        }

        public override void DetermineTarget()
        {

        }

        public override void MoveToTarget()
        {
        
        }

        public override void AttackTarget()
        {
        }
    }
}

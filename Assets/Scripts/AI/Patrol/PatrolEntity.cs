using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Android;
using Utilties;

namespace AI.Patrol
{
    [RequireComponent(typeof(NavMeshAgent))]
    public abstract class PatrolEntity : MonoBehaviour
    {
        [Tooltip("<= 0 means every frame")]
        [SerializeField] private float m_RefreshRate;

        [SerializeField] private FloatRange m_WaitRange;
        [Min(0)]
        [SerializeField] private float m_DistanceToTargetThresholdSqr = 1;
        

        private YieldInstruction m_RefreshInstruction;
        private NavMeshAgent m_Agent;

        protected NavMeshAgent Agent => m_Agent;
        
        private Vector3 m_Target;

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

        private void Start()
        {
            StartPatroling();
        }

        public void StartPatroling()
        {
            StartCoroutine(c_PatrolRoutine());
        }

        public void StopPatroling()
        {
            StopAllCoroutines();
            // TODO: Stop the nav mesh agent?
        }

        protected abstract Vector3 NextPosition
        {
            get;
        }
        private IEnumerator c_PatrolRoutine()
        {
            while (true)
            {
                m_Target = NextPosition;
                // TODO: Check if in path
                m_Agent.destination = m_Target;
                do
                {
                    yield return m_RefreshInstruction;
                } while ((transform.position - m_Target).sqrMagnitude > m_DistanceToTargetThresholdSqr);

                yield return new WaitForSeconds(m_WaitRange.RandomValue);
            }
        }

        protected virtual void OnDrawGizmos()
        {
            if (!Application.isPlaying)
            {
                return;
            }
            
            Gizmos.DrawWireSphere(m_Target, 0.5f);
            var corners = m_Agent.path.corners;
            for (int i = 0; i < corners.Length - 1; ++i)
            {
                Gizmos.DrawLine(corners[i], corners[i + 1]);
            }
            
        }
    }
}
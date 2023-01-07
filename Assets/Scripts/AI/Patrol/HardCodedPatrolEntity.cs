using Utilities;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace AI.Patrol
{
    public enum LoopType
    {
        Loop,
        PingPong
    }
    
    public class HardCodedPatrolEntity : PatrolEntity
    {
        [SerializeField] private Vector2[] m_Positions;
        [SerializeField] private int m_StartIndex = 0;
        [DisableIf("IsApplicationPlaying")]
        [SerializeField] private LoopType m_LoopType;

        [SerializeField] private bool m_ShouldMatchFirstPosition;
        

        private bool IsApplicationPlaying => Application.isPlaying;
        
        private int m_CurrentPosition;
        private int m_IndexDelta = 1;

        protected override Vector3 NextPosition
        {
            get
            {
                m_CurrentPosition += m_IndexDelta;
                if (m_CurrentPosition == m_Positions.Length)
                {
                    switch (m_LoopType)
                    {
                        case LoopType.Loop:
                            m_CurrentPosition = 0;
                            break;
                        case LoopType.PingPong:
                            m_IndexDelta = -1;
                            --m_CurrentPosition;
                            break;
                    }
                }

                if (m_CurrentPosition < 0)
                {
                    m_CurrentPosition = 0;
                    m_IndexDelta = 1;
                }

                return m_Positions[m_CurrentPosition].X0Y();
            }
        }

        protected override void Awake()
        {
            base.Awake();
            m_CurrentPosition = m_StartIndex;
            if (m_ShouldMatchFirstPosition)
            {
                transform.position = m_Positions[0].X0Y();
            }
        }

        protected override void OnDrawGizmos()
        {
            base.OnDrawGizmos();
            if (!Selection.Contains(gameObject))
            {
                return;
            }
            Color originalColor = Gizmos.color;
            Gizmos.color = Color.red;
            foreach (var pos in m_Positions)
            {
                Gizmos.DrawWireSphere(pos.X0Y(), 0.5f);
            }
            Gizmos.color = originalColor;
        }
    }
}
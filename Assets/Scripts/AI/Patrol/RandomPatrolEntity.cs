using UnityEngine;
using Utilties;

namespace AI.Patrol
{
    public class RandomPatrolEntity : PatrolEntity
    {
        [SerializeField] private FloatRange m_RadiusRange;
        [Range(0, 180)]
        [SerializeField] private float m_AngleRange = 90;

        private float RadianAngle => m_AngleRange * 2 * Mathf.Deg2Rad;

        // TODO: Move to utility
        private Vector3 RandomOffset
        {
            get
            {
                float radianAngle = RadianAngle;
                float angle = Random.Range(-radianAngle, radianAngle);
                return new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * m_RadiusRange.RandomValue;
            }
        }

        protected override Vector3 NextPosition => transform.position + RandomOffset;

        protected override void OnDrawGizmos()
        {
            base.OnDrawGizmos();
            if (!Application.isPlaying)
            {
                return;
            }
            
            GizmosUilities.DrawWireDisc(transform.position, m_RadiusRange.Min);
            GizmosUilities.DrawWireDisc(transform.position, m_RadiusRange.Max);
            DrawGizmosAngleSight();
        }

        private void DrawGizmosAngleSight()
        {
            Vector3 matchRangeForward = transform.forward * m_RadiusRange.Max;

            Gizmos.DrawLine(transform.position, 
                transform.position + Quaternion.Euler(0, -m_AngleRange , 0) * matchRangeForward);
            Gizmos.DrawLine(transform.position, 
                transform.position + Quaternion.Euler(0, m_AngleRange , 0) * matchRangeForward);
        }
    }
}
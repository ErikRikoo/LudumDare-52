using UnityEngine;
using Utilties;

namespace AI.Patrol
{
    public class RandomPatrolEntity : PatrolEntity
    {
        [SerializeField] private FloatRange m_RadiusRange;

        // TODO: Move to utility
        private Vector3 RandomOffset
        {
            get
            {
                float angle = Random.Range(0, 2 * Mathf.PI);
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
        }
    }
}
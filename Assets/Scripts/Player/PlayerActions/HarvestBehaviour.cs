using UnityEngine;

namespace Player.PlayerActions.Harvest
{
    public class HarvestBehaviour : MonoBehaviour
    {
        [SerializeField] private PlayerStats m_Stats;

        [Min(0)]
        [SerializeField] private float m_HarvestingRadius;

        [SerializeField] private LayerMask m_HarvestMask;

        private Collider[] m_ColliderBuffer = new Collider[16];
        
        public void TryHarvestPlants()
        {
            int count = Physics.OverlapSphereNonAlloc(transform.position, m_HarvestingRadius, m_ColliderBuffer, m_HarvestMask);
            for (int i = 0; i < count; ++i)
            {
                if (m_ColliderBuffer[i].TryGetComponent<IHarvestable>(out var harvestable))
                {
                    harvestable.OnHarvested(m_Stats.gameObject);
                }
            }
        }
    }
}
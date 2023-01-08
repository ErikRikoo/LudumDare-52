using UnityEngine;

namespace Player.PlayerActions.Harvest
{
    public class HarvestBehaviour : MonoBehaviour
    {
        [Min(0)]
        [SerializeField] private float m_HarvestingRadius;

        [SerializeField] private LayerMask m_HarvestMask;
        
        private Collider[] m_ColliderBuffer;
        
        public void TryHarvestPlants()
        {
            int count = Physics.OverlapSphereNonAlloc(transform.position, m_HarvestingRadius, m_ColliderBuffer, m_HarvestMask);
            for (int i = 0; i < count; ++i)
            {
                if (m_ColliderBuffer[count].TryGetComponent<IHarvestable>(out var harvestable))
                {
                    harvestable.OnHarvested(gameObject);
                }
            }
        }
    }
}
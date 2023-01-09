using UnityEngine;

namespace Collectable
{
    public class CollectableDropper : MonoBehaviour
    {
        [SerializeField] private ACollectableMB m_CollectableToDrop;

        public void DropItem()
        {
            Instantiate(m_CollectableToDrop, transform.position, Quaternion.identity);
        }
    }
}
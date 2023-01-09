using UnityEngine;

namespace Collectable
{
    public class CollectableDropper : MonoBehaviour
    {
        [SerializeField] private ACollectableMB m_CollectableToDrop;

        public T DropItem<T>()
            where T : ACollectableMB
        {
            return Instantiate(m_CollectableToDrop, transform.position, Quaternion.identity) as T;
        }
    }
}
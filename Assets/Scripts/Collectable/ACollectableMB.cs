using UnityEngine;

namespace Collectable
{
    public abstract class ACollectableMB : MonoBehaviour, ICollectable
    {
        public abstract void OnCollected(GameObject collector);
    }
}
using UnityEngine;

namespace Collectable
{
    public interface ICollectable
    {
        void OnCollected(GameObject collector);
    }
}
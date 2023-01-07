using System.Collections.Generic;
using UnityEngine;

namespace Inventory
{
   
    public class PlayerInventory : MonoBehaviour
    {
        [SerializeField] protected int m_MaxCount;
        [SerializeField] private int m_MaxItemCount;
        
        private Dictionary<InventoryItem, int> m_Items = new();

        public IEnumerator<(InventoryItem, int)> Items {
            get
            {
                foreach (var (item, count) in m_Items)
                {
                    yield return (item, count);
                }
            }
        }

        protected void UnsafeGainItem(InventoryItem _item, int _count = 1)
        {
            if (!m_Items.TryAdd(_item, _count))
            {
                m_Items[_item] += _count;
            }
        }
        
        public int GainItem(InventoryItem _item, int _count = 1)
        {
            _count = Mathf.Min(_count, m_MaxCount);
            int amountUsed = _count;
            if (!m_Items.TryAdd(_item, _count))
            {
                if (m_Items.Count >= m_MaxItemCount)
                {
                    return 0;
                }
                
                int currentCount = m_Items[_item];
                int newAmount = currentCount + _count;
                if (newAmount > m_MaxCount)
                {
                    amountUsed -= newAmount - m_MaxCount;
                    newAmount = m_MaxCount;
                }

                m_Items[_item] = newAmount;
            }
            
            GameEvents.OnPlayerGainedItem?.Invoke(_item, amountUsed);
            return amountUsed;
        }
    }
}
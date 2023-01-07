using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utilties.Extensions;

namespace Inventory
{
    public class MappedPlayerInventory : MonoBehaviour
    {
        [SerializeField] private InventoryItem[] m_Pattern;

        private List<InventoryItem> m_AvailableItems;
        private Dictionary<InventoryItem, int> m_AcquiredItems = new();

        private void Awake()
        {
            m_AvailableItems.AddRange(m_Pattern);
        }

        public void GainRandomItem()
        {
            InventoryItem randomItem = m_AvailableItems.RandomItem();
            if (m_AcquiredItems.TryAdd(randomItem, 1) && randomItem.MaxCount == 1)
            {
                m_AvailableItems.Remove(randomItem);
            }
            else
            {
                int newAmount = m_AcquiredItems[randomItem] + 1;
                if (newAmount >= randomItem.MaxCount)
                {
                    m_AvailableItems.Remove(randomItem);
                }

                m_AcquiredItems[randomItem] = newAmount;
            }
            GameEvents.OnPlayerGainedItem(randomItem, 1);
            if (HasAllItems)
            {
                GameEvents.OnPlayerGainedAllItems?.Invoke();
            }
        }

        private bool HasAllItems => m_AvailableItems.Count == 0;
    }
}
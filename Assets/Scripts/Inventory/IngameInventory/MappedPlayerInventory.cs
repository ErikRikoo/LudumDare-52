using System;
using System.Collections.Generic;
using UnityEngine;
using Utilties.Extensions;

namespace Inventory
{
    public class MappedPlayerInventory : MonoBehaviour
    {
        [SerializeField] private InventoryItem[] m_Pattern;

        private List<InventoryItem> m_AvailableItems;
        private List<InventoryItem> m_AcquiredItems = new();

        private void Awake()
        {
            m_AvailableItems.AddRange(m_Pattern);
        }

        public void GainRandomItem()
        {
            InventoryItem randomItem = m_AvailableItems.RandomItem();
            m_AvailableItems.Remove(randomItem);
            m_AcquiredItems.Add(randomItem);
            GameEvents.OnPlayerGainedItem(randomItem, 1);
            if (m_AvailableItems.Count == 0)
            {
                GameEvents.OnPlayerGainedAllItems?.Invoke();
            }
        }
    }
}
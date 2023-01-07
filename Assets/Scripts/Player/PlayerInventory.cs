using System;
using System.Collections.Generic;
using Inventory;
using Player.PlayerActions;
using Player.PlayerActions.Weapons;
using UnityEngine;

namespace Player
{
    public class PlayerInventory : MonoBehaviour
    {
        [SerializeField] private PlayerStats m_Stats;
        
        [SerializeField] private Inventory<InventoryItem> m_Seeds;
        [SerializeField] private AWeapon m_Weapon;

        private int m_CurrentSeed;

        public int CurrentSeed
        {
            get => m_CurrentSeed;
            set
            {
                m_CurrentSeed = value;
                GameEvents.OnCurrentSeedChanged?.Invoke(m_CurrentSeed);
            }
        }

        public InventoryItem CurrentSeedItem => m_Seeds.Get(CurrentSeed);

        // TODO: Use seed item
        public void AddSeed(InventoryItem _item, int _count)
        {
            m_Seeds.AddItem(_item, _count, GameEvents.OnSeedGained);
        }

        public void ChangeWeapon(AWeapon _newWeapon)
        {
            // TODO: disable old weapon
            m_Weapon = _newWeapon;
            GameEvents.OnWeaponChanged?.Invoke(_newWeapon);
        }
    }

    [Serializable]
    class Inventory<T>
    where T : InventoryItem
    {
        [SerializeField] private int m_MaxItemCount;
        
        private Dictionary<T, int> m_Items = new();

        public void AddItem(T _item, int _count, Action<T, bool> _event)
        {
            _count = Mathf.Min(_count, _item.MaxCount);
            if (!m_Items.TryAdd(_item, _count))
            {
                m_Items[_item] = Mathf.Min(_item.MaxCount, m_Items[_item] + _count);
                _event?.Invoke(_item, false);
            }
            else
            {
                _event?.Invoke(_item, true);
            }
        }

        public bool ConsumeItem(T _item, int _count, Action<T, bool> _event)
        {
            if (m_Items.TryGetValue(_item, out int current))
            {
                if (current < _count)
                {
                    return false;
                }
                // TODO: Extract method
                int newValue = current + _count;
                m_Items[_item] = newValue;
                _event?.Invoke(_item, false);
                
                return true;
            }

            return false;
        }

        public T Get(int currentSeed)
        {
            int i = 0;
            foreach (var (item, _) in m_Items)
            {
                if (i == currentSeed)
                {
                    return item;
                }

                ++i;
            }

            return null;
        }
    }
    
}
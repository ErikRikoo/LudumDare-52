using System;
using System.Collections.Generic;
using Inventory;
using PlantHandling.PlantType;
using Player.PlayerActions;
using Player.PlayerActions.Weapons;
using UnityEngine;

namespace Player
{
    public class PlayerInventory : MonoBehaviour
    {
        [SerializeField] private PlayerStats m_Stats;
        
        [SerializeField] private Inventory<PlantType> m_Seeds;
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

        public PlantType CurrentSeedItem => m_Seeds.Get(CurrentSeed);

        public void AddSeed(PlantType _item, int _count)
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
        class CountedItem
        {
            public T Item;
            public int Count;
        }
        
        [SerializeField] private int m_MaxItemCount;

        private List<CountedItem> m_Items = new();
        

        public void AddItem(T _item, int _count, Action<T, bool> _event)
        {
            _count = Mathf.Min(_count, _item.MaxCount);
            var value = GetItem(_item);
            if (value == null)
            {
                m_Items.Add(new CountedItem()
                {
                    Count = _count,
                    Item = _item
                });
                _event?.Invoke(_item, true);
            }
            else
            {
                value.Count = Mathf.Min(_item.MaxCount, value.Count + _count);
                _event?.Invoke(_item, false);
            }
        }

        private CountedItem GetItem(T _item) => m_Items.Find(item => item.Item == _item);

        public bool ConsumeItem(T _item, int _count, Action<T, bool> _event)
        {
            var value = GetItem(_item);
            if (value == null || value.Count < _count)
            {
                return false;
            }
            
            int newValue = value.Count + _count;
            value.Count = newValue;
            _event?.Invoke(_item, false);
            return true;
        }

        public T Get(int currentSeed)
        {
            return m_Items[currentSeed].Item;
        }
    }
    
}
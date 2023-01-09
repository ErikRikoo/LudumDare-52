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
        
        [SerializeField] public Inventory<PlantType> m_Seeds;

        [SerializeField] private Transform m_WeaponHolder;
        [SerializeField] private AWeapon m_FirstWeapon;

        private List<AWeapon> m_Weapons = new();

        private AWeapon CurrentWeapon
        {
            get => m_Stats.CurrentWeapon;
            set
            {
                m_Stats.CurrentWeapon = m_FirstWeapon;
                GameEvents.OnWeaponChanged?.Invoke(value);
            }
        }
        
        private int m_CurrentSeed;

        public int CurrentSeed
        {
            get => m_CurrentSeed;
            set
            {
                m_CurrentSeed = value;
                m_CurrentSeed %= m_Seeds.Count;
                GameEvents.OnCurrentSeedChanged?.Invoke(m_CurrentSeed);
            }
        }

        private void Awake()
        {
            m_Weapons.Add(m_FirstWeapon);
            CurrentWeapon = m_FirstWeapon;
            GameEvents.OnWeaponAmmoEntirelyConsumed += OnWeaponConsumed;
        }

        private void OnWeaponConsumed()
        {
            CurrentWeapon = m_Weapons[0];
        }

        public PlantType CurrentSeedItem => m_Seeds.Get(CurrentSeed)?.Item;

        public void AddSeed(PlantType _item, int _count)
        {
            m_Seeds.AddItem(_item, _count, GameEvents.OnSeedGained);
        }

        public void ChangeWeapon(AWeapon _newWeapon)
        {
            if (_newWeapon == null)
            {
                return;
            }
            
            if (CurrentWeapon != null)
            {
                CurrentWeapon.enabled = false;
            }

            var foundWeapon = m_Weapons.Find(weapon => weapon.GetType() == _newWeapon.GetType());
            if (foundWeapon == null)
            {
                // Use a transform position
                foundWeapon = Instantiate(_newWeapon, Vector3.zero, Quaternion.identity, m_WeaponHolder);
            }

            CurrentWeapon = foundWeapon;
        }
    }

    [Serializable]
    public class Inventory<T>
    where T : InventoryItem
    {
        public class CountedItem
        {
            public T Item;
            public int Count;
        }
        
        [SerializeField] private int m_MaxItemCount;

        private List<CountedItem> m_Items = new();

        public int Count => m_Items.Count;
        

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

        public CountedItem GetItem(T _item) => m_Items.Find(item => item.Item == _item);

        public bool ConsumeItem(T _item, int _count, Action<T, bool> _event)
        {
            var value = GetItem(_item);
            if (value == null || value.Count < _count)
            {
                return false;
            }
            
            int newValue = value.Count - _count;
            value.Count = newValue;
            _event?.Invoke(_item, false);
            return true;
        }

        public CountedItem Get(int currentSeed)
        {
            if (currentSeed < 0 || currentSeed >= m_Items.Count)
            {
                return null;
            }
            
            return m_Items[currentSeed];
        }
    }
    
}
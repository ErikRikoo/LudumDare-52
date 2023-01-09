using System;
using System.Collections.Generic;
using Inventory;
using PlantHandling;

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
        [SerializeField] private PlantList m_Plants;
        

        [SerializeField] private Transform m_WeaponHolder;
        [SerializeField] private AWeapon m_FirstWeapon;
        

        [SerializeField] private AudioSource _source;
        [SerializeField] private AudioClip _pickpClip;

        private List<AWeapon> m_Weapons = new();

        private AWeapon CurrentWeapon
        {
            get => m_Stats.CurrentWeapon;
            set
            {
                m_Stats.CurrentWeapon = value;
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

        private void OnEnable()
        {
            foreach (var plantType in m_Plants.plantTypes)
            {
                m_Seeds.AddItem(plantType, 0, (_, _) => {});
            }

            GameEvents.OnCurrentSeedChanged += OnCurrentSeedChanged;
        }

        private void OnDisable()
        {
            GameEvents.OnCurrentSeedChanged -= OnCurrentSeedChanged;
        }
        
        private void OnCurrentSeedChanged(int _index)
        {
            m_CurrentSeed = _index;
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
            _source.PlayOneShot(_pickpClip);
            m_Seeds.AddItem(_item, _count, GameEvents.OnSeedGained);
        }

        public void ChangeWeapon(AWeapon _newWeapon)
        {
            if (_newWeapon == null)
            {
                return;
            }
            


            var foundWeapon = m_Weapons.Find(weapon => weapon.GetType() == _newWeapon.GetType());
            if (foundWeapon == null)
            {
                if (CurrentWeapon != null)
                {
                    CurrentWeapon.gameObject.SetActive(false);
                }
                // Use a transform position
                foundWeapon = Instantiate(_newWeapon, m_WeaponHolder);
                foundWeapon.transform.localPosition = Vector3.zero;
                foundWeapon.transform.localRotation = _newWeapon.transform.localRotation;
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
using PlantHandling.PlantType;
using System;
using UnityEngine;

namespace Player.PlayerActions.Harvest.Implementation
{
    public class PlantHarvestable : MonoBehaviour, IHarvestable
    {
        public PlantType Seed;
        public int Count;
        public System.Guid plantId;

        [NonSerialized] public static Action<System.Guid> WhenHarvested;

        public void OnHarvested(GameObject _harvester)
        {
            if (_harvester.TryGetComponent<PlayerStats>(out var stats))
            {
                WhenHarvested.Invoke(plantId);
                stats.Inventory.AddSeed(Seed, Count);
                stats.Inventory.ChangeWeapon(Seed.Weapon);
            }
        }
    }
}
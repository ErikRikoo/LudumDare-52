using PlantHandling.PlantType;
using System;
using UnityEngine;

namespace Player.PlayerActions.Harvest.Implementation
{
    public class PlantHarvestable : MonoBehaviour, IHarvestable
    {
        public PlantType Seed;
        public int Count;
        private System.Guid plantId;

        public Action<System.Guid> WhenHarvested;

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
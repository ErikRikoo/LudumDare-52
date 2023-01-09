using PlantHandling.PlantType;
using UnityEngine;

namespace Player.PlayerActions.Harvest.Implementation
{
    public class PlantHarvestable : MonoBehaviour, IHarvestable
    {
        public PlantType Seed;
        public int Count;

        public void OnHarvested(GameObject _harvester)
        {
            if (_harvester.TryGetComponent<PlayerStats>(out var stats))
            {
                stats.Inventory.AddSeed(Seed, Count);
                stats.Inventory.ChangeWeapon(Seed.Weapon);
            }
        }
    }
}
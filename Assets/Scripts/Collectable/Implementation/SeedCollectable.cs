using System;
using System.Linq;
using PlantHandling.PlantType;
using Player.PlayerActions;
using UnityEngine;
using Utilties;
using Random = UnityEngine.Random;

namespace Collectable.Implementation
{
    [Serializable]
    public struct PlantItem
    {
        public PlantType Type;
        [Range(0, 1)]
        public float Luck;
    }
    
    public class SeedCollectable : ACollectableMB
    {
        // Add editor for the luck?
        [SerializeField] private PlantItem[] m_items;
        [SerializeField] private FloatRange m_CountRange;

        public override void OnCollected(GameObject collector)
        {
            if (collector.TryGetComponent<PlayerStats>(out var stats))
            {
                PlantType randomSeed = GetRandomPlant();
                int count = (int) m_CountRange.RandomValue;
                stats.Inventory.AddSeed(randomSeed, count);
            }
        }

        private PlantType GetRandomPlant()
        {
            float totalLuck = m_items.Select(item => item.Luck).Aggregate((sum, item) => sum + item);

            float randomLuck = Random.Range(0, totalLuck);
            float accumulate = 0;
            foreach (var item in m_items)
            {
                accumulate += item.Luck;

                if (randomLuck < accumulate)
                {
                    return item.Type;
                }
            }

            // TODO: assert cause there is an error if we get there
            return null;
        }
    }
}
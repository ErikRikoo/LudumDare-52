using UnityEngine;

namespace Player.PlayerActions
{
    public class PlantingBehavior : MonoBehaviour
    {
        [SerializeField] private PlayerStats m_Stats;
        
        public void PlantSeed()
        {
            if (m_Stats.Inventory.CurrentSeedItem == null)
            {
                return;
            }
            
            m_Stats.Inventory.m_Seeds.ConsumeItem(m_Stats.Inventory.CurrentSeedItem, 1, (plant, _) =>
            {
                GameEvents.OnSeedPlanted?.Invoke(plant, transform.position);
                Debug.Log("Planting");
            });
        }
    }
}
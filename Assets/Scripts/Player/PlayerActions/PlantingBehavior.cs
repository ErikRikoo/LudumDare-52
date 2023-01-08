using UnityEngine;

namespace Player.PlayerActions
{
    public class PlantingBehavior : MonoBehaviour
    {
        [SerializeField] private PlayerStats m_Stats;
        
        public void PlantSeed()
        {
            GameEvents.OnSeedPlanted?.Invoke(m_Stats.Inventory.CurrentSeedItem, transform.position);
            Debug.Log("Planting");
        }
    }
}
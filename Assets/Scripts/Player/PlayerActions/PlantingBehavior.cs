using UnityEngine;

namespace Player.PlayerActions
{
    public class PlantingBehavior : MonoBehaviour
    {
        [SerializeField] private PlayerStats m_Stats;
        [SerializeField] private AudioSource _as;
        [SerializeField] private AudioClip _clip;
        
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
            
            _as.PlayOneShot(_clip);
        }
    }
}
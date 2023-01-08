
using Inventory;
using Sirenix.OdinInspector;
using UnityEngine;


namespace PlantHandling.PlantType
{
    [CreateAssetMenu(fileName = "Plant Type", menuName = "LD 52/Plants/Type", order = 0)]
    // Would it be abstract for some strategy pattern?
    public class PlantType : InventoryItem
    {
        public enum TypeEnum
        {
            Melee,
            Pistol,
            Shotgun,
            AutoRifle,
            Sniper
        }
        
        public PlantType type;
        
        [MinMaxSlider(0, 60)]
        public Vector2 GrowthTimeRange;
        [HideInInspector]
        public Vector2Int ShapeSize;
        [HideInInspector]
        public bool[] Shape;
    }
}
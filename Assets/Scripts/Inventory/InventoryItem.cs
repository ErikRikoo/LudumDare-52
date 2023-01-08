using UnityEngine;

namespace Inventory
{
    [UnityEngine.CreateAssetMenu(fileName = "InventoryItem", menuName = "LD 52/Inventory/Item", order = 0)]
    public class InventoryItem : ScriptableObject
    {
        [SerializeField] private Sprite m_Icon;
        public Sprite Icon => m_Icon;

        [SerializeField] private string m_ItemName;
        public string ItemName => m_ItemName;


        [SerializeField] private int m_MaxCount = 1;
        public int MaxCount => m_MaxCount;
    }
}
using UnityEngine;

namespace Inventory
{
    [UnityEngine.CreateAssetMenu(fileName = "InventoryItem", menuName = "LD52/Inventory/Item", order = 0)]
    public class InventoryItem : ScriptableObject
    {
        [SerializeField] private Sprite m_Texture;
        [SerializeField] private string m_Name;
    }
}
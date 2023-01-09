using UnityEngine;

namespace PlantHandling.PlantType
{
    [CreateAssetMenu(fileName = "PlantList", menuName = "LD 52/Plants/List", order = 0)]
    // Would it be abstract for some strategy pattern?
    public class PlantList : ScriptableObject
    {
        [SerializeField] public PlantType[] plantTypes;

        public int Length => plantTypes.Length;

        public PlantType this[int _index] => plantTypes[_index];
    }
}
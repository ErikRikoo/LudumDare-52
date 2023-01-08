//To be replaced with a more robust system later
//just writing this to get the basic functionality working
using UnityEngine;
namespace PlantHandling{
    [CreateAssetMenu(fileName = "Plant Manager", menuName = "LD 52/Plants/Manager", order = 0)]
    public class PlantManager: ScriptableObject{
        public float cellSize;
        private class LandPlot
        {
            Vector2 position;
            Vector2Int size;
        }
    }
}
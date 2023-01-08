﻿
using Sirenix.OdinInspector;
using UnityEngine;
using Utilities;
using Utilties;

namespace PlantHandling.PlantType
{
    [CreateAssetMenu(fileName = "Plant Type", menuName = "LD 52/Plants/Type", order = 0)]
    // Would it be abstract for some strategy pattern?
    public class APlantType : ScriptableObject
    {
        [MinMaxSlider(0, 60)]
        public Vector2 GrowthTimeRange;
        [HideInInspector]
        public Vector2Int ShapeSize;
        [HideInInspector]
        public bool[] Shape;
    }
}
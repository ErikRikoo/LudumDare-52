using System;
using Random = UnityEngine.Random;

namespace Utilties
{
    [Serializable]
    public class FloatRange
    {
        // TODO: Check if Min and Max value from odin would work
        public float Min;
        
        public float Max;

        public float RandomValue => Random.Range(Min, Max);
    }
}
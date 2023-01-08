//To be replaced with a more robust system later
//just writing this to get the basic functionality working
using Freya;
using System.Collections.Generic;
using UnityEngine;
using Utilities;
using UnityRandom = UnityEngine.Random;

namespace PlantHandling
{
    [CreateAssetMenu(fileName = "Plant Manager", menuName = "LD 52/Plants/Manager", order = 0)]
    public class PlantManager : ScriptableObject
    {
        public float cellSize;
        public PlantType.PlantType[] plantTypes;
        public List<LandPlot> LandPlots;
        
        private class PlantedSeed
        {
            int[] filledSlots;
            PlantType.PlantType type;
            double plantTime;
            public PlantedSeed(PlantType.PlantType type, int[] filledSlots, float plantTime)
            {
                this.type = type;
                this.filledSlots = filledSlots;
                this.plantTime = plantTime;
            }
        }
        public class LandPlot
        {
            public Rect Rect => rect;
            Rect rect;
            public Vector2Int size;
            Dictionary<System.Guid, PlantedSeed> plantedSeeds;
            bool[] slots;

            public LandPlot(Vector2Int size, Rect rect)
            {
                plantedSeeds = new Dictionary<System.Guid, PlantedSeed>();
                this.size = size;
                slots = new bool[size.x * size.y];
                this.rect = rect;
            }

            public bool Overlaps(Rect other)
            {
                return rect.Overlaps(other);
            }
        }

        void GenerateLandPlots(int landPlotCount, Vector2 radiusRange, Vector2Int minSize, Vector2Int maxSize, float landPlotPadding)
        {
            var landPlots = new List<LandPlot>();
            var landPlotSize = new Vector2Int(UnityRandom.Range(minSize.x, maxSize.x), UnityRandom.Range(minSize.y, maxSize.y));

            const int maxIterationCount = 1000;
            const int maxSizeChangeCount = 10;

            int iterationCount = 0;
            int sizeChangeCount = 0;
            while (landPlots.Count < landPlotCount && sizeChangeCount < maxSizeChangeCount)
            {
                iterationCount++;
                var randomPosition = UnityRandom.insideUnitCircle * UnityRandom.Range(radiusRange.x, radiusRange.y);
                randomPosition = randomPosition.ClampMagnitude(radiusRange.x, radiusRange.y);
                var positionOffset = ((randomPosition.Sign() * -1.0f).Clamp01() * -1.0f) * (landPlotSize.ToFloat() * cellSize);
                var position = randomPosition + positionOffset;
                var rect = new Rect(position, landPlotSize.ToFloat() * cellSize);
                var paddedRect = new Rect(rect.position - Vector2.one * landPlotPadding, rect.size + Vector2.one * landPlotPadding * 2);
                bool overlaps = false;
                foreach (var landPlot in landPlots)
                {
                    if (landPlot.Overlaps(paddedRect))
                    {
                        overlaps = true;
                        break;
                    }
                }
                if (!overlaps)
                {
                    landPlots.Add(new LandPlot(landPlotSize, rect));
                    //Debug.Log("Iter count : " + iterationCount + " | Size change count : " + sizeChangeCount);
                    iterationCount = 0;
                    sizeChangeCount = 0;
                    landPlotSize = new Vector2Int(UnityRandom.Range(minSize.x, maxSize.x), UnityRandom.Range(minSize.y, maxSize.y));
                }else
                {
                    if (iterationCount >= maxIterationCount)
                    {
                        iterationCount = 0;
                        sizeChangeCount++;
                        landPlotSize = new Vector2Int(UnityRandom.Range(minSize.x, maxSize.x), UnityRandom.Range(minSize.y, maxSize.y));
                    }
                }
            }
            //Debug.Log("Iter count : " + iterationCount + " | Size change count : " + sizeChangeCount);
            this.LandPlots = landPlots;
        }

        public void Initialize(int landPlotCount, Vector2 radiusRange, Vector2Int minSize, Vector2Int maxSize, float landPlotPadding)
        {
            GenerateLandPlots(landPlotCount, radiusRange, minSize, maxSize, landPlotPadding);
        }
    }
}
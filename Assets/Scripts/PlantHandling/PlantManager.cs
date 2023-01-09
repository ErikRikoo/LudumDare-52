//To be replaced with a more robust system later
//just writing this to get the basic functionality working
using Freya;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using TreeEditor;
using UnityEngine;
using UnityEngine.Assertions;
using Utilities;
using UnityRandom = UnityEngine.Random;

namespace PlantHandling
{
    [CreateAssetMenu(fileName = "Plant Manager", menuName = "LD 52/Plants/Manager", order = 0)]
    public class PlantManager : ScriptableObject
    {
        public float cellSize;
        [MinMaxSlider(0.0f, 30.0f)]
        public Vector2 blockTimeRange;
        public PlantType.PlantType[] plantTypes;
        public List<LandPlot> landPlots;
        public GameObject LandPlotGO;

        public Vector2[] TransformSlotCoordinatesToPositions(Vector2Int[] slotCoords, int landPlotIndex)
        {
            var positions = new List<Vector2>();
            var landPlot = this.landPlots[landPlotIndex];
            var startPosition = landPlot.Rect.position + new Vector2(this.cellSize, this.cellSize) * 0.5f;
            foreach (var coord in slotCoords)
            {
                positions.Add(startPosition + coord.ToFloat() * this.cellSize);
            }
            return positions.ToArray();
        }

        void GetRandomLandPlotPosition(Vector2 radiusRange, Vector2Int landPlotSize, float landPlotPadding, out Rect rect, out Rect paddedRect)
        {
            var randomPosition = UnityRandom.insideUnitCircle * UnityRandom.Range(radiusRange.x, radiusRange.y);
            randomPosition = randomPosition.ClampMagnitude(radiusRange.x, radiusRange.y);
            var positionOffset = ((randomPosition.Sign() * -1.0f).Clamp01() * -1.0f) * (landPlotSize.ToFloat() * cellSize);
            var position = randomPosition + positionOffset;
            rect = new Rect(position, landPlotSize.ToFloat() * cellSize);
            paddedRect = new Rect(rect.position - Vector2.one * landPlotPadding, rect.size + Vector2.one * landPlotPadding * 2);
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
                GetRandomLandPlotPosition(radiusRange, landPlotSize, landPlotPadding, out var rect, out var paddedRect);
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
                    var landPlotGO = Instantiate(LandPlotGO);
                    var landPlot = landPlotGO.GetComponent<LandPlot>();
                    Assert.IsNotNull(landPlot);
                    
                    landPlot.Initialize(rect, landPlotSize);
                    landPlots.Add(landPlot);
                    landPlotGO.name = $"LandPlot-{landPlots.Count}";

                    iterationCount = 0;
                    sizeChangeCount = 0;
                    landPlotSize = new Vector2Int(UnityRandom.Range(minSize.x, maxSize.x), UnityRandom.Range(minSize.y, maxSize.y));
                }
                else
                {
                    if (iterationCount >= maxIterationCount)
                    {
                        iterationCount = 0;
                        sizeChangeCount++;
                        landPlotSize = new Vector2Int(UnityRandom.Range(minSize.x, maxSize.x), UnityRandom.Range(minSize.y, maxSize.y));
                    }
                }
            }
            this.landPlots = landPlots;
        }

        public bool GetLandPlotAtPosition(Vector2 position, out int landPlotIndex)
        {
            for (int i = 0; i < this.landPlots.Count; i++)
            {
                var plot = this.landPlots[i];
                if (plot.Rect.Contains(position))
                {
                    landPlotIndex = i;
                    return true;
                }
            }
            landPlotIndex = -1;
            return false;
        }

        public bool GetLandPlotAndSlotAt(Vector2 position, out int landPlotIndex, out Vector2Int slotCoord)
        {
            if (!GetLandPlotAtPosition(position, out landPlotIndex))
            {
                slotCoord = Vector2Int.zero;
                return false;
            }
            return landPlots[landPlotIndex].GetSlotCoordinate(position, this.cellSize, out slotCoord);
        }

        public void Initialize(int landPlotCount, Vector2 radiusRange, Vector2Int minSize, Vector2Int maxSize, float landPlotPadding)
        {
            GenerateLandPlots(landPlotCount, radiusRange, minSize, maxSize, landPlotPadding);
        }
    }
}
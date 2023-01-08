//To be replaced with a more robust system later
//just writing this to get the basic functionality working
using Freya;
using System.Collections.Generic;
using TreeEditor;
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
        public struct LandPlotSlot
        {
            public enum SlotState
            {
                Ready,
                Planted,
                Harvested
            }
            public SlotState State;
            public System.Guid guid;
            public double harvestedTime;

            public void SetReady()
            {
                this.State = SlotState.Ready;
                this.guid = System.Guid.Empty;
                this.harvestedTime = -1.0f;
            }
            public bool IsReady()
            {
                return this.State == SlotState.Ready;
            }
            public void SetPlanted(System.Guid guid)
            {
                this.State = SlotState.Planted;
                this.guid = guid;
                this.harvestedTime = -1.0f;
            }
            public bool IsPlanted()
            {
                return this.State == SlotState.Planted;
            }
            public void SetHarvested(double harvestedTime)
            {
                this.State = SlotState.Harvested;
                this.guid = System.Guid.Empty;
                this.harvestedTime = harvestedTime;
            }
            public bool IsHarvested()
            {
                return this.State == SlotState.Harvested;
            }
        }
        public class LandPlot
        {
            public Rect Rect => rect;
            Rect rect;
            public Vector2Int size;
            Dictionary<System.Guid, PlantedSeed> plantedSeeds;
            LandPlotSlot[] slots;

            public LandPlot(Vector2Int size, Rect rect)
            {
                plantedSeeds = new Dictionary<System.Guid, PlantedSeed>();
                this.size = size;
                slots = new LandPlotSlot[size.x * size.y];
                for (int i = 0; i < slots.Length; i++)
                {
                    slots[i] = new LandPlotSlot();
                    slots[i].SetReady();
                }
                this.rect = rect;
            }

            public bool Overlaps(Rect other)
            {
                return rect.Overlaps(other);
            }

            public bool GetPossiblePlantPlacement(Vector2Int slotPosition, PlantType.PlantType plant, out Vector2Int[] usedSlots)
            {
                //Check if slot position is valid
                if (slotPosition.x < 0 || slotPosition.y < 0 || slotPosition.x >= size.x || slotPosition.y >= size.y)
                {
                    usedSlots = new Vector2Int[0];
                    return false;
                }
                //Check if slot position is available
                var slotIndex = slotPosition.x + slotPosition.y * this.size.x;
                if (!this.slots[slotIndex].IsReady())
                {
                    usedSlots = new Vector2Int[0];
                    return false;
                }
                //
                for (int i = 0; i < plant.ShapeSize.x; i++)
                {
                    for (int j = 0; j < plant.ShapeSize.y; j++)
                    {
                        var startPosition = slotPosition - new Vector2Int(i, j);
                        if (CheckPlantFit(startPosition, plant, false, out var uSlots))
                        {
                            usedSlots = uSlots;
                            return true;
                        }
                        startPosition = slotPosition - new Vector2Int(j, i);
                        if (CheckPlantFit(startPosition, plant, false, out var uSlotsRev))
                        {
                            usedSlots = uSlotsRev;
                            return true;
                        }
                    }
                }
                usedSlots = new Vector2Int[0];
                return false;
            }

            public bool CheckPlantFit(Vector2Int startPosition, PlantType.PlantType plant, bool rotateShape, out Vector2Int[] usedSlots)
            {
                var uSlots = new List<Vector2Int>();
                for (int i = 0; i < plant.ShapeSize.x; i++)
                {
                    for (int j = 0; j < plant.ShapeSize.y; j++)
                    {
                        var slotPos = startPosition + (rotateShape ? new Vector2Int(j, i) : new Vector2Int(i, j));
                        if (slotPos.x < 0 || slotPos.y < 0 || slotPos.x >= this.size.x || slotPos.y >= this.size.y)
                        {
                            usedSlots = new Vector2Int[0];
                            return false;
                        }

                        var sIndex = slotPos.x + slotPos.y * this.size.x;
                        if (!plant.ShapeAt(i, j)) continue;
                        if (this.slots[sIndex].IsReady())
                        {
                            uSlots.Add(slotPos);
                            continue;
                        }
                        //the plant does not fit
                        usedSlots = new Vector2Int[0];
                        return false;
                    }
                }
                usedSlots = uSlots.ToArray();
                return true;


            }
        }

        public Vector2[] TransformSlotCoordinatesToPositions(Vector2Int[] slotCoords, int landPlotIndex)
        {
            var positions = new List<Vector2>();
            var landPlot = this.LandPlots[landPlotIndex];
            var startPosition = landPlot.Rect.position + new Vector2(this.cellSize, this.cellSize) * 0.5f;
            foreach (var coord in slotCoords)
            {
                positions.Add(startPosition + coord.ToFloat() * this.cellSize);
            }
            return positions.ToArray();
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
            //Debug.Log("Iter count : " + iterationCount + " | Size change count : " + sizeChangeCount);
            this.LandPlots = landPlots;
        }

        public void Initialize(int landPlotCount, Vector2 radiusRange, Vector2Int minSize, Vector2Int maxSize, float landPlotPadding)
        {
            GenerateLandPlots(landPlotCount, radiusRange, minSize, maxSize, landPlotPadding);
        }
    }
}
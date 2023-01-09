using Freya;
using Sirenix.OdinInspector;
using System.Collections;
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
        [ReadOnly]
        public List<LandPlot> landPlots;
        public GameObject LandPlotGO;
        public Vector2 TimeBetweenLandPlotGeneration;

        public Mesh cursorQuad;
        public Material cursorMaterial;
        public Material landPlotTimer;

        [Header("Land Plot Generation Properties")]
        [SerializeField]
        private float landPlotPadding;
        [SerializeField]
        private int landPlotMaxCount;
        [SerializeField]
        private Vector2 landPlotGenerationRange;
        public Vector2 LandPlotGenerationRange => landPlotGenerationRange;
        [SerializeField]
        private Vector2Int landPlotMinSize;
        [SerializeField]
        private Vector2Int landPlotMaxSize;

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

        public struct LandPlotInitData
        {
            public Rect rect;
            public Vector2Int size;
            public LandPlotInitData(Rect rect, Vector2Int size)
            {
                this.rect = rect;
                this.size = size;
            }
        }

        Vector2Int GetRandomV2Int(Vector2Int minSize, Vector2Int maxSize)
        {
            return new Vector2Int(UnityRandom.Range(minSize.x, maxSize.x), UnityRandom.Range(minSize.y, maxSize.y));
        }

        void CreateLandPlotGO(LandPlotInitData initData)
        {
            var landPlotGO = Instantiate(LandPlotGO);
            var landPlot = landPlotGO.GetComponent<LandPlot>();
            Assert.IsNotNull(landPlot);

            landPlot.Initialize(initData.rect, initData.size);
            landPlots.Add(landPlot);
            landPlotGO.name = $"LandPlot-{landPlots.Count}";
        }

        List<LandPlotInitData> GenerateLandPlots(int landPlotCount, Vector2 radiusRange, Vector2Int minSize, Vector2Int maxSize, float landPlotPadding)
        {
            var landPlotInit = new List<LandPlotInitData>();
            var landPlotSize = GetRandomV2Int(minSize, maxSize);

            const int maxIterationCount = 1000;
            const int maxSizeChangeCount = 10;

            int iterationCount = 0;
            int sizeChangeCount = 0;
            while (landPlotInit.Count < landPlotCount && sizeChangeCount < maxSizeChangeCount)
            {
                iterationCount++;
                GetRandomLandPlotPosition(radiusRange, landPlotSize, landPlotPadding, out var rect, out var paddedRect);
                bool overlaps = false;
                foreach (var landPlot in landPlotInit)
                {
                    if (landPlot.rect.Overlaps(paddedRect))
                    {
                        overlaps = true;
                        break;
                    }
                }
                if (!overlaps)
                {
                    landPlotInit.Add(new LandPlotInitData(rect, landPlotSize));
                    iterationCount = 0;
                    sizeChangeCount = 0;
                    landPlotSize = GetRandomV2Int(minSize, maxSize);
                }
                else
                {
                    if (iterationCount >= maxIterationCount)
                    {
                        iterationCount = 0;
                        sizeChangeCount++;
                        landPlotSize = GetRandomV2Int(minSize, maxSize);
                    }
                }
            }
            return landPlotInit;
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

        // int landPlotCount, Vector2 radiusRange, Vector2Int minSize, Vector2Int maxSize, float landPlotPadding
        public void Initialize(Planter planter)
        {
            this.landPlots = new List<LandPlot>();
            var landPlotInit = GenerateLandPlots(landPlotMaxCount, landPlotGenerationRange, landPlotMinSize, landPlotMaxSize, landPlotPadding);
            Debug.Log($"Land count {landPlotInit.Count}");
            landPlotInit.Sort((plotA, plotB) => plotA.rect.center.magnitude.CompareTo(plotB.rect.center.magnitude));
            CreateLandPlotGO(landPlotInit[0]);
            planter.StartLandGenCoroutine(landPlotInit);
        }

        private readonly int _VerticalFillID = Shader.PropertyToID("_VerticalFill");

        public IEnumerator GeneratePlotsOverTime(List<LandPlotInitData> landPlotInit)
        {
            for (int landPLotIndex = 1; landPLotIndex < landPlotInit.Count; landPLotIndex++)
            {
                Debug.Log($"Generate plot # {landPLotIndex}");
                var genTimer = UnityRandom.Range(TimeBetweenLandPlotGeneration.x, TimeBetweenLandPlotGeneration.y);
                var startTime = Time.time;
                while (true)
                {
                    var elapsedTime = Time.time - startTime;
                    if (elapsedTime >= genTimer)
                    {
                        CreateLandPlotGO(landPlotInit[landPLotIndex]);
                        break;
                    }
                    else
                    {
                        var landRect = landPlotInit[landPLotIndex].rect;
                        Graphics.DrawMesh(cursorQuad,
                            Matrix4x4.TRS(
                                landRect.center.X0Y() + new Vector3(0.0f, 0.5f, 0.0f),
                                Quaternion.AngleAxis(90.0f, Vector3.right),
                                landRect.size.XYtoXYZ(1.0f)),
                            landPlotTimer, 0);
                        landPlotTimer.SetFloat(_VerticalFillID, (elapsedTime / genTimer).Clamp01());
                    }
                    yield return null;
                }
                yield return null;
            }
            yield break;
        }

        public void RenderCurrentCursor(Vector2[] slotPositions)
        {
            foreach (var position in slotPositions)
            {
                Graphics.DrawMesh(cursorQuad,
                Matrix4x4.TRS(
                    position.X0Y() + new Vector3(0.0f, 0.5f, 0.0f),
                    Quaternion.AngleAxis(90.0f, Vector3.right),
                    Vector3.one * cellSize * 0.9f),
                cursorMaterial, 0);
            }
        }
    }
}
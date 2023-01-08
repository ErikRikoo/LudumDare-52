using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;
using Utilities;

namespace PlantHandling
{
    public class Planter : MonoBehaviour
    {
        public PlantManager PlantManager;

        private Camera _mainCamera;
        private Plane _groundPlane;
        private Vector3 _lastMousePosition;
        private bool _initialized;
        [SerializeField]
        private float landPlotPadding;
        [SerializeField]
        private int landPlotMaxCount;
        [SerializeField]
        private Vector2 landPlotGenerationRange;
        [SerializeField]
        private Vector2Int landPlotMinSize;
        [SerializeField]
        private Vector2Int landPlotMaxSize;

        //Debug
        [Header("Debug -----")]
        [SerializeField]
        private int plantType;
        [SerializeField]
        private Material _debugMaterial;
        private Rect lastPlotRect;
        private Vector2[] slotPositions;
        
        readonly int baseColorID = Shader.PropertyToID("_BaseColor");
        private List<MaterialPropertyBlockComponent[]> landPlotSlotMPBs;
        private List<MeshRenderer[]> landPlotSlotRenderers;


        // Start is called before the first frame update
        void Start()
        {
            //Get primitive cube mesh
            _mainCamera = Camera.main;
            _groundPlane = new UnityEngine.Plane(Vector3.up, Vector3.zero);
            _initialized = true;
            Assert.IsNotNull(PlantManager, "Plant Manager is null");
            Assert.IsNotNull(_debugMaterial, "Debug Material is null");

            PlantManager.Initialize(landPlotMaxCount, landPlotGenerationRange, landPlotMinSize, landPlotMaxSize, landPlotPadding);

            var cubePrimitive = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cubePrimitive.GetComponent<MeshRenderer>().sharedMaterial = _debugMaterial;
            GameObject.Destroy(cubePrimitive.GetComponent<BoxCollider>());

            var plotMPB = new MaterialPropertyBlock();
            plotMPB.SetColor(baseColorID, Color.green);

            landPlotSlotMPBs = new List<MaterialPropertyBlockComponent[]>();
            landPlotSlotRenderers = new List<MeshRenderer[]>();
            foreach (var landPlot in PlantManager.landPlots)
            {
                var landPlotGO = GameObject.Instantiate(cubePrimitive, transform);
                landPlotGO.transform.position = landPlot.Rect.center.X0Y();
                landPlotGO.transform.localScale = new Vector3(landPlot.Rect.width, 0.1f, landPlot.Rect.height);
                landPlotGO.GetComponent<MeshRenderer>().SetPropertyBlock(plotMPB);

                var slotMPBs = new MaterialPropertyBlockComponent[landPlot.slots.Length];
                landPlotSlotMPBs.Add(slotMPBs);
                var SlotRenderers = new MeshRenderer[landPlot.slots.Length];
                landPlotSlotRenderers.Add(SlotRenderers);

                var startPosition = landPlot.Rect.position.X0Y() + new Vector3(PlantManager.cellSize * 0.5f, 0, PlantManager.cellSize * 0.5f);
                for (int i = 0; i < landPlot.size.x; i++)
                {
                    for (int j = 0; j < landPlot.size.y; j++)
                    {
                        var slotIndex = i + j * landPlot.size.x;

                        var slotGO = GameObject.Instantiate(cubePrimitive, transform);
                        var slotPosition = startPosition + new Vector3(i * PlantManager.cellSize, 0.012f, j * PlantManager.cellSize);
                        slotGO.transform.position = slotPosition;
                        slotGO.transform.localScale = new Vector3(PlantManager.cellSize * 0.85f, 0.1f, PlantManager.cellSize * 0.85f);

                        var mpb = slotGO.AddComponent<MaterialPropertyBlockComponent>();
                        slotMPBs[slotIndex] = mpb;
                        mpb.Initialize();
                        mpb.MaterialPropertyBlock.SetColor(baseColorID, Color.red);

                        var renderer = slotGO.GetComponent<MeshRenderer>();
                        SlotRenderers[slotIndex] = renderer;
                        renderer.SetPropertyBlock(mpb.MaterialPropertyBlock);
                    }
                }
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (_mainCamera is null) _mainCamera = Camera.main;
            //Get ray from mouse position
            Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
            _groundPlane.Raycast(ray, out var distance);
            var hitPoint = ray.GetPoint(distance);
            var planePoint = hitPoint.XZ();

            lastPlotRect = new Rect();
            var slots = new List<Vector2Int>();
            if (PlantManager.GetLandPlotAndSlotAt(planePoint, out var landPlotIndex, out var slotCoord))
            {
                var landPlot = PlantManager.landPlots[landPlotIndex];
                lastPlotRect = landPlot.Rect;

                slots.Add(slotCoord);
                if (plantType >= 0 && plantType < PlantManager.plantTypes.Length)
                {
                    var plant = PlantManager.plantTypes[this.plantType];
                    if (Input.GetMouseButtonUp(0))
                    {
                        Debug.Log("Hey mouse up");
                        if (landPlot.PlantSeed(slotCoord, plant, out var usedSlots))
                        {
                            slots.AddRange(usedSlots);
                        }
                    }
                    else
                    {
                        if(landPlot.GetPossiblePlantPlacement(slotCoord, plant, out var usedSlots))
                        {
                            slots.AddRange(usedSlots);
                        }
                    }
                }
                slotPositions = PlantManager.TransformSlotCoordinatesToPositions(slots.ToArray(), landPlotIndex);
            }
            else
            {
                slotPositions = new Vector2[0];
            }
            UpdateLandPlots();
            //Debuging
            _lastMousePosition = hitPoint + Vector3.up * 0.05f;
        }

        void UpdateLandPlots()
        {
            for (int i = 0; i < PlantManager.landPlots.Count; i++)
            {
                var landPlot = PlantManager.landPlots[i];
                for (int j = 0; j < landPlot.slots.Length; j++)
                {
                    var slot = landPlot.slots[j];
                    if (slot.IsReady())
                    {
                        landPlotSlotMPBs[i][j].MaterialPropertyBlock.SetColor(baseColorID, Color.grey);
                    }
                    else if (slot.IsPlanted())
                    {
                        landPlotSlotMPBs[i][j].MaterialPropertyBlock.SetColor(baseColorID, Color.blue);
                    }
                    else if (slot.IsHarvested())
                    {
                        landPlotSlotMPBs[i][j].MaterialPropertyBlock.SetColor(baseColorID, Color.black);
                    }
                    else
                    {
                        Debug.LogError("Invalid slot state");
                    }
                    landPlotSlotRenderers[i][j].SetPropertyBlock(landPlotSlotMPBs[i][j].MaterialPropertyBlock);
                }
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(_lastMousePosition, 0.5f);
            Gizmos.DrawWireSphere(Vector3.zero, landPlotGenerationRange.x);
            Gizmos.DrawWireSphere(Vector3.zero, landPlotGenerationRange.y);

            if (Application.isPlaying)
            {
                var positionCenter = lastPlotRect.center.X0Y() + new Vector3(0, 0.15f, 0);
                for (int i = 0; i < 9; i++)
                {
                    Gizmos.DrawWireCube(positionCenter, lastPlotRect.size.X0Y() * (1.0f - (float)i * 0.1f));
                }
                if (slotPositions == null || slotPositions.Length == 0) return;
                var color = Gizmos.color;
                for (int i = 1; i < slotPositions.Length; i++)
                {
                    var slotPos = slotPositions[i];
                    Gizmos.color = Color.grey;
                    Gizmos.DrawCube(slotPos.X0Y() + new Vector3(0, 0.25f, 0), new Vector3(1.0f, 0.2f, 1.0f) * PlantManager.cellSize * 0.8f);
                }
                {
                    var slotPos = slotPositions[0];
                    Gizmos.color = Color.cyan;
                    Gizmos.DrawCube(slotPos.X0Y() + new Vector3(0, 0.35f, 0), new Vector3(1.0f, 0.3f, 1.0f) * PlantManager.cellSize * 0.6f);
                }

                Gizmos.color = color;
            }
        }
    }
}
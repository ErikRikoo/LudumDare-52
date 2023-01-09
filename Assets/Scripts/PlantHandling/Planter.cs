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
        [SerializeField]
        private PlantManager plantManager;
        [SerializeField]
        private Transform playerTransform;

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


        // Start is called before the first frame update
        void Start()
        {
            //Get primitive cube mesh
            _mainCamera = Camera.main;
            _groundPlane = new UnityEngine.Plane(Vector3.up, Vector3.zero);
            _initialized = true;

            Assert.IsNotNull(plantManager, "Plant Manager is null");
            Assert.IsNotNull(_debugMaterial, "Debug Material is null");

            plantManager.Initialize(this, landPlotMaxCount, landPlotGenerationRange, landPlotMinSize, landPlotMaxSize, landPlotPadding);
        }

        public void StartLandGenCoroutine(List<PlantManager.LandPlotInitData> landPlotInit)
        {
            StartCoroutine(plantManager.GeneratePlotsOverTime(landPlotInit));
        }

        private void OnEnable()
        {
            GameEvents.OnCurrentSeedChanged += OnCurrentSeedChanged;
        }

        private void OnDisable()
        {
            GameEvents.OnCurrentSeedChanged -= OnCurrentSeedChanged;
        }

        void OnCurrentSeedChanged(int plantType)
        {
            this.plantType = plantType;
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
            var cursorSlots = new List<Vector2Int>();
            if (plantManager.GetLandPlotAndSlotAt(planePoint, out var landPlotIndex, out var slotCoord))
            {
                var landPlot = plantManager.landPlots[landPlotIndex];
                lastPlotRect = landPlot.Rect;

                slots.Add(slotCoord);
                if (plantType >= 0 && plantType < plantManager.plantTypes.Length)
                {
                    var plant = plantManager.plantTypes[this.plantType];
                    if (Input.GetMouseButtonUp(1))
                    {
                        Debug.Log("Hey mouse up");
                        if (landPlot.PlantSeed(slotCoord, plant, out var usedSlots))
                        {
                            cursorSlots.AddRange(usedSlots);
                        }
                    }
                    else
                    {
                        if(landPlot.GetPossiblePlantPlacement(slotCoord, plant, out var usedSlots))
                        {
                            cursorSlots.AddRange(usedSlots);
                        }
                    }
                }
                var cursorSlotPositions = plantManager.TransformSlotCoordinatesToPositions(cursorSlots.ToArray(), landPlotIndex);
                plantManager.RenderCurrentCursor(cursorSlotPositions);
            }
            else
            {
                slotPositions = new Vector2[0];
            }
            _lastMousePosition = hitPoint + Vector3.up * 0.05f;
        }

        /*private void OnDrawGizmos()
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
                    Gizmos.DrawCube(slotPos.X0Y() + new Vector3(0, 0.25f, 0), new Vector3(1.0f, 0.2f, 1.0f) * plantManager.cellSize * 0.8f);
                }
                {
                    var slotPos = slotPositions[0];
                    Gizmos.color = Color.cyan;
                    Gizmos.DrawCube(slotPos.X0Y() + new Vector3(0, 0.35f, 0), new Vector3(1.0f, 0.3f, 1.0f) * plantManager.cellSize * 0.6f);
                }

                Gizmos.color = color;
            }
        }*/
    }
}
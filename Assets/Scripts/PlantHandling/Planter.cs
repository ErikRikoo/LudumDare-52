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
        private int plantType;

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

            plantManager.Initialize(this);
        }

        public void StartLandGenCoroutine(List<PlantManager.LandPlotInitData> landPlotInit)
        {
            StartCoroutine(plantManager.GeneratePlotsOverTime(landPlotInit));
        }

        private void OnEnable()
        {
            GameEvents.OnCurrentSeedChanged += OnCurrentSeedChanged;
            GameEvents.OnSeedPlanted += OnSeedPlanted;
        }

        private void OnDisable()
        {
            GameEvents.OnCurrentSeedChanged -= OnCurrentSeedChanged;
            GameEvents.OnSeedPlanted -= OnSeedPlanted;

        }
        
        private void OnSeedPlanted(PlantType.PlantType choosen, Vector3 _)
        {
            Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
            _groundPlane.Raycast(ray, out var distance);
            var hitPoint = ray.GetPoint(distance);
            var planePoint = hitPoint.XZ();

            lastPlotRect = new Rect();
            var slots = new List<Vector2Int>();
            if (plantManager.GetLandPlotAndSlotAt(planePoint, out var landPlotIndex, out var slotCoord))
            {
                var landPlot = plantManager.landPlots[landPlotIndex];
                lastPlotRect = landPlot.Rect;

                if (landPlot.PlantSeed(slotCoord, choosen, out var usedSlots))
                {
                    GameEvents.OnSuccessfulPlantedSeed.Invoke(choosen);
                }
            }
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
                if (plantType >= 0 && plantType < plantManager.plants.Length)
                {
                    var plant = plantManager.plants[this.plantType];
                    
                    if(landPlot.GetPossiblePlantPlacement(slotCoord, plant, out var usedSlots))
                    {
                        cursorSlots.AddRange(usedSlots);
                    }
                    
                }
                var cursorSlotPositions = plantManager.TransformSlotCoordinatesToPositions(cursorSlots.ToArray(), landPlotIndex);
                plantManager.RenderCurrentCursor(cursorSlotPositions);
            }

            _lastMousePosition = hitPoint + Vector3.up * 0.05f;
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(_lastMousePosition, 0.5f);
            if (plantManager == null) return;
            Gizmos.DrawWireSphere(Vector3.zero, plantManager.LandPlotGenerationRange.x);
            Gizmos.DrawWireSphere(Vector3.zero, plantManager.LandPlotGenerationRange.y);
        }
    }
}
using System.Collections;
using System.Collections.Generic;
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
        [SerializeField]
        private Material _debugMaterial;

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

            var baseColorID = Shader.PropertyToID("_BaseColor");
            var plotMPB = new MaterialPropertyBlock();
            plotMPB.SetColor(baseColorID, Color.green);
            var slotMPB = new MaterialPropertyBlock();
            slotMPB.SetColor(baseColorID, Color.red);

            foreach (var landPlot in PlantManager.LandPlots)
            {
                var landPlotGO = GameObject.Instantiate(cubePrimitive, transform);
                landPlotGO.transform.position = landPlot.Rect.center.X0Y();
                landPlotGO.transform.localScale = new Vector3(landPlot.Rect.width, 0.1f, landPlot.Rect.height);
                landPlotGO.GetComponent<MeshRenderer>().SetPropertyBlock(plotMPB);

                var startPosition = landPlot.Rect.position.X0Y() + new Vector3(PlantManager.cellSize * 0.5f, 0, PlantManager.cellSize * 0.5f);
                for (int i = 0; i < landPlot.size.x; i++)
                {
                    for (int j = 0; j < landPlot.size.y; j++)
                    {
                        var slotGO = GameObject.Instantiate(cubePrimitive, transform);
                        var slotPosition = startPosition + new Vector3(i * PlantManager.cellSize, 0.012f, j * PlantManager.cellSize);
                        slotGO.transform.position = slotPosition;
                        slotGO.transform.localScale = new Vector3(PlantManager.cellSize * 0.85f, 0.1f, PlantManager.cellSize * 0.85f);
                        slotGO.GetComponent<MeshRenderer>().SetPropertyBlock(slotMPB);
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

            //Debuging
            _lastMousePosition = hitPoint + Vector3.up * 0.05f;
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(_lastMousePosition, 0.5f);
            Gizmos.DrawWireSphere(Vector3.zero, landPlotGenerationRange.x);
            Gizmos.DrawWireSphere(Vector3.zero, landPlotGenerationRange.y);
            /*if (_initialized)
            {
                foreach (var landPlot in PlantManager.LandPlots)
                {
                    Gizmos.DrawWireCube(landPlot.Rect.center.X0Y(), landPlot.Rect.size.X0Y());
                }
            }*/ 
        }
    }
}
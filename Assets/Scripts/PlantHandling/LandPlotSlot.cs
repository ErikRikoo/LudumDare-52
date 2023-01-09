using Player.PlayerActions.Harvest.Implementation;
using System.Collections;
using UnityEngine;
 
namespace PlantHandling
{
    public class LandPlotSlot : MonoBehaviour
    {
        public PlantManager plantManager;

        [Header("State Mesh")]
        public Mesh availableMesh;
        public Mesh occupiedMesh;
        public Mesh blockedMesh;

        [Header("Renderer")]
        public MeshFilter meshFilter;
        public Transform meshTransform;

        [Header("Info Effects")]
        public MaterialPropertyBlockComponent blockTimerEditor;
        public MaterialPropertyBlockComponent growthTimeEditor;
        public GameObject readyParticlesGO;

        [Header("System References")]
        public LandPlot landPlot;
        [SerializeField]
        private PlantHarvestable plantHarvestable;

        private readonly int _timeID = Shader.PropertyToID("_CTime");
        public enum SlotState
        {
            Available,
            Growing,
            Ready,
            Blocked
        }
        [HideInInspector]
        public SlotState State;
        [HideInInspector]
        public System.Guid guid;

        public void Start()
        {
            this.transform.localScale = Vector3.one * plantManager.cellSize;
            this.SetAvailable();
        }
        //-----------------------
        public void SetAvailable()
        {
            this.State = SlotState.Available;
            this.guid = System.Guid.Empty;

            meshFilter.sharedMesh = availableMesh;
            blockTimerEditor.gameObject.SetActive(false);
            growthTimeEditor.gameObject.SetActive(false);
            readyParticlesGO.gameObject.SetActive(false); 
            plantHarvestable.gameObject.SetActive(false);
        }
        public bool IsAvailable()
        {
            return this.State == SlotState.Available;
        }
        
        public void SetPlanted(System.Guid guid)
        {
            this.State = SlotState.Growing;
            this.guid = guid;

            meshFilter.sharedMesh = occupiedMesh;
            if (landPlot.plantedSeeds.TryGetValue(this.guid, out var plantedSeed))
            {
                blockTimerEditor.gameObject.SetActive(false);
                growthTimeEditor.gameObject.SetActive(true);
                readyParticlesGO.gameObject.SetActive(false);
                plantHarvestable.gameObject.SetActive(false);

                plantHarvestable.Seed = plantedSeed.type;
                float growTime = Random.Range(plantedSeed.type.GrowthTimeRange.x, plantedSeed.type.GrowthTimeRange.y);
                StartCoroutine(GrowPlant(growTime));
            }
            else
            {
                throw new System.Exception("Invalid GUID");
            }
        }
        public bool IsPlanted()
        {
            return this.State == SlotState.Growing;
        }

        public void SetReady()
        {
            blockTimerEditor.gameObject.SetActive(false);
            growthTimeEditor.gameObject.SetActive(false);
            readyParticlesGO.gameObject.SetActive(true);
            plantHarvestable.gameObject.SetActive(true);
        }

        public void SetBlocked()
        {
            this.State = SlotState.Blocked;
            this.guid = System.Guid.Empty;

            meshFilter.sharedMesh = blockedMesh;

            blockTimerEditor.gameObject.SetActive(true);
            growthTimeEditor.gameObject.SetActive(false);
            readyParticlesGO.gameObject.SetActive(false);
            plantHarvestable.gameObject.SetActive(false);
            StartCoroutine(ResetToAvailable());
        }
        public bool IsBlocked()
        {
            return this.State == SlotState.Blocked;
        }
        
        IEnumerator ResetToAvailable()
        {
            var blockTime = Random.Range(plantManager.blockTimeRange.x, plantManager.blockTimeRange.y);
            var initialTime = Time.time;

            while (true)
            {
                var elapsedTime = Time.time - initialTime;
                if (elapsedTime >= blockTime) break;
                var elapsedNormTime = Mathf.Clamp01(elapsedTime / blockTime);

                var _block = blockTimerEditor.MaterialPropertyBlock;
                _block.SetFloat(_timeID, 1.0f - elapsedNormTime);
                //-----------------
                yield return null;
            }
            this.SetAvailable();
            yield break;
        }

        IEnumerator GrowPlant(float growTime)
        {
            var initialTime = Time.time;

            while (true)
            {
                var elapsedTime = Time.time - initialTime;
                if (elapsedTime >= growTime) break;
                var elapsedNormTime = Mathf.Clamp01(elapsedTime / growTime);

                if (growthTimeEditor == null || growthTimeEditor.MaterialPropertyBlock == null) yield break;
                var _block = growthTimeEditor.MaterialPropertyBlock;
                _block.SetFloat(_timeID, 1.0f - elapsedNormTime);
                //-----------------
                yield return null;
            }
            this.SetReady();
            yield break;
        }
    }
}

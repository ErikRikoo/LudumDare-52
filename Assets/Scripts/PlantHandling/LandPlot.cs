using Freya;
using PlantHandling;
using PlantHandling.PlantType;
using Player.PlayerActions.Harvest.Implementation;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;

public class LandPlot : MonoBehaviour
{
    public PlantManager plantManager;
    public Rect Rect => rect;
    Rect rect;
    public Vector2Int size;
    public Dictionary<System.Guid, PlantedSeed> plantedSeeds;
    public LandPlotSlot[] slots;
    public GameObject groundMesh;
    public GameObject landPlotSlotGO;

    private readonly int _sizeID = Shader.PropertyToID("_Size");
    private readonly int _radiusID = Shader.PropertyToID("_Radius");

    public class PlantedSeed
    {
        public int[] filledSlots;
        public PlantType type;
        public double plantTime;
        public PlantedSeed(PlantType type, int[] filledSlots, float plantTime)
        {
            this.type = type;
            this.filledSlots = filledSlots;
            this.plantTime = plantTime;
        }
    }

    public void Initialize(Rect rect, Vector2Int size)
    {
        this.size = size;
        this.rect = rect;
    }//PlantHarvestable

    public void Start()
    {
        //generate the thing
        this.transform.position = new Vector3(rect.x, 0.02f, rect.y);
        groundMesh.transform.position = new Vector3(rect.center.x, 0.02f, rect.center.y);
        var meshSize = rect.size + new Vector2(1.5f, 1.5f) * plantManager.cellSize;
        groundMesh.transform.localScale = meshSize.XYtoXYZ(1.0f);
        var groundMPB = groundMesh.GetComponent<MaterialPropertyBlockComponent>();
		groundMPB.Initialize();
		groundMPB.MaterialPropertyBlock.SetVector(_sizeID, new Vector4(meshSize.x * 0.5f, meshSize.y * 0.5f, 0.0f, 0.0f));
		groundMPB.MaterialPropertyBlock.SetFloat(_radiusID, Mathf.Min(rect.width, rect.height) * 0.25f);
		groundMesh.GetComponent<MeshRenderer>().SetPropertyBlock(groundMPB.MaterialPropertyBlock);

        plantedSeeds = new Dictionary<System.Guid, PlantedSeed>();

        var startPosition = new Vector2(0.5f, 0.5f) * plantManager.cellSize;
        var cellSize = plantManager.cellSize;
        slots = new LandPlotSlot[size.x * size.y];
        for (int i = 0; i < size.x; i++)
        {
            for (int j = 0; j < size.y; j++)
            {
                var index = i + j * size.x;
                var slotGO = GameObject.Instantiate(landPlotSlotGO, this.transform);
                slotGO.transform.localPosition = startPosition.X0Y() + new Vector3(i, 0, j) * cellSize;
                slots[index] = slotGO.GetComponent<LandPlotSlot>();
                slots[index].landPlot = this;
                var plantHarvestable = slotGO.GetComponentInChildren<PlantHarvestable>();
                plantHarvestable.WhenHarvested += PlantHarvested;
            }
        }
    }

    public void PlantHarvested(System.Guid id)
    {
        if (this.plantedSeeds.Remove(id, out var plant))
        {
            foreach (var slot in plant.filledSlots)
            {
                this.slots[slot].SetBlocked();
            }
        }
    }

    public void PlantDestroyed(System.Guid id)
    {
        if (this.plantedSeeds.Remove(id, out var plant))
        {
            foreach (var slot in plant.filledSlots)
            {
                this.slots[slot].SetBlocked();
            }
        }
    }

    public bool Overlaps(Rect other)
    {
        return rect.Overlaps(other);
    }

    public bool GetSlotCoordinate(Vector2 position, float cellSize, out Vector2Int coord)
    {
        if (!this.rect.Contains(position))
        {
            coord = Vector2Int.zero;
            return false;
        }
        var localPosition = position - this.rect.position;
        coord = (localPosition / cellSize).FloorToInt();
        return true;
    }

    public bool PlantSeed(Vector2Int slotCoord, PlantType plant, out Vector2Int[] usedSlots)
    {
        if (!GetPossiblePlantPlacement(slotCoord, plant, out usedSlots)) return false;
        var plantGuid = System.Guid.NewGuid();
        var usedIndices = new List<int>();
        foreach (var coord in usedSlots)
        {
            var index = coord.x + coord.y * this.size.x;
            usedIndices.Add(index);
        }
        var seed = new PlantedSeed(plant, usedIndices.ToArray(), Time.time);
        this.plantedSeeds.Add(plantGuid, seed);
        foreach (var index in usedIndices)
        {
            this.slots[index].SetPlanted(plantGuid);
        }
        return true;
    }

    public bool GetPossiblePlantPlacement(Vector2Int slotCoord, PlantType plant, out Vector2Int[] usedSlots)
    {
        //Check if slot position is valid
        if (slotCoord.x < 0 || slotCoord.y < 0 || slotCoord.x >= size.x || slotCoord.y >= size.y)
        {
            usedSlots = new Vector2Int[0];
            return false;
        }
        //Check if slot position is available
        var slotIndex = slotCoord.x + slotCoord.y * this.size.x;
        if (!this.slots[slotIndex].IsAvailable())
        {
            usedSlots = new Vector2Int[0];
            return false;
        }
        //
        for (int i = 0; i < plant.ShapeSize.x; i++)
        {
            for (int j = 0; j < plant.ShapeSize.y; j++)
            {
                var startPosition = slotCoord - new Vector2Int(i, j);
                if (CheckPlantFit(startPosition, plant, false, out var uSlots))
                {
                    usedSlots = uSlots;
                    return true;
                }
                startPosition = slotCoord - new Vector2Int(j, i);
                if (CheckPlantFit(startPosition, plant, true, out var uSlotsRev))
                {
                    usedSlots = uSlotsRev;
                    return true;
                }
            }
        }
        usedSlots = new Vector2Int[0];
        return false;
    }

    public bool CheckPlantFit(Vector2Int startPosition, PlantType plant, bool rotateShape, out Vector2Int[] usedSlots)
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
                if (this.slots[sIndex].IsAvailable())
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
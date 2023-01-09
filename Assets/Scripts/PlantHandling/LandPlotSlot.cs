using PlantHandling;
using System.Collections;
using UnityEngine;

public class LandPlotSlot: MonoBehaviour
{
    public PlantManager plantManager;

    public Mesh availableMesh;
    public Mesh occupiedMesh;
    public Mesh blockedMesh;
	
    public MeshFilter meshFilter;
	public Transform meshTransform;
	public MeshRenderer timerRenderer;
	private MaterialPropertyBlock timerMPB;
    private readonly int _timeID = Shader.PropertyToID("_CTime");
    public enum SlotState
	{
		Available,
		Occupied,
		Blocked
	}
    [HideInInspector]
    public SlotState State;
    [HideInInspector]
    public System.Guid guid;

    public void Start()
    {
		timerMPB = new MaterialPropertyBlock();
        this.transform.localScale = Vector3.one * plantManager.cellSize;
		this.SetAvailable();
    }
    //-----------------------
    public void SetAvailable()
	{
		this.State = SlotState.Available;
		this.guid = System.Guid.Empty;

		meshFilter.sharedMesh = availableMesh;
        timerRenderer.gameObject.SetActive(false);
    }
	public bool IsAvailable()
	{
		return this.State == SlotState.Available;
	}
    //-----------------------
    public void SetOccupied(System.Guid guid)
	{
		this.State = SlotState.Occupied;
		this.guid = guid;

        meshFilter.sharedMesh = occupiedMesh;
    }
	public bool IsOccupied()
	{
		return this.State == SlotState.Occupied;
	}
	//-----------------------
	public void SetBlocked()
	{
		this.State = SlotState.Blocked;
		this.guid = System.Guid.Empty;

        meshFilter.sharedMesh = blockedMesh;
		timerRenderer.gameObject.SetActive(true);
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

			timerMPB.SetFloat(_timeID, 1.0f - elapsedNormTime);
			timerRenderer.SetPropertyBlock(timerMPB);
			//-----------------
            yield return null;
		}
        this.SetAvailable();
        yield break;
    }
}
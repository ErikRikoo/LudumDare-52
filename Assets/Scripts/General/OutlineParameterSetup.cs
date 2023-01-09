using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutlineParameterSetup : MonoBehaviour
{
    private SkinnedMeshRenderer _meshRenderer;
    private int _variableId;

    private void Awake()
    {
        _meshRenderer = GetComponent<SkinnedMeshRenderer>();
        _variableId = Shader.PropertyToID("_ObjectId");
    }

    void Start()
    {
        var rng = new System.Random(transform.position.GetHashCode() + 1);
        var rand = rng.Next();
        var scaled = (float) rand/int.MaxValue;
        _meshRenderer.material.SetFloat(_variableId, scaled);

    }
    
}

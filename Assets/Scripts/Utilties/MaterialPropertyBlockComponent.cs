using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialPropertyBlockComponent : MonoBehaviour
{
    private bool _initialized;
    public MaterialPropertyBlock MaterialPropertyBlock;

    // Start is called before the first frame update
    void Start()
    {
        if (_initialized) return;
        
    }

    public void Initialize()
    {
        _initialized = true;
        MaterialPropertyBlock = new MaterialPropertyBlock();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

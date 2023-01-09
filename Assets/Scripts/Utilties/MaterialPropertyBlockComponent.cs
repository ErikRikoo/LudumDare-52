using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialPropertyBlockComponent : MonoBehaviour
{
    private bool _initialized;
    public MaterialPropertyBlock MaterialPropertyBlock;
    private MeshRenderer renderer;
    public bool setOnUpdate;

    // Start is called before the first frame update
    void Start()
    {
        this.Initialize();
        renderer = gameObject.GetComponent<MeshRenderer>();
    }

    public void Initialize()
    {
        if (_initialized) return;
        _initialized = true;
        MaterialPropertyBlock = new MaterialPropertyBlock();
    }

    // Update is called once per frame
    void Update()
    {
        if (setOnUpdate)
        {
            renderer.SetPropertyBlock(this.MaterialPropertyBlock);
        }
    }
}

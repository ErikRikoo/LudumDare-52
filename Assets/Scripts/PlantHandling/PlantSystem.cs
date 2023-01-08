using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantSystem : MonoBehaviour
{
    private Camera _mainCamera;
    private Plane _groundPlane;

    private Vector3 _lastMousePosition;

    // Start is called before the first frame update
    void Start()
    {
        //Get primitive cube mesh
        _mainCamera = Camera.main;
        _groundPlane = new UnityEngine.Plane(Vector3.up, Vector3.zero);
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
        Gizmos.DrawWireSphere(Vector3.zero, 2.0f);
        
    }
}

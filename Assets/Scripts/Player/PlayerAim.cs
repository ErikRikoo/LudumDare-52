using System;
using Player.PlayerActions.Weapons;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class PlayerAim : MonoBehaviour
    {
        
        private Plane m_GroundCollisionPlane;
        private Camera m_MainCamera;

        private void Awake()
        {
            m_GroundCollisionPlane = new Plane(Vector3.up, Vector3.zero);
            m_MainCamera = Camera.main;
        }

        private void Update()
        {
            Vector2 mousePosition = Mouse.current.position.ReadValue();
            Ray ray = m_MainCamera.ScreenPointToRay(mousePosition);
            if (m_GroundCollisionPlane.Raycast(ray, out float distance))
            {
                Vector3 pos = ray.GetPoint(distance);
                pos.y = transform.position.y;
                transform.LookAt(pos);
            }
        }
    }
}
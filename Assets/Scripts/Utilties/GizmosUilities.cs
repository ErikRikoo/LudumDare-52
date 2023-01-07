using UnityEditor;
using UnityEngine;

namespace Utilties
{
    public class GizmosUilities
    {
        private const float GIZMO_DISK_THICKNESS = 0.01f;
        public static void DrawWireDisc(Vector3 _position, float _radius, Vector3 _up)
        {
            // https://answers.unity.com/questions/842981/draw-2d-circle-with-gizmos.html
            Handles.DrawWireDisc(_position, _up, _radius, 1);
        }

        public static void DrawWireDisc(Vector3 _position, float _radius)
        {
            DrawWireDisc(_position, _radius, Vector3.up);
        }
    }
}
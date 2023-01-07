using UnityEngine;

namespace Utilities
{
    public static class VectorExtension
    {
        public static Vector2 X0(this Vector2 _instance)
        {
            return new Vector3(_instance.x, 0);
        }
        
        public static Vector2 _0Y(this Vector2 _instance)
        {
            return new Vector3(0, _instance.y);
        }
        
        public static Vector3 X0Y(this Vector2 _instance)
        {
            return new Vector3(_instance.x, 0, _instance.y);
        }
        
        public static Vector3 XY0(this Vector2 _instance)
        {
            return new Vector3(_instance.x, _instance.y, 0);
        }
        
        public static Vector3 X00(this Vector3 _instance)
        {
            return new Vector3(_instance.x, 0, 0);
        }
        
        public static Vector3 X00(this Vector2 _instance)
        {
            return new Vector3(_instance.x, 0, 0);
        }
        
        public static Vector3 _00Z(this Vector3 _instance)
        {
            return new Vector3(0, 0, _instance.z);
        }
        
        public static Vector3 _00Y(this Vector2 _instance)
        {
            return new Vector3(0, 0, _instance.y);
        }
        
        public static Vector3 X1Y(this Vector2 _instance)
        {
            return new Vector3(_instance.x, 1, _instance.y);
        }
        
        
        public static Vector2 XZ(this Vector3 _instance)
        {
            return new Vector2(_instance.x,  _instance.z);
        }
        
        public static Vector2 XY(this Vector3 _instance)
        {
            return new Vector2(_instance.x,  _instance.y);
        }

        public static Vector2Int ToInt(this Vector2 _instance)
        {
            return new Vector2Int((int) _instance.x, (int) _instance.y);
        }
        
        public static Vector2 ToFloat(this Vector2Int _instance)
        {
            return new Vector2(_instance.x, _instance.y);
        }

        public static Vector3 MemberWiseMul(this Vector3 _instance, Vector3 _element)
        {
            return new Vector3(_instance.x * _element.x, _instance.y * _element.y, _instance.z * _element.z);
        }

        public static Vector2Int SnappedXZ(this Vector3 _instance)
        {
            return new Vector2Int(Mathf.RoundToInt(_instance.x), Mathf.RoundToInt(_instance.z));
        }

        public static Vector2 Abs(this Vector2 _instance)
        {
            return new Vector2(Mathf.Abs(_instance.x), Mathf.Abs(_instance.y));
        }
        
        public static Vector2 Max(this Vector2 _instance, float _other)
        {
            return new Vector2(
                Mathf.Max(_instance.x, _other), 
                Mathf.Max(_instance.y, _other)
                );
        }

        public static Vector2 RotateByDegrees(this Vector2 _instance, float _degrees)
        {
            float radians = _degrees * Mathf.Deg2Rad;
            Vector2 rotation = new Vector2(Mathf.Cos(radians), Mathf.Sin(radians));
            return new Vector2(
                _instance.x * rotation.x + _instance.y * rotation.y,
                -_instance.x * rotation.y + _instance.y * rotation.x
            );
        }
        
        public static Vector2Int Snapped(this Vector2 _instance)
        {
            return new Vector2Int(Mathf.RoundToInt(_instance.x), Mathf.RoundToInt(_instance.y));
        }

        public static Vector2Int Clamp(this Vector2Int _instance, Vector2Int _min, Vector2Int _max)
        {
            return new Vector2Int(
                Mathf.Clamp(_instance.x, _min.x, _max.x),
                Mathf.Clamp(_instance.y, _min.y, _max.y)
            );
        }

        public static bool Contains(this Vector2Int _dimension, Vector2Int _other)
        {
            return _other.x >= 0 && _other.x < _dimension.x && _other.y >= 0 && _other.y < _dimension.y;
        }
        
        public static bool Contains(this (Vector2Int startInclusive, Vector2Int endExclusive) _range, Vector2Int _other)
        {
            return _other.x >= _range.startInclusive.x && _other.x < _range.endExclusive.x && 
                   _other.y >= _range.startInclusive.y && _other.y < _range.endExclusive.y;
        }

        public static Vector2 Polar(this Vector2 _instance)
        {
            var angle = Mathf.Atan2(_instance.y, _instance.x);
            if (angle < 0)
            {
                angle = 2 * Mathf.PI + angle;
            }
            return new Vector2(angle, _instance.magnitude);
        }

        public static Vector2 Cartesian(this Vector2 _instance)
        {
            return _instance.y * new Vector2(Mathf.Cos(_instance.x), Mathf.Sin(_instance.x));
        }

        public static Vector2 Slerp(Vector2 _a, Vector2 _b, float _t)
        {
            if (_a == Vector2.zero || _b == Vector2.zero)
            {
                return Vector2.LerpUnclamped(_a, _b, _t);
            }
            
            var polarA = _a.Polar();
            var polarB = _b.Polar();
            if (Mathf.Abs(polarB.x - polarA.x) > Mathf.PI)
            {
                if (polarA.x < polarB.x)
                {
                    polarA.x += 2 * Mathf.PI;
                }
                else
                {
                    polarB.x += 2 * Mathf.PI;
                }
            }
            
            var slerpValue = Vector2.LerpUnclamped(polarA, polarB, _t);
            return slerpValue.Cartesian();
        }

        public static Vector2 ClampAngle(this Vector2 _instance, float min, float max)
        {
            Vector2 polarDirection = _instance.Polar();
            polarDirection.x = Mathf.Clamp(polarDirection.x * Mathf.Rad2Deg, min, max);
            polarDirection.x *= Mathf.Deg2Rad;
            return polarDirection.Cartesian();
        }
        
        public static Vector2 ClampAngleMagnitude(this Vector2 _instance, float min, float max, float maxLength)
        {
            Vector2 polarDirection = _instance.Polar();
            polarDirection.y = Mathf.Min(polarDirection.y, maxLength);
            polarDirection.x = Mathf.Clamp(polarDirection.x * Mathf.Rad2Deg, min, max);
            polarDirection.x *= Mathf.Deg2Rad;
            return polarDirection.Cartesian();
        }

        public static Vector2 Saturate(this Vector2 _instance)
        {
            return new Vector2(Mathf.Clamp01(_instance.x), Mathf.Clamp01(_instance.y));
        }
    }
}
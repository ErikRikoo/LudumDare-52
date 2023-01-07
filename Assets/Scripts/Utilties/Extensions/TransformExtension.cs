using UnityEngine;
using UnityEngine.Assertions;

namespace Utilities
{
    public static class TransformExtension
    {
        public static Transform LastChild(this Transform _instance)
        {
            Transform ret = _instance;
            while (ret.childCount != 0)
            {
                Assert.AreEqual(ret.childCount, 1, "Last Child can only be called with string chain of transform");
                ret = ret.GetChild(0);
            }

            return ret;
        }

        public static void MakeZLookAtDirection(this Transform _instance, Vector3 _direction)
        {
            var rot = _instance.rotation.eulerAngles;
            rot.y = (Mathf.Atan2(_direction.y, _direction.x) - 0.5f *  Mathf.PI) * Mathf.Rad2Deg;
            _instance.rotation = Quaternion.Euler(rot);
        }
    }
}
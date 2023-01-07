using System.Collections.Generic;
using UnityEngine;

namespace Utilties.Extensions
{
    public static class ArrayExtension
    {
        public static T RandomItem<T>(this T[] _instance)
        {
            return _instance[Random.Range(0, _instance.Length)];
        }

        public static T RandomItem<T>(this List<T> _instance)
        {
            return _instance[Random.Range(0, _instance.Count)];
        }
    }
}
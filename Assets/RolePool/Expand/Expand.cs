using System;
using System.Collections.Generic;
using System.Linq;
using Random = UnityEngine.Random;

namespace RolePool.Expand
{
    public static class Expand
    {
        public static T GetRandomItem<T>(this IEnumerable<T> enumerable)
        {
            var array = enumerable.ToArray();
            if (enumerable == null || array.Length <= 0) throw new NullReferenceException();

            return array.Length == 1 ? array[0] : array[Random.Range(0, array.Length)];
        }
    }
}
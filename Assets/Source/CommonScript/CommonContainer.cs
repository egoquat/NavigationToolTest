using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

public static class CommonContainer
{
    public static bool IsNullOrEmpty<T>(this IEnumerable<T> container)
    {
        if (null == container)
            return true;
        return container.Count() <= 0;
    }

    public static List<T> ToListOrDefault<T>(this IEnumerable<T> container)
    {
        if (true == container.IsNullOrEmpty())
            return null;
        return container.ToList();
    }

    public static T[] ToArrayOrDefault<T>(this IEnumerable<T> container)
    {
        if (true == container.IsNullOrEmpty())
            return null;
        return container.ToArray();
    }
}

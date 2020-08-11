using System;
using System.Collections.Generic;
using System.Linq;

public static class CommonContainer
{
    public static void Each<T>(this IEnumerable<T> container, Action<T, int> action)
    {
        int i = 0;

        foreach (T x in container)
            action(x, i++);
    }

    public static void Each<T>(this IEnumerable<T> container, Action<T> action)
    {
        foreach (T x in container)
            action(x);
    }

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

    public static bool IsRanged<T>(this IEnumerable<T> container, int idx)
    {
        if (true == container.IsNullOrEmpty())
            return false;
        return idx >= 0 && idx <= container.Count() - 1;
    }

    public static T GetSafe<T>(this IEnumerable<T> container, int idx)
    {
        if (true == container.IsNullOrEmpty() || false == container.IsRanged(idx))
            return default(T);
        return container.ElementAt(idx);
    }
}

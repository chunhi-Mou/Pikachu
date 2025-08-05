using System.Collections.Generic;

// Phương thức Shuffle cho IList<T>
public static class ShuffleUtility
{
    public static void Shuffle<T>(this IList<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = UnityEngine.Random.Range(0, n + 1);
            (list[k], list[n]) = (list[n], list[k]);
        }
    }
}
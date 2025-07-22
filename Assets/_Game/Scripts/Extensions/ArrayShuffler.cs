using System;
using System.Collections.Generic;
using UnityEngine;

public static class ArrayShuffler
{
    public static void ShuffleExceptFixed<T>(T[,] array, Func<T, bool> isFixed)
    {
        if (array == null) return;

        int rows = array.GetLength(0);
        int cols = array.GetLength(1);

        // Tạo danh sách các vị trí không cố định
        List<(int, int)> unpinned = new List<(int, int)>();
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                if (!isFixed(array[i, j]))
                {
                    unpinned.Add((i, j));
                }
            }
        }
        
        List<T> v = new List<T>(unpinned.Count);
        foreach (var pos in unpinned)
        {
            v.Add(array[pos.Item1, pos.Item2]);
        }
        
        v.Shuffle();//Xáo trộn danh sách giá trị
        
        //Gán lại giá trị đã xáo trộn vào các vị trí không cố định
        int index = 0;
        foreach (var pos in unpinned)
        {
            array[pos.Item1, pos.Item2] = v[index];
            index++;
        }
    }
}

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
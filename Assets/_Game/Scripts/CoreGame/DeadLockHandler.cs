using System;
using System.Collections.Generic;
using UnityEngine;

public class DeadLockHandler : MonoBehaviour
{
    public bool CheckAndHandleDeadlock(Dictionary<TileType, List<Vector2Int>> tileTypeDic, Func<Vector2Int, Vector2Int, bool> pathChecker)
    {
        for (int i = 0; i < 100; i++) // Xu li 100 lan
        {
            if (!IsDeadlocked(tileTypeDic, pathChecker))
            {
                    return false; // khong Deadlock -> Dung
            }
            Debug.Log("Deadlock Detected!");
            GameEvents.OnDeadlockDetected?.Invoke();//Xu li
        }
        return true;//TODO: Method2
    }
    
    private bool IsDeadlocked(Dictionary<TileType, List<Vector2Int>> tileTypeDic, Func<Vector2Int, Vector2Int, bool> pathChecker)
    {
        if (tileTypeDic == null) return true;

        foreach (var tileList in tileTypeDic.Values)
        {
            if (tileList.Count < 2) continue;

            for (int i = 0; i < tileList.Count; i++)
            {
                for (int j = i + 1; j < tileList.Count; j++)
                {
                    if (pathChecker(tileList[i], tileList[j]))
                    {
                        return false;
                    }
                }
            }
        }
        
        return true;
    }
}
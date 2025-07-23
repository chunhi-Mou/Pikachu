using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UIElements;

public class BoosterController : MonoBehaviour
{
    public void ShuffleButtonClick()
    {
        if (!GameManager.Instance.UseShuffle()) return; 
        SoundManager.Instance.PlayFx(FxID.Shuffle);
        ShuffleTiles();
    }
    public void HintButtonClick()
    {
        if (!GameManager.Instance.UseHint()) return;
        SoundManager.Instance.PlayFx(FxID.Hint);
        FindHint();
    }
    private void FindHint()
    {
        Dictionary<TileType, List<Vector2Int>> tileDict = TileManager.Instance.TileTypeDic;
        GameTile[,] tiles = TileManager.Instance.Tiles;
        var keys = new List<TileType>(tileDict.Keys);
        keys.Shuffle();
        foreach(var key in keys) {
            List<Vector2Int> hintPos = tileDict[key];
            for (int i = 0; i < hintPos.Count; i++)
            {
                for (int j = i + 1; j < hintPos.Count; j++)
                {
                    Vector2Int posA = hintPos[i];
                    Vector2Int posB = hintPos[j];
                    
                    if (TileManager.Instance.CheckPathExits(posA, posB))
                    {
                        TileManager.Instance.ForceMatchByBooster(tiles[posA.y, posA.x], tiles[posB.y, posB.x]);
                        return;
                    }
                }
            }
        }
        TileManager.Instance.CheckDeadLock();
    }

    private void ShuffleTiles()
    {
        GameTile[,] shuffledTiles = TileManager.Instance.Tiles;
        List<TileType> activePos = new List<TileType>();
        int width = shuffledTiles.GetLength(0);
        int height = shuffledTiles.GetLength(1);

        // Chọn ra những Tiles được Shuffle
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (shuffledTiles[i, j] != null 
                    && (int)shuffledTiles[i, j].TileType < (int)TileType.Obstacles) // Không xào vật cản
                {
                    activePos.Add(shuffledTiles[i, j].TileType);
                }
            }
        }
        
        activePos.Shuffle();
        
        //Update lại Tiles chính
        int visitedIdx = 0;
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (shuffledTiles[i, j] != null 
                    && (int)shuffledTiles[i, j].TileType < (int)TileType.Obstacles) // Không xào Vật cản
                {
                    shuffledTiles[i, j].SetCellType((int)activePos[visitedIdx++]);
                }
            }
        }
        
        TileManager.Instance.SetTilesData(shuffledTiles);
        TileManager.Instance.CheckDeadLock();
    }
}

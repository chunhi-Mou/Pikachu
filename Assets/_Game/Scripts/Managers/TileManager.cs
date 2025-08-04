using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineVisual))]
[RequireComponent(typeof(DeadLockHandler))]
public class TileManager : Singleton<TileManager>
{
    [SerializeField] LineVisual lineVisual;
    [SerializeField] DeadLockHandler deadLockHandler;
    
    private int cols;
    private int rows;
    private GameTile[,] tiles;
    private Dictionary<TileType, List<Vector2Int>> tileTypeDic;
    
    private List<Vector2Int> path;
    private void OnEnable()
    {
        GameEvents.OnValidPairClicked += ProcessMatch;
        GameEvents.OnDeadlockDetected += ShuffleTiles;
    }

    void OnDisable()
    {
        GameEvents.OnValidPairClicked -= ProcessMatch;
        GameEvents.OnDeadlockDetected -= ShuffleTiles;
    }
    private void Awake()
    {
        tileTypeDic = new Dictionary<TileType, List<Vector2Int>>();
        path = new List<Vector2Int>();
    }
    public GameTile GetGameplayTileAt(Vector2Int pos) {
        GameTile tile = tiles[pos.y, pos.x];
        if (tile != null && tile.TileType >= TileType.Obstacles)
        {
            return null;
        }
        return tiles[pos.y, pos.x];
    }
    public bool CheckPathExits(Vector2Int startPos, Vector2Int endPos)
    {
        path = BFSUtils.FindPathPikachu(startPos, endPos, 2, IsWalkable, IsInBounds);
        return path != null && path.Count > 0;
    }
    public void SetTilesMatrix(GameTile[,] newTiles)
    {
        rows = newTiles.GetLength(0);
        cols = newTiles.GetLength(1);
        tiles = newTiles;

        UpdateTileTypeDictionary(tiles);
    }
    public void UpdateTileTypeDictionary(GameTile[,] sourceTiles)
    {
        tileTypeDic.Clear();

        for (int i = 1; i < sourceTiles.GetLength(0) - 1; i++)
        {
            for (int j = 1; j < sourceTiles.GetLength(1) - 1; j++)
            {
                GameTile tile = sourceTiles[i, j];
                if (tile == null) continue;
                
                if ((int)tile.TileType < (int)TileType.Obstacles)
                {
                    Vector2Int tilePos = new Vector2Int(j, i);

                    if (!tileTypeDic.ContainsKey(tile.TileType))
                        tileTypeDic[tile.TileType] = new List<Vector2Int>();

                    tileTypeDic[tile.TileType].Add(tilePos);
                }
            }
        }
    }

    #region Booster Logic
    public void FindHintPair()
    {
        var keys = new List<TileType>(tileTypeDic.Keys);
        keys.Shuffle();
        foreach(var key in keys) {
            List<Vector2Int> hintPos = tileTypeDic[key];
            for (int i = 0; i < hintPos.Count; i++)
            {
                for (int j = i + 1; j < hintPos.Count; j++)
                {
                    Vector2Int posA = hintPos[i];
                    Vector2Int posB = hintPos[j];
                    
                    if (CheckPathExits(posA, posB))
                    {
                        ProcessMatch(tiles[posA.y, posA.x], tiles[posB.y, posB.x]);
                        return;
                    }
                }
            }
        }
        CheckDeadLock();
    }
    public void ShuffleTiles()
    {
        List<TileType> activePos = new List<TileType>();
        GameTile[,] shuffleTiles = tiles;

        // Chọn ra những Tiles được Shuffle
        for (int i = 1; i < rows-1; i++)
        {
            for (int j = 1; j < cols-1; j++)
            {
                if (shuffleTiles[i, j] != null 
                    && (int)shuffleTiles[i, j].TileType < (int)TileType.Obstacles) // Không xào vật cản
                {
                    activePos.Add(shuffleTiles[i, j].TileType);
                }
            }
        }
        
        activePos.Shuffle();
        
        //Update lại Tiles chính
        int visitedIdx = 0;
        for (int i = 1; i < rows-1; i++)
        {
            for (int j = 1; j < cols-1; j++)
            {
                if (tiles[i, j] != null && (int)tiles[i, j].TileType < (int)TileType.Obstacles) // Không xào Vật cản
                {
                    shuffleTiles[i, j].SetCellType((int)activePos[visitedIdx++]);
                }
            }
        }
        
        SetTilesMatrix(shuffleTiles);
        CheckDeadLock();
    }
    private void CheckDeadLock()
    {
        deadLockHandler.CheckAndHandleDeadlock(tileTypeDic, CheckPathExits);
    }
    #endregion
    
    #region Match Logic
    private void ProcessMatch(GameTile tileA, GameTile tileB)
    {
        if (CheckPathExits(tileA.Position, tileB.Position))
        {
            GameEvents.OnTilesMatched?.Invoke();
            
            lineVisual.DrawPath(path); // VẼ đường
            
            HandleTileMatched(tileA);
            HandleTileMatched(tileB);
            if (IsWin())
            {
                StartCoroutine(WinRoutine());
            }
        }
    }
    private void HandleTileMatched(GameTile tile) // Xử logic Match Tile ĐƠN LẺ
    {
        tile.HandleMatch();
        tileTypeDic[tile.TileType].Remove(tile.Position);
        tiles[tile.Position.y, tile.Position.x] = null;
        if (tileTypeDic[tile.TileType].Count == 0) // Không còn cặp nào
        {
            tileTypeDic.Remove(tile.TileType); // Loại bỏ key
        }
    }
    # endregion
    
    #region WinLogic
    private bool IsWin()
    {
        return (tileTypeDic == null || tileTypeDic.Count == 0);
    }
    private IEnumerator WinRoutine()
    {
        yield return new WaitForSeconds(0.5f); 
        GameManager.Instance.EndGame(true);
    }
    #endregion
    
    #region Hàm hỗ trợ BFS
    private bool IsWalkable(Vector2Int pos)
    {
        return tiles[pos.y, pos.x] == null;
    }
    private bool IsInBounds(Vector2Int pos)
    {
        return pos.x >= 0 && pos.x < cols &&
               pos.y >= 0 && pos.y < rows;
    }
    #endregion
}

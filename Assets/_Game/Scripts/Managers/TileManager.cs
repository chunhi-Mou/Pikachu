using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineVisual))]
[RequireComponent(typeof(DeadLockHandler))]
public class TileManager : Singleton<TileManager>
{
    [SerializeField] LineVisual lineVisual;
    [SerializeField] DeadLockHandler deadLockHandler;
    
    private int width;
    private int height;
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
    
    public void SetTilesData(GameTile[,] newTiles) // Nhận data đã khởi tạo
    {
        height = newTiles.GetLength(0);
        width = newTiles.GetLength(1);
        tiles = newTiles;
        
        //UPDATE TILE DIC
        tileTypeDic.Clear();
        for (int i = 1; i < height - 1; i++)
        {
            for (int j = 1; j < width - 1; j++)
            {
                GameTile tile = newTiles[i, j];
                if (tile != null)
                {
                    tile.SetCellType((int)tile.TileType);
                    Vector2Int tilePos = new Vector2Int(j, i);

                    if ((int)tile.TileType < (int)TileType.Obstacles) {
                        if (tileTypeDic.ContainsKey(newTiles[i, j].TileType))
                        {
                            tileTypeDic[newTiles[i, j].TileType].Add(tilePos);
                        }
                        else
                        {
                            tileTypeDic[newTiles[i, j].TileType] = new List<Vector2Int>() { tilePos };
                        }
                    }
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
        // Chọn ra những Tiles được Shuffle
        for (int i = 1; i < width-1; i++)
        {
            for (int j = 1; j < height-1; j++)
            {
                if (tiles[i, j] != null && (int)tiles[i, j].TileType < (int)TileType.Obstacles) // Không xào vật cản
                {
                    activePos.Add(tiles[i, j].TileType);
                }
            }
        }
        
        activePos.Shuffle();
        
        //Update lại Tiles chính
        int visitedIdx = 0;
        for (int i = 1; i < width-1; i++)
        {
            for (int j = 1; j < height-1; j++)
            {
                if (tiles[i, j] != null && (int)tiles[i, j].TileType < (int)TileType.Obstacles) // Không xào Vật cản
                {
                    tiles[i, j].SetCellType((int)activePos[visitedIdx++]);
                }
            }
        }
        
        SetTilesData(tiles);
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
        return pos.x >= 0 && pos.x < width &&
               pos.y >= 0 && pos.y < height;
    }
    #endregion
}

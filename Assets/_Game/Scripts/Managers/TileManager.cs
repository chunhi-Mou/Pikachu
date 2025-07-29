using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineVisual))]
[RequireComponent(typeof(DeadLockHandler))]
public class TileManager : Singleton<TileManager>
{
    [SerializeField] LineVisual lineVisual;
    [SerializeField] DeadLockHandler deadLockHandler;
    [SerializeField] private int addedScore = 5;
    
    private int width;
    private int height;
    public GameTile[,] Tiles { get; private set; }
    public Dictionary<TileType, List<Vector2Int>> TileTypeDic { get; private set; }
    
    private List<Vector2Int> path;
    private void OnEnable()
    {
        GameEvents.OnValidPairClicked += ProcessMatch;
        GameEvents.OnTriggerDeadlockCheck += CheckDeadLock;
    }

    void OnDisable()
    {
        GameEvents.OnValidPairClicked -= ProcessMatch;
        GameEvents.OnTriggerDeadlockCheck -= CheckDeadLock;
    }
    private void Awake()
    {
        TileTypeDic = new Dictionary<TileType, List<Vector2Int>>();
        path = new List<Vector2Int>();
    }
    public GameTile GetGameplayTileAt(Vector2Int pos) {
        GameTile tile = Tiles[pos.y, pos.x];
        if (tile != null && tile.TileType >= TileType.Obstacles)
        {
            return null;
        }
        return Tiles[pos.y, pos.x];
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
        Tiles = newTiles;
        
        TileTypeDic.Clear();
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
                        if (TileTypeDic.ContainsKey(newTiles[i, j].TileType))
                        {
                            TileTypeDic[newTiles[i, j].TileType].Add(tilePos);
                        }
                        else
                        {
                            TileTypeDic[newTiles[i, j].TileType] = new List<Vector2Int>() { tilePos };
                        }
                    }
                }
            }
        }
    }
    
    private void CheckDeadLock()
    {
        deadLockHandler.CheckAndHandleDeadlock(TileTypeDic, CheckPathExits);
    }
    
    #region Match Logic
    private void ProcessMatch(GameTile tileA, GameTile tileB)
    {
        if (CheckPathExits(tileA.Position, tileB.Position))
        {
            lineVisual.DrawPath(path); // VẼ đường
            
            HandleTileMatched(tileA);
            HandleTileMatched(tileB);
            GameManager.Instance.AddScore(addedScore); //TODO: chuyển thành Gửi TBao
            if (IsWin())
            {
                StartCoroutine(WinRoutine());
            }
        }
    }
    private void HandleTileMatched(GameTile tile) // Xử logic Match Tile ĐƠN LẺ
    {
        tile.HandleMatch();
        TileTypeDic[tile.TileType].Remove(tile.Position);
        Tiles[tile.Position.y, tile.Position.x] = null;
        if (TileTypeDic[tile.TileType].Count == 0) // Không còn cặp nào
        {
            TileTypeDic.Remove(tile.TileType); // Loại bỏ key
        }
    }
    # endregion
    
    #region WinLogic
    private bool IsWin()
    {
        return (TileTypeDic == null || TileTypeDic.Count == 0);
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
        return Tiles[pos.y, pos.x] == null;
    }
    private bool IsInBounds(Vector2Int pos)
    {
        return pos.x >= 0 && pos.x < width &&
               pos.y >= 0 && pos.y < height;
    }
    #endregion
}

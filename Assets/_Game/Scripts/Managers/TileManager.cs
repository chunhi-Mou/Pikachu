using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(TileVisualController))]
[RequireComponent(typeof(DeadLockHandler))]
public class TileManager : Singleton<TileManager>
{
    [SerializeField] TileVisualController tileVisualController;
    [SerializeField] DeadLockHandler deadLockHandler;
    [SerializeField] private int addedScore = 5;
    private int width;
    private int height;
    private Vector2 cellSize;
    private Vector3 origin;
    
    public GameTile[,] Tiles { get; private set; }
    private GameTile tile1;
    private GameTile tile2;
    private List<Vector2Int> path;
    public Dictionary<TileType, List<Vector2Int>> TileTypeDic { get; private set; }

    private void Awake()
    {
        OnInit();
    }
    private void OnDestroy()
    {
        OnDespawn();
    }
    private void OnInit() // Hàm khởi tạo
    {
        GameEvents.OnTileClicked += OnTileClicked;
        TileTypeDic = new Dictionary<TileType, List<Vector2Int>>();
        path = new List<Vector2Int>();
        tile1 = null; tile2 = null;
        if (tileVisualController == null)
        {
            tileVisualController.GetComponent<TileVisualController>();
        }

        if (deadLockHandler == null)
        {
            deadLockHandler.GetComponent<DeadLockHandler>();
        } 
    }
    private void OnDespawn()
    {
        GameEvents.OnTileClicked -= OnTileClicked;
    }
    public void ForceMatchByBooster(GameTile tileA, GameTile tileB)
    {
        ProcessMatch(tileA, tileB);
    }
    public bool CheckPathExits(Vector2Int startPos, Vector2Int endPos)
    {
        path = BFSUtils.FindPathPikachu(startPos, endPos, 2, IsWalkable, IsInBounds);
        return path != null && path.Count > 0;
    }
    public void CheckDeadLock()
    {
        deadLockHandler.CheckAndHandleDeadlock(TileTypeDic, CheckPathExits);
    }
    public void SetTilesData(GameTile[,] newTiles)
    {
        cellSize = LevelManager.Instance.cellSize;
        origin = LevelManager.Instance.origin;
        height = newTiles.GetLength(0);
        width = newTiles.GetLength(1);
        Tiles = newTiles;
        
        TileTypeDic.Clear();
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                if (newTiles[i, j] != null) {
                    //Update TILE VISUAL
                    newTiles[i, j].SetCellType((int)newTiles[i, j].TileType);

                    if ((int)newTiles[i, j].TileType >= (int)TileType.Obstacles) // Không theo dõi Vật Cản
                    {
                        continue;
                    }
                    // Update TILES DICT
                    Vector2Int tilePos = new Vector2Int(j, i);
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

    private void OnTileClicked(Vector2 worldPosition)
    {
        Vector2Int cellPos = GridUtils.WorldToGrid(cellSize, origin, worldPosition);
        if (!IsInBounds(cellPos))
        {
            return;
        }

        GameTile clickedTile = Tiles[cellPos.y+1, cellPos.x+1];
 
        if (clickedTile == null || clickedTile == tile1) return;

        if (tile1 == null)
        {
            tile1 = clickedTile;
            tile1.HandleSelected();
        }
        else // tile2 is guaranteed to be null here if tile1 is not
        {
            tile2 = clickedTile;
            tile2.HandleSelected();

            if (TilesAreMatchable(tile1, tile2))
            {
                CheckWhenFoundMatched(); // ProcessMatch và reset
            }
            else
            {
                // Không hợp lệ, hủy chọn tile1, và coi tile2 là tile1 mới
                tile1.HandleDeSelected();
                tile1 = tile2;
                tile2 = null;
                CheckDeadLock();
            }
        }
    }
    private bool TilesAreMatchable(GameTile tileA, GameTile tileB)
    {
        if (tileA.TileType != tileB.TileType) return false;
    
        return CheckPathExits(tileA.Position, tileB.Position);
    }
    private void ProcessMatch(GameTile tileA, GameTile tileB)
    {
        if (CheckPathExits(tileA.Position, tileB.Position))
        {

            tileVisualController.DrawPath(path);
            
            HandleTileMatched(tileA);
            HandleTileMatched(tileB);
        }
        GameManager.Instance.AddScore(addedScore);
    }
    private void CheckWhenFoundMatched()
    {
        ProcessMatch(tile1, tile2);
        
        tile1 = null;
        tile2 = null;

        if (IsWin())
        {
            StartCoroutine(WinRoutine());
        }
    }
    private IEnumerator WinRoutine()
    {
        yield return new WaitForSeconds(0.5f); 
        GameManager.Instance.EndGame(true);
    }
    private void HandleTileMatched(GameTile tile)
    {
        tile.HandleMatch();
        TileTypeDic[tile.TileType].Remove(tile.Position);
        Tiles[tile.Position.y, tile.Position.x] = null;
        if (TileTypeDic[tile.TileType].Count == 0) // Không còn cặp nào
        {
            TileTypeDic.Remove(tile.TileType); // Loại bỏ key
        }
    }
    
    private bool IsWin()
    {
        return (TileTypeDic == null || TileTypeDic.Count == 0);
    }
    private bool IsWalkable(Vector2Int pos)
    {
        return Tiles[pos.y, pos.x] == null;
    }
    private bool IsInBounds(Vector2Int pos)
    {
        return pos.x >= 0 && pos.x < width &&
               pos.y >= 0 && pos.y < height;
    }
}

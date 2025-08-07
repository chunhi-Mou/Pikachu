using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineVisual))]
public class TileManager : Singleton<TileManager>
{
    [SerializeField] LineVisual lineVisual;
    
    private int cols;
    private int rows;
    private GameTile[,] tiles;
    private List<Vector2Int> path;
    private Dictionary<TileType, List<Vector2Int>> tileTypeDic;
    [SerializeField] private float hintDelay = 5f;
    private float idleTimer;
    private bool canCheckIdleHint = true;
    private GameTile cacheHint1;
    private GameTile cacheHint2;
    
    private void OnEnable()
    {
        GameEvents.OnValidPairClicked += ProcessMatch;
        GameEvents.OnUserClicked += ClearHint;
    }

    private void OnDisable()
    {
        GameEvents.OnValidPairClicked -= ProcessMatch;
        GameEvents.OnUserClicked -= ClearHint;
    }
    private void Awake()
    {
        tileTypeDic = new Dictionary<TileType, List<Vector2Int>>();
        path = new List<Vector2Int>();
    }
    private void Update()
    {
        if (!GameManager.IsState(GameState.GamePlay))
        {
            idleTimer = 0f;
            return;
        }

        if (canCheckIdleHint)
        {
            idleTimer += Time.deltaTime;
            if (idleTimer >= hintDelay)
            {
                idleTimer = 0f;
                ShowHintIfAvailable();
            }
        }
    }

    public GameTile GetGameplayTileAt(Vector2Int pos) {
        GameTile tile = tiles[pos.y, pos.x];
        if (tile != null && tile.IsObstacle())
        {
            return null;
        }
        return tiles[pos.y, pos.x];
    }
    
    public GameTile FindObstacle() // Tìm VẬT CẢN mà gây ra Deadlock
    {
        // Lấy danh sách tất cả các ô vật cản
        List<GameTile> obstacles = new List<GameTile>();
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                if (tiles[i, j] != null && tiles[i, j].IsObstacle() && tiles[i, j].gameObject.activeSelf)
                {
                    obstacles.Add(tiles[i, j]);
                }
            }
        }
        
        // Duyệt qua từng vật cản để thử loại bỏ
        foreach (GameTile obstacle in obstacles)
        {
            Vector2Int obstaclePos = obstacle.Position;
            tiles[obstaclePos.y, obstaclePos.x] = null;
            if (!IsDeadlocked(this.tileTypeDic)) // Nếu không còn deadlock
            {
                tiles[obstaclePos.y, obstaclePos.x] = null;
                return obstacle; // CẦN TÌM
            }
            tiles[obstaclePos.y, obstaclePos.x] = obstacle;
        }
        return null;
    }
    void HandleDeadlock()
    {
        for (int i = 0; i < 100; i++) // Xu li 100 lan
        {
            if (!IsDeadlocked(this.tileTypeDic))
            {
                return; // khong Deadlock -> Dung
            }
        }
        GameTile deadlockObstacle =  FindObstacle();
        if (deadlockObstacle != null && deadlockObstacle.transform != null)
        {
            GameEvents.OnFoundDeadlockObs?.Invoke(deadlockObstacle.transform);
            deadlockObstacle.Match();
        }
        Debug.Log("Deadlock Detected!");
    }
    
    private bool IsDeadlocked(Dictionary<TileType, List<Vector2Int>> tileTypeDic)
    {
        if (tileTypeDic == null) return true;
        foreach (var tileList in tileTypeDic.Values)
        {
            if (tileList.Count < 2) continue;

            for (int i = 0; i < tileList.Count; i++)
            {
                for (int j = i + 1; j < tileList.Count; j++)
                {
                    if (CheckPathExits(tileList[i], tileList[j]))
                    {
                        return false;
                    }
                }
            }
        }
        return true;
    }
    private bool CheckPathExits(Vector2Int startPos, Vector2Int endPos)
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

    private void UpdateTileTypeDictionary(GameTile[,] sourceTiles)
    {
        tileTypeDic.Clear();

        for (int i = 1; i < sourceTiles.GetLength(0) - 1; i++)
        {
            for (int j = 1; j < sourceTiles.GetLength(1) - 1; j++)
            {
                GameTile tile = sourceTiles[i, j];
                if (tile == null) continue;
                
                if (!tile.IsObstacle())
                {
                    Vector2Int tilePos = new Vector2Int(j, i);

                    if (!tileTypeDic.ContainsKey(tile.TileType))
                    {
                        tileTypeDic[tile.TileType] = new List<Vector2Int>();
                    }
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
                        ClearHint();
                        ProcessMatch(tiles[posA.y, posA.x], tiles[posB.y, posB.x]);
                        return;
                    }
                }
            }
        }
        HandleDeadlock();
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
                if (shuffleTiles[i, j] != null && !shuffleTiles[i, j].IsObstacle()) // Không xào vật cản
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
                if (tiles[i, j] != null && !tiles[i, j].IsObstacle()) // Không xào Vật cản
                {
                    shuffleTiles[i, j].SetCellType((int)activePos[visitedIdx++]);
                }
            }
        }
        
        SetTilesMatrix(shuffleTiles);
        ClearHint();
        HandleDeadlock();
    }
    private void ShowHintIfAvailable()
    {
        if (cacheHint1 != null || cacheHint2 != null)
            return;
        var keys = new List<TileType>(tileTypeDic.Keys);
        keys.Shuffle();

        foreach (var key in keys)
        {
            List<Vector2Int> hintPos = tileTypeDic[key];
            for (int i = 0; i < hintPos.Count; i++)
            {
                for (int j = i + 1; j < hintPos.Count; j++)
                {
                    Vector2Int posA = hintPos[i];
                    Vector2Int posB = hintPos[j];

                    if (CheckPathExits(posA, posB))
                    {
                        var tile1 = tiles[posA.y, posA.x];
                        var tile2 = tiles[posB.y, posB.x];

                        if (tile1 != null && tile2 != null)
                        {
                            cacheHint1 = tile1;
                            cacheHint2 = tile2;

                            cacheHint1.Hint();
                            cacheHint2.Hint();
                        }
                        return;
                    }
                }
            }
        }

        // Nếu không tìm thấy cặp -> deadlock
        HandleDeadlock();
    }
    private void ClearHint()
    {
        idleTimer = 0f;
        if (cacheHint1 != null)
        {
            cacheHint1.StopHint();
            cacheHint1 = null;
        }
        if (cacheHint2 != null)
        {
            cacheHint2.StopHint();
            cacheHint2 = null;
        }
    }
    #endregion
    
    #region Match Logic
    private void ProcessMatch(GameTile tileA, GameTile tileB)
    {
        ClearHint();
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
        else
        {
            HandleDeadlock();
        }
    }
    private void HandleTileMatched(GameTile tile) // Xử logic Match Tile ĐƠN LẺ
    {
        tile.Match(); // Gọi để tile tự xử lí khi match
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
    
    #region Helper
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

using System;
using UnityEngine;
using UnityEngine.Serialization;

public class LevelManager : Singleton<LevelManager>
{
    [Header("Level Data")]
    [SerializeField] private float levelTime = 90f;
    public float LevelTime => levelTime;
    [SerializeField] private int addedScore = 5;
    [SerializeField] private Vector2 cellSize = Vector2.one;
    
    [Header("Preferences")]
    [SerializeField] private GameTile gameTilePrefab;
    [SerializeField] private Timer timer;
    [SerializeField] private Score score;
    
    public Score Score => score;
    public Timer Timer => timer;


    public Vector2 CellSize => cellSize;
    private readonly string levelName = GameCONST.PRE_LEVEL_NAME;

    private int rows;
    private int cols;
    private int[,] matrix;
    private GameTile[,] tiles;

    private int currentLevel = 1;

    public Vector3 Origin => GridUtils.CalOrigin(new Vector2Int(cols, rows), cellSize);

    private void Awake()
    {
        currentLevel = DataManager.Instance.GetCurLevel();
    }
    private void OnEnable()
    {
        GameEvents.OnTilesMatched += AddScoreOnMatch;
    }

    private void OnDisable()
    {
        GameEvents.OnTilesMatched -= AddScoreOnMatch;
    }

    private void AddScoreOnMatch()
    {
        score.AddScore(addedScore);
    }
    
    #region Level Logic
    public void ResetPlayedLevel()
    {
        DataManager.Instance.UpdatePlayedLevel(1);
    }
    public void PreLoadLevel()
    {
        ClearGrid();
        timer.OnInit(levelTime);
        score.OnInit(0);
        currentLevel = DataManager.Instance.GetCurLevel();
    }
    public void OnLoadLevel()
    {
        Data<int> data = JsonUtils.Load<int>(levelName + currentLevel); // Load Data tu Json
        matrix = GridData<int>.ConvertGridDataTo2DArray(data.grid); //TODO: code chuyen thang ve Matrix khi load
        GenerateGrid(matrix); 
    }
    public void OnNextLevel()
    {
        currentLevel++;
        DataManager.Instance.UpdatePlayedLevel(currentLevel);
    }
    #endregion
    
    #region Grid Handler
    private void GenerateGrid(int[,] gridData)
    {
        int rawRows = gridData.GetLength(0);
        int rawCols = gridData.GetLength(1);

        rows = rawRows + 2; // THÊM PADDING
        cols = rawCols + 2;

        // Khởi tạo mảng có padding
        tiles = new GameTile[rows, cols];
        matrix = new int[rows, cols];

        for (int i = 0; i < rawRows; i++)
        {
            for (int j = 0; j < rawCols; j++)
            {
                // Dữ liệu từ JSON nằm trong phần lõi (không bao gồm padding)
                int paddedI = i + 1;
                int paddedJ = j + 1;

                matrix[paddedI, paddedJ] = gridData[i, j];

                if (gridData[i, j] > 0)
                {
                    Vector2Int gridPos = new Vector2Int(paddedJ, paddedI); // (x, y)
                    Vector3 worldPos = GridUtils.GridToWorld(cellSize, Origin, gridPos);

                    GameTile newTile = Instantiate(gameTilePrefab, worldPos, Quaternion.identity, transform);
                    newTile.SetPosition(gridPos.x, gridPos.y);
                    newTile.SetCellType(gridData[i, j]);

                    tiles[gridPos.y, gridPos.x] = newTile;
                }
            }
        }

        TileManager.Instance.SetTilesData(tiles);
    }
    //Dọn dẹp grid khi qua level
    private void ClearGrid()
    {
        if (tiles == null) return;
        
        for (int i = 0; i < tiles.GetLength(0); i++)
        {
            for (int j = 0; j < tiles.GetLength(1); j++)
            {
                if (tiles[i,j] != null)
                {
                    Destroy(tiles[i,j].gameObject);
                }
            }
        }
        tiles = null;
    }
    #endregion
}

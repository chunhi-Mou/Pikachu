using System;
using UnityEngine;
using UnityEngine.Serialization;

public class LevelManager : Singleton<LevelManager>
{
    [SerializeField] private GameTile gameTilePrefab;
    [SerializeField] private Vector2 cellSize = Vector2.one;
    public Vector2 CellSize => cellSize;
    private readonly string levelName = GameCONST.PRE_LEVEL_NAME;
    
    private int currentLevel = 1;
    private int height;
    private int width;
    private int[,] matrix;
    private GameTile[,] tiles;
    
    public Vector3 Origin => GridUtils.CalOrigin(new Vector2Int(width, height), cellSize);
    
    protected void Awake()
    {
        currentLevel = DataManager.Instance.GetCurLevel();
    }
    public void ResetLevel()
    {
        DataManager.Instance.UpdatePlayedLevel(1);
    }
    public void OnLoadLevel()
    {
        ClearGrid();
        currentLevel = DataManager.Instance.GetCurLevel();
        Data<int> data = JsonUtils.Load<int>(levelName + currentLevel);
        matrix = GridData<int>.ConvertGridDataTo2DArray(data.grid);
        GenerateGrid(matrix); 
    }
    public void OnNextLevel(bool isStartGame)
    {
        currentLevel++;
        DataManager.Instance.UpdatePlayedLevel(currentLevel);
        if (isStartGame)
        {
            GameManager.Instance.StartGame();
        }
    }
    
    #region Grid Handler
    private void GenerateGrid(int[,] gridData)
    {
        int rawHeight = gridData.GetLength(0);
        int rawWidth = gridData.GetLength(1);

        height = rawHeight + 2; // THÊM PADDING
        width = rawWidth + 2;

        // Khởi tạo mảng có padding
        tiles = new GameTile[height, width];
        matrix = new int[height, width];

        for (int i = 0; i < rawHeight; i++)
        {
            for (int j = 0; j < rawWidth; j++)
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

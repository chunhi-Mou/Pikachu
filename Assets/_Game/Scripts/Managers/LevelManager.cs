using System;
using UnityEngine;

public class LevelManager : Singleton<LevelManager>
{
    public Vector2 cellSize;
    [SerializeField] private GameTile gameTilePrefab;
    private int currentLevel = 1;
    private string levelName = GameCONST.PRE_LEVEL_NAME;
    private int[,] matrix;
    private GameTile[,] tiles;
    private int height;
    private int width;
    public Vector3 origin => GridUtils.CalOrigin(new Vector2Int(width, height), cellSize);
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
        currentLevel = DataManager.Instance.GetCurLevel();
        ClearGrid();
        Data<int> data = JsonUtils.Load<int>(levelName + currentLevel.ToString());
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
        // Lấy kích thước từ dữ liệu được truyền vào
        height = gridData.GetLength(0);
        width = gridData.GetLength(1);
        
        tiles = new GameTile[height+2, width+2];
        
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                if (gridData[i, j] > 0) // 0 là Null
                {
                    Vector2Int positionInArray = new Vector2Int(j + 1, i + 1);
                    Vector3 tileWorldPosition = GridUtils.GridToWorld(cellSize, origin, new Vector2Int(j, i));
                    
                    // Tạo tile
                    GameTile newTile = Instantiate(gameTilePrefab, tileWorldPosition, Quaternion.identity, transform);
                    newTile.SetPosition(positionInArray.x, positionInArray.y);
                    newTile.SetCellType(gridData[i, j]);
                    
                    // Save Data
                    tiles[positionInArray.y, positionInArray.x] = newTile;
                  
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

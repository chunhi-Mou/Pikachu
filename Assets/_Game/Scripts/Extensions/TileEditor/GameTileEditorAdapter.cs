using UnityEngine;

[ExecuteInEditMode]
public class GameTileEditorAdapter : MonoBehaviour, ILevelUnit
{
    private GameTile tile;

    private void Awake()
    {
        tile = GetComponent<GameTile>();
        if (tile == null)
        {
            Debug.LogError("Missing GameTile");
        }
    }

    public bool SetCellType(int cellType)
    {
        if (!System.Enum.IsDefined(typeof(TileType), cellType))
            return false;

        tile.SetCellType(cellType);
        return true;
    }
}
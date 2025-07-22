using UnityEngine;

public static class GridUtils
{
    public static Vector3 CalOrigin(Vector2Int gridSize, Vector2 cellSize)
    {
        return new Vector3( -gridSize.x * cellSize.x * 0.5f, -gridSize.y * cellSize.y * 0.5f, 0);
    }

    public static Vector2Int WorldToGrid(Vector2 cellSize, Vector3 origin, Vector3 worldPos)
    {
        Vector3 localPos = worldPos - origin;
        return new Vector2Int(
            Mathf.FloorToInt(localPos.x / cellSize.x),
            Mathf.FloorToInt(localPos.y / cellSize.y)
        );
    }

    public static Vector3 GridToWorld(Vector2 cellSize, Vector3 origin, Vector2Int cellPos)
    {
        return origin + new Vector3((cellPos.x + 0.5f) * cellSize.x, (cellPos.y + 0.5f) * cellSize.y, 0);
    }

}
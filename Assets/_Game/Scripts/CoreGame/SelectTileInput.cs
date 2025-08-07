using System.Collections;
using UnityEngine;

// Nhận Input của Player và Chuyển Sang Vị trí Cell -> Thông báo cho TileManager
public class SelectTileInput : MonoBehaviour
{
    private Camera _camera;
    
    private GameTile tile1;
    private GameTile tile2;

    private void Start()
    {
        _camera = Camera.main;
        tile1 = null;
        tile2 = null;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && GameManager.IsState(GameState.GamePlay)) // Game đang chạy
        {
            Vector2 worldPos = _camera.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(worldPos, Vector2.zero);

            if (hit.collider != null) // Nếu có Collider
            {
                HandleTileClicked(worldPos);
            }
        }
    }
    private void HandleTileClicked (Vector2 worldPosition)
    {
        StartCoroutine(DelayedTrigger(0.2f));
        Vector2 cellSize = LevelManager.Instance.CellSize;
        Vector3 origin = LevelManager.Instance.Origin;
        Vector2Int cellPos = GridUtils.WorldToGrid(cellSize, origin, worldPosition);
        
        GameTile clickedTile = TileManager.Instance.GetGameplayTileAt(cellPos);
 
        if (clickedTile == null || clickedTile == tile1) return;

        if (tile1 == null)
        {
            tile1 = clickedTile;
            tile1.Select();
        }
        else
        {
            tile2 = clickedTile;
            tile2.Select();

            if (tile1.TileType == tile2.TileType)
            {
                GameEvents.OnValidPairClicked?.Invoke(tile1, tile2);
                tile1.Deselect(0.2f);
                tile2.Deselect(0.2f);
                tile1 = null;
                tile2 = null;
            }
            else
            {
                // Không hợp lệ, hủy chọn tile1, và coi tile2 là tile1 mới
                tile1.Deselect(0f);
                tile1 = tile2;
                tile2 = null;
            }
        }
    }

    IEnumerator DelayedTrigger(float delay)
    {
        yield return new WaitForSeconds(delay);
        GameEvents.OnUserClicked?.Invoke();
    }
}
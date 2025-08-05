using System;
using System.Collections;
using Scriptable;
using UnityEngine;
public class GameTile : MonoBehaviour
{
    [Header("Tile Data")]
    [SerializeField] private TilesData tilesData;
    [SerializeField] private GameTileVisual visual;
    
    private TileType tileType;
    public TileType TileType => tileType;
    
    private Vector2Int position;
    public Vector2Int Position => position;
    
    public void OnInit(int gridX, int gridY, TileType initTileType) // Init
    {
        position.x = gridX;
        position.y = gridY;
        tileType = initTileType;
    }
    public void UpdateVisual() // Thuc hien Updata hinh anh
    {
        Sprite sprite = tilesData.GetSprite(tileType);
        visual.InitVisual(tileType, sprite, !IsObstacle());
    }
    public void SetCellType(int cellType) // Thay doi Tile khi trong Gameplay
    {
        tileType = (TileType)cellType;
        StartCoroutine(UpdateVisualWithDelay(0.3f));
    }
    private IEnumerator UpdateVisualWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        visual.ShuffleEffect();
        UpdateVisual();
    }
    public bool IsObstacle()
    {
        return TileType >= TileType.Obstacles;
    }
    
    public void Select()
    {
        SoundManager.Instance.PlayFx(FxID.TileSelect);
        visual.SetSelected(true);
    }
    public void Match()
    {
        visual.PlayMatchEffect(OnDespawn);
    }
    
    public void Deselect(float delay)
    {
        StartCoroutine(DeselectAfterDelay(delay));
    }
    private IEnumerator DeselectAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        visual.SetSelected(false);
    }
    
    private void OnDespawn()
    {
        gameObject.SetActive(false);
    }
}

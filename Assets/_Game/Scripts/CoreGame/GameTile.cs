using System;
using System.Collections;
using Scriptable;
using UnityEngine;
public class GameTile : MonoBehaviour
{
    [SerializeField] private TilesData tilesData;
    [SerializeField] private TileType tileType;
    [SerializeField] private int x, y;
    [SerializeField] private GameTileVisual visual;

    public TileType TileType => tileType;
    public Vector2Int Position => new Vector2Int(x, y);

    public void SetUp(int gridX, int gridY, TileType initTileType) // Init
    {
        x = gridX;
        y = gridY;
        this.tileType = initTileType;
    }
    
    public void ApplyTileVisual() // Thuc hien Updata hinh anh
    {
        Sprite sprite = tilesData.GetSprite(tileType);
        bool isPlayable = (int)tileType > 0 && (int)tileType < (int)TileType.Obstacles;
        visual.InitVisual(tileType, sprite, isPlayable);
    }
    public void SetCellType(int cellType) // Thay doi Tile khi trong Gameplay
    {
        if (!Enum.IsDefined(typeof(TileType), cellType)) return;
        tileType = (TileType)cellType;
        if (tileType == TileType.None)
        {
            OnDespawn();
        }
        else
        {
            StartCoroutine(DelayedTileUpdate(0.3f));
        }
    }
    public void HandleSelected()
    {
        SoundManager.Instance.PlayFx(FxID.TileSelect);
        visual.SetSelected(true);
    }
    public void HandleMatch()
    {
        visual.PlayMatchEffect(OnDespawn);
    }
    
    public void HandleDeSelected(float delay)
    {
        StartCoroutine(DeSelectDelay(delay));
    }
    private void OnDespawn()
    {
        gameObject.SetActive(false);
    }
    private IEnumerator DeSelectDelay (float delay)
    {
        yield return new WaitForSeconds(delay);
        visual.SetSelected(false);
    }
    private IEnumerator DelayedTileUpdate(float delay)
    {
        yield return new WaitForSeconds(delay);
        visual.ShuffleEffect();
        ApplyTileVisual();
    }
}

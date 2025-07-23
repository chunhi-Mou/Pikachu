using System;
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

    private void Start() => Init();

    public void SetPosition(int gridX, int gridY)
    {
        x = gridX;
        y = gridY;
    }

    private void Init()
    {
        Sprite sprite = tilesData.GetSprite(tileType);
        bool isPlayable = (int)tileType > 0 && (int)tileType < (int)TileType.Obstacles;
        visual.InitVisual(tileType, sprite, isPlayable);
    }

    public void HandleSelected()
    {
        SoundManager.Instance.PlayFx(FxID.TileSelect);
        visual.SetSelected(true);
    }

    public void HandleDeSelected()
    {
        visual.SetSelected(false);
    }

    public void HandleMatch()
    {
        visual.PlayMatchEffect(OnDespawn);
    }

    private void OnDespawn()
    {
        gameObject.SetActive(false);
    }

    public bool SetCellType(int cellType)
    {
        if (!Enum.IsDefined(typeof(TileType), cellType)) return false;
        tileType = (TileType)cellType;
        if (tileType == TileType.None)
        {
            OnDespawn();
        }
        else
        {
            Init();
        }
        return true;
    }
}

using UnityEngine;

public static class GameEvents
{
    public static System.Action<Vector2> OnTileClicked;
    public static System.Action<GameTile, GameTile> OnTilesMatched;
    public static System.Action OnWin;
    public static System.Action OnDeadlockDetected;
}
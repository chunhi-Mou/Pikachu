using System;
using UnityEngine;

public static class GameEvents
{
    public static Action<GameTile, GameTile> OnValidPairClicked;
    public static Action<Transform> OnFoundDeadlockObs;
    public static Action OnTilesMatched;
}
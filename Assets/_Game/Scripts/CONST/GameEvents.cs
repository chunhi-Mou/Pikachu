using System;
using UnityEngine;

public static class GameEvents
{
    public static Action<GameTile, GameTile> OnValidPairClicked;
    public static Action OnDeadlockDetected;
    public static Action OnTilesMatched;
    public static Action<Transform> OnFoundDeadlockObs;
}
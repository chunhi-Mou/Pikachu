using System;
using UnityEngine;

public class BoosterManager : Singleton<BoosterManager>
{
    [Header("Booster Settings")]
    [SerializeField] private int maxShuffleUse = 1;
    [SerializeField] private int maxHintUse = 1;
    
    public static event Action<int> OnShuffleCountChanged;
    public static event Action<int> OnHintCountChanged;

    private int currentShuffleUse;
    private int currentHintUse;
    
    public int CurrentShuffleUse => currentShuffleUse;
    public int CurrentHintUse => currentHintUse;

    protected void Start()
    {
        ResetBoosters();
    }

    public bool TryUseShuffle()
    {
        if (currentShuffleUse <= 0) return false;

        currentShuffleUse--;
        OnShuffleCountChanged?.Invoke(currentShuffleUse);
        
        SoundManager.Instance.PlayFx(FxID.Shuffle);
        TileManager.Instance.ShuffleTiles();
        return true;
    }

    public bool TryUseHint()
    {
        if (currentHintUse <= 0) return false;

        currentHintUse--;
        OnHintCountChanged?.Invoke(currentHintUse);
        
        SoundManager.Instance.PlayFx(FxID.Hint);
        TileManager.Instance.FindHintPair();
        return true;
    }

    public void ResetBoosters()
    {
        currentShuffleUse = maxShuffleUse;
        currentHintUse = maxHintUse;
        
        OnShuffleCountChanged?.Invoke(currentShuffleUse);
        OnHintCountChanged?.Invoke(currentHintUse);
    }
}
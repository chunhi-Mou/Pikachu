using System;
using UnityEngine;

public class GameTileVisual : MonoBehaviour
{
    [SerializeField] private SpriteRenderer tileIcon;
    [SerializeField] private GameObject VFXPrefab;
    [SerializeField] private GameObject EffectSelected;
    [SerializeField] private GameObject tileBG;
    [SerializeField] private new Collider2D collider2D;

    public void InitVisual(TileType type, Sprite sprite, bool isPlayable)
    {
        tileIcon.sprite = sprite;
        VFXPrefab.SetActive(false);
        EffectSelected.SetActive(false);
        tileBG.SetActive(isPlayable);
        tileIcon.transform.localScale = isPlayable ? Vector2.one : new Vector2(1.13f, 1.13f);
        collider2D.enabled = isPlayable;
    }

    public void PlayMatchEffect(Action onFinished)
    {
        EffectSelected.SetActive(false);
        collider2D.enabled = false;
        VFXPrefab.SetActive(true);
        Invoke(nameof(InvokeCallback), 0.5f);

        void InvokeCallback() => onFinished?.Invoke();
    }

    public void SetSelected(bool selected) => EffectSelected.SetActive(selected);
}


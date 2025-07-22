using System;
using System.Collections;
using UnityEngine;

public class GameTileVisual : MonoBehaviour
{
    [SerializeField] private SpriteRenderer tileIcon;
    [SerializeField] private GameObject VFXPrefab;
    [SerializeField] private GameObject EffectSelected;
    [SerializeField] private GameObject tileBG;
    [SerializeField] private new Collider2D collider2D;
    private Coroutine _playMatchEffectCoroutine;

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
        if (_playMatchEffectCoroutine != null)
        {
            StopCoroutine(_playMatchEffectCoroutine);
        }
        _playMatchEffectCoroutine = StartCoroutine(PlayMatchEffectRoutine(onFinished));
    }
    private IEnumerator PlayMatchEffectRoutine(Action onFinished)
    {
        EffectSelected.SetActive(false);
        collider2D.enabled = false;
        VFXPrefab.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        onFinished?.Invoke();
    }
    public void SetSelected(bool selected) => EffectSelected.SetActive(selected);
}


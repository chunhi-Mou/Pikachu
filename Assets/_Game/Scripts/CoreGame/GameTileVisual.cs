using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

public class GameTileVisual : MonoBehaviour
{
    [SerializeField] Animator tileAnimator;
    [SerializeField] private SpriteRenderer tileIcon;
    [SerializeField] private GameObject vfxPrefab;
    [SerializeField] private GameObject effectSelected;
    [SerializeField] private GameObject tileBg;
    [SerializeField] private new Collider2D collider2D;
    private Coroutine playMatchEffectCoroutine;

    private void Start()
    {
        Invoke(nameof(EnterEffect), 0.3f);
    }
    public void InitVisual(TileType type, Sprite sprite, bool isPlayable)
    {
        tileIcon.sprite = sprite;
        vfxPrefab.SetActive(false);
        effectSelected.SetActive(false);
        tileBg.SetActive(isPlayable);
        tileIcon.transform.localScale = isPlayable ? Vector2.one : new Vector2(1.13f, 1.13f);
        collider2D.enabled = isPlayable;
    }
    private void EnterEffect()
    {
        AnimatorUtils.ChangeAnimUI(GameCONST.TILE_ENTER, tileAnimator);
    }
    public void ShuffleEffect()
    {
        AnimatorUtils.ChangeAnimUI(GameCONST.TILE_SHUFFLE, tileAnimator);
    }
    public void PlayMatchEffect(Action onDespawn)
    {
        if (playMatchEffectCoroutine != null)
        {
            StopCoroutine(playMatchEffectCoroutine);
        }
        playMatchEffectCoroutine = StartCoroutine(PlayMatchEffectRoutine(onDespawn));
    }
    private IEnumerator PlayMatchEffectRoutine(Action onDespawn)
    {
        effectSelected.SetActive(false);
        collider2D.enabled = false;
        vfxPrefab.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        onDespawn?.Invoke();
    }
    public void SetSelected(bool selected) => effectSelected.SetActive(selected);
}


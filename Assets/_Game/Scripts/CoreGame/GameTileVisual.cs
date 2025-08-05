using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

public class GameTileVisual : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] Animator tileAnimator;
    [SerializeField] private SpriteRenderer tileIcon;
    [SerializeField] private new Collider2D collider2D;
    
    [Header("Effects")]
    [SerializeField] private GameObject vfxPrefab;
    [SerializeField] private GameObject effectSelected;
    [SerializeField] private GameObject tileBg;
    
    public void InitVisual(TileType type, Sprite sprite, bool isPlayable)
    {
        tileIcon.sprite = sprite;
        collider2D.enabled = isPlayable;
        
        vfxPrefab.SetActive(false);
        effectSelected.SetActive(false);
        tileBg.SetActive(isPlayable); // Vật cản không active BG
    }
    
    public void SetSelected(bool selected) => effectSelected.SetActive(selected);
    
    public void ShuffleEffect()
    {
        AnimatorUtils.ChangeAnimUI(GameCONST.Anim_TILE_SHUFFLE, tileAnimator);
    }
    
    public void MatchEffect(Action onDespawn)
    {
        collider2D.enabled = false;
        effectSelected.SetActive(false);
        vfxPrefab.SetActive(true);
        
        StartCoroutine(DespawnAfterDelay(onDespawn));
    }

    public void HintEffect()
    {
        AnimatorUtils.ChangeAnimUI(GameCONST.Anim_TILE_HINT, tileAnimator);
    }
    public void ResetAnim()
    {
        AnimatorUtils.ChangeAnimUI(GameCONST.Anim_TILE_NONE, tileAnimator);
    }
    
    private void Awake()
    {
        SoundManager.Instance.PlayFx(FxID.Enter);
    }
    private IEnumerator DespawnAfterDelay(Action onDespawn)
    {
        yield return new WaitForSeconds(0.5f);
        onDespawn?.Invoke();
    }
}


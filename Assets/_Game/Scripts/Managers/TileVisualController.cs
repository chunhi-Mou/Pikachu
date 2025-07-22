using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileVisualController : MonoBehaviour
{
    [SerializeField] LineRenderer lineRenderer;
    [SerializeField] private float zOffset = 0f;
    
    public void DrawPath(List<Vector2Int> path)
    {
        Vector3 origin = LevelManager.Instance.origin;
        Vector2 cellSize = LevelManager.Instance.cellSize;
        lineRenderer.positionCount = path.Count;
        for (int i = 0; i < path.Count; i++)
        {
            Vector2Int gridPos = new Vector2Int(path[i].x-1, path[i].y-1);
            Vector3 worldPos = GridUtils.GridToWorld(cellSize, origin, gridPos);
            worldPos.z = zOffset;
            lineRenderer.SetPosition(i, worldPos);
        }
        lineRenderer.enabled = true;
        StartCoroutine(FadeAndDisable(0.5f));
    }
    IEnumerator FadeAndDisable(float duration)
    {
        float time = 0f;
        Color startColor = lineRenderer.material.color;

        while (time < duration)
        {
            float alpha = Mathf.Lerp(1f, 0f, time / duration);
            Color newColor = startColor;
            newColor.a = alpha;
            lineRenderer.material.color = newColor;
            time += Time.deltaTime;
            yield return null;
        }
        lineRenderer.enabled = false;
    }
}
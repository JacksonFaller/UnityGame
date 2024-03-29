﻿using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(Transform))]
public class GhostTrail : MonoBehaviour
{
    public Movement MovementScript;
    public SpriteRenderer PlayerSpriteRenderer;
    public Color TrailColor;
    public Color FadeColor;
    public float GhostInterval;
    public float FadeTime;

    private Transform _ghostsParent;

    private void Start()
    {
        if (MovementScript == null) Debug.LogError("Movement script is not referenced!");
        if (PlayerSpriteRenderer == null) Debug.LogError("SpriteRenderer is not referenced!");
        _ghostsParent = GetComponent<Transform>();
    }

    public void ShowGhost()
    {
        Sequence s = DOTween.Sequence();

        for (int i = 0; i < _ghostsParent.childCount; i++)
        {
            Transform currentGhost = _ghostsParent.GetChild(i);
            SpriteRenderer spriteRenderer = currentGhost.GetComponent<SpriteRenderer>();
            s.AppendCallback(() => GhosTrailCallback(currentGhost, spriteRenderer));
            s.Append(spriteRenderer.material.DOColor(TrailColor, 0));
            s.AppendCallback(() => FadeSprite(spriteRenderer));
            s.AppendInterval(GhostInterval);
        }
    }

    private void GhosTrailCallback(Transform currentGhost, SpriteRenderer spriteRenderer)
    {
        currentGhost.position = MovementScript.transform.position;
        spriteRenderer.flipX = PlayerSpriteRenderer.flipX;
        spriteRenderer.sprite = PlayerSpriteRenderer.sprite;
    }

    public void FadeSprite(SpriteRenderer spriteRenderer)
    {
        spriteRenderer.material.DOKill();
        spriteRenderer.material.DOColor(FadeColor, FadeTime);
    }
}

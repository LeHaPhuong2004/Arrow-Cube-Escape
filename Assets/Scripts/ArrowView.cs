using UnityEngine;
using DG.Tweening;
using System.Collections;

public class ArrowView : MonoBehaviour
{
    public Vector2Int gridPos;
    public SpriteRenderer sr;

    public FaceType face;

    public void PlayBlockedFeedback()
    {
        StopAllCoroutines();
        StartCoroutine(BlockedEffect());
    }

    IEnumerator BlockedEffect()
    {
        if (sr == null) yield break;

        Color originalColor = sr.color;
        Vector3 originalPos = transform.localPosition;  
        sr.color = Color.red;

        float duration = 0.15f;
        float elapsed = 0f;
        float strength = 0.08f;

        while (elapsed < duration)
        {
            transform.localPosition = originalPos + (Vector3)Random.insideUnitCircle * strength;
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = originalPos;
      
    }

    public void SetDirection(Direction dir)
    {
        float angle = 0f;
        switch (dir)
        {
            case Direction.Up: angle = 0f; break;
            case Direction.Right: angle = -90f; break;
            case Direction.Down: angle = 180f; break;
            case Direction.Left: angle = 90f; break;
        }

        transform.localRotation = Quaternion.Euler(0, 0, angle);
    }
}
using UnityEngine;
using DG.Tweening;

public class ArrowView : MonoBehaviour
{
    public Vector2Int gridPos;

 
    public FaceType face;

    public void PlayBlockedFeedback()
    {
        transform.DOShakePosition(0.2f, 0.2f, 10, 90, false, true);
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
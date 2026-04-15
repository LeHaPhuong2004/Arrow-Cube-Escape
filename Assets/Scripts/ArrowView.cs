using UnityEngine;

public class ArrowView : MonoBehaviour
{
    public Vector2Int gridPos;

    public void SetDirection(Direction dir)
    {
        float angle = 0f;

        switch (dir)
        {
            case Direction.Up:
                angle = 0f;
                break;
            case Direction.Right:
                angle = -90f;
                break;
            case Direction.Down:
                angle = 180f;
                break;
            case Direction.Left:
                angle = 90f;
                break;
        }

        transform.rotation = Quaternion.Euler(0, 0, angle);
    }
}
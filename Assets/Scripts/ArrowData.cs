using UnityEngine;

public enum Direction
{
    None, Up, Down, Left, Right
}
[System.Serializable]
public class ArrowData
{
    public Direction direction;
    public bool isRemoved;
    public ArrowData(Direction dir)
    {
        direction = dir;
        isRemoved = false;
    }
}

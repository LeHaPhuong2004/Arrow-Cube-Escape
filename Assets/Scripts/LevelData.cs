using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ArrowSpawnData
{
    public Vector2Int position;
    public float zOffset; 
    public Direction direction;
    public FaceType face; 
}
public enum FaceType
{
    Front,
    Back,
    Left,
    Right,
    Top,
    Bottom
}
[CreateAssetMenu(fileName = "LevelData", menuName = "Game/Level")]
public class LevelData : ScriptableObject
{
    public int optimalSteps = 5;
    public int gridSize = 5;
    public List<ArrowSpawnData> arrows;

}
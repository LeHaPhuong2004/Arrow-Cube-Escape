using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Collections;
public class GridManager : MonoBehaviour
{
    public UIManager uIManager;
    public List<LevelData> levels;
    private int currentLevelIndex = 0;
    public LevelData currentLevel;
    private int stepCount = 0;
    private bool isMoving = false;
    public const int GRID_SIZE = 5;

    public GameObject arrowPrefab;
    public float cellSize = 1.5f;

    public ArrowData[,] grid;

    private Dictionary<Vector2Int, ArrowView> viewMap = new Dictionary<Vector2Int, ArrowView>();

    void Start()
    {
        InitGrid();
    }

    void InitGrid()
    {
        grid = new ArrowData[currentLevel.gridSize, currentLevel.gridSize];

        foreach (var arrow in currentLevel.arrows)
        {
            grid[arrow.position.x, arrow.position.y] = new ArrowData(arrow.direction);
        }

        SpawnAllArrows();
    }

    void SpawnAllArrows()
    {

        for (int x = 0; x < GRID_SIZE; x++)
        {
            for (int y = 0; y < GRID_SIZE; y++)
            {
                if (grid[x, y] != null)
                {
                    SpawnArrow(new Vector2Int(x, y));
                }
            }
        }
    }

    void SpawnArrow(Vector2Int pos)
    {

        Vector3 worldPos = GridToWorld(pos);

        GameObject obj = Instantiate(arrowPrefab, worldPos, Quaternion.identity);
        ArrowView view = obj.GetComponent<ArrowView>();
        view.gridPos = pos;
        view.SetDirection(grid[pos.x, pos.y].direction);
        viewMap[pos] = view;
       
    }

    Vector3 GridToWorld(Vector2Int pos)
    {
        // Tính toán độ lệch (Offset) để đưa tâm Grid về (0,0)
        // Công thức: (Số ô - 1) * cellSize / 2
        float offset = (GRID_SIZE - 1) * cellSize / 2f;

        float x = pos.x * cellSize - offset;
        float y = pos.y * cellSize - offset;

        return new Vector3(x, y, 0);
    }

    public void OnArrowClicked(Vector2Int pos)
    {
        if (isMoving) return; // 🔥 chặn spam

        var path = GetSlidePath(pos);

        if (path == null)
        {
            Debug.Log("❌ BLOCKED");
            return;
        }

        Debug.Log("✅ MOVE");

        StartCoroutine(MoveArrowAnimated(pos, path));

        stepCount++;
        uIManager.UpdateStep(stepCount, currentLevel.optimalSteps);
        Debug.Log("Step: " + stepCount);
    }
    public void RestartLevel()
    {
        ResetLevel();
    }
    bool CheckWin()
    {
        for (int x = 0; x < GRID_SIZE; x++)
        {
            for (int y = 0; y < GRID_SIZE; y++)
            {
                if (grid[x, y] != null && !grid[x, y].isRemoved)
                {
                    return false;
                }
            }
        }
        return true;
    }
    public List<Vector2Int> GetSlidePath(Vector2Int start)
    {
        List<Vector2Int> path = new List<Vector2Int>();

        ArrowData arrow = grid[start.x, start.y];

        if (arrow == null || arrow.isRemoved)
            return null;

        Vector2Int dir = GetDirectionVector(arrow.direction);
        Vector2Int current = start;

        while (true)
        {
            current += dir;

            if (IsOutOfBounds(current))
            {
                return path;
            }

            if (grid[current.x, current.y] != null &&
                !grid[current.x, current.y].isRemoved)
            {
                return null;
            }

            path.Add(current);
        }
    }
    int CalculateStars()
    {
        int optimal = currentLevel.optimalSteps;

        if (stepCount <= optimal) return 3;
        if (stepCount <= optimal + 3) return 2;
        return 1;
    }
    Vector2Int GetDirectionVector(Direction dir)
    {
        switch (dir)
        {
            case Direction.Up: return new Vector2Int(0, 1);
            case Direction.Down: return new Vector2Int(0, -1);
            case Direction.Left: return new Vector2Int(-1, 0);
            case Direction.Right: return new Vector2Int(1, 0);
            default: return Vector2Int.zero;
        }
    }

    bool IsOutOfBounds(Vector2Int pos)
    {
        return pos.x < 0 || pos.x >= GRID_SIZE ||
               pos.y < 0 || pos.y >= GRID_SIZE;
    }

    IEnumerator MoveArrowAnimated(Vector2Int start, List<Vector2Int> path)
    {
        isMoving = true;

        ArrowView view = viewMap[start];

        Vector3 finalPos = GridToWorld(path[path.Count - 1]);

        float duration = 0.4f; // chuẩn đề

        view.transform.DOMove(finalPos, duration)
            .SetEase(Ease.OutCubic);

        yield return new WaitForSeconds(duration);

        // 👉 bay ra ngoài 1 chút (cho đẹp)
        Vector2Int dir = GetDirectionVector(grid[start.x, start.y].direction);
        Vector3 outPos = finalPos + new Vector3(dir.x, dir.y, 0) * cellSize;

        view.transform.DOMove(outPos, 0.2f)
            .SetEase(Ease.InQuad);

        yield return new WaitForSeconds(0.2f);

        // remove
        grid[start.x, start.y].isRemoved = true;
        Destroy(view.gameObject);
        viewMap.Remove(start);

        isMoving = false;

        if (CheckWin())
        {
            int stars = CalculateStars(); // 🔥 tính sao

            uIManager.ShowWin(stars);
        }
    }

    public void NextLevel()
    {


        currentLevelIndex++;
        Debug.Log("Index: " + currentLevelIndex);
        Debug.Log("Count: " + levels.Count);
        if (currentLevelIndex >= levels.Count)
        {
            currentLevelIndex = 0;
        }

        currentLevel = levels[currentLevelIndex];

        ResetLevel();

    }
    void ResetLevel()
    {
        stepCount = 0;

        foreach (var v in viewMap.Values)
        {
            Destroy(v.gameObject);
        }

        viewMap.Clear();

        InitGrid();
    }

    
}
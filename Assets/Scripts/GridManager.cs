using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Collections;
using static UnityEditor.PlayerSettings;
public class GridManager : MonoBehaviour
{
    public UIManager uIManager;
    public List<LevelData> levels;
    private int currentLevelIndex = 0;
    public LevelData currentLevel;
    private int stepCount = 0;
    private bool isMoving = false;
    public const int GRID_SIZE = 5;
    public int unlockedLevel;
    public GameObject arrowPrefab;
    public float cellSize = 1.5f;
    public CubeController cubeController;
    public ArrowData[,] grid;
    public Transform faceRoot;
    private Dictionary<Vector2Int, ArrowView> viewMap = new Dictionary<Vector2Int, ArrowView>();

    void Start()
    {
        PlayerPrefs.DeleteAll(); // để tạm de reset level tiện debug
        unlockedLevel = PlayerPrefs.GetInt("UnlockedLevel", 0);
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

        for (int x = 0; x < currentLevel.gridSize; x++)
        {
            for (int y = 0; y < currentLevel.gridSize; y++)
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
        // tìm arrow data tương ứng
        ArrowSpawnData arrow = currentLevel.arrows.Find(a => a.position == pos);

        Vector3 worldPos = GridToWorld(pos);

        if (arrow != null)
        {
            worldPos.z += arrow.zOffset;
        }

        GameObject obj = Instantiate(arrowPrefab);
        obj.transform.position = worldPos;
        obj.transform.rotation = Quaternion.identity;
        obj.transform.SetParent(faceRoot, true);

        ArrowView view = obj.GetComponent<ArrowView>();
        view.gridPos = pos;

        view.SetDirection(grid[pos.x, pos.y].direction);

        viewMap[pos] = view;
    }

    Vector3 GridToWorld(Vector2Int pos)
    {

        float offset = (currentLevel.gridSize - 1) * cellSize / 2f;

        float x = pos.x * cellSize - offset;
        float y = pos.y * cellSize - offset;

        return new Vector3(x, y, 0);
    }

    public void OnArrowClicked(Vector2Int pos)
    {
        if (isMoving) return;

        var path = GetSlidePath(pos);

        stepCount++; 
        uIManager.UpdateStep(stepCount, currentLevel.optimalSteps);

        if (path == null)
        {
            Debug.Log("block");

         
            if (viewMap.ContainsKey(pos))
            {
                viewMap[pos].PlayBlockedFeedback();
            }

            return;
        }

        Debug.Log("move");

        StartCoroutine(MoveArrowAnimated(pos, path));
    }
    public void RestartLevel()
    {
        ResetLevel();
    }
    bool CheckWin()
    {
        for (int x = 0; x < currentLevel.gridSize; x++)
        {
            for (int y = 0; y < currentLevel.gridSize; y++)
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
        return pos.x < 0 || pos.x >= currentLevel.gridSize ||
       pos.y < 0 || pos.y >= currentLevel.gridSize;
    }
    private bool isLoading = false;

    public void LoadLevel(int index)
    {
        if (isLoading) return;
        isLoading = true;

        currentLevelIndex = index;
        currentLevel = levels[index];

        ResetLevel();


        isLoading = false;
    }
    IEnumerator MoveArrowAnimated(Vector2Int start, List<Vector2Int> path)
    {
        isMoving = true;

        ArrowView view = viewMap[start];
        if (view == null) yield break;

        Vector3 finalPos = GridToWorld(path[path.Count - 1]);

        float duration = 0.4f;
        view.transform.DOKill();
        view.transform.DOMove(finalPos, duration)
            .SetEase(Ease.OutCubic);

        yield return new WaitForSeconds(duration);

       
        Vector2Int dir = GetDirectionVector(grid[start.x, start.y].direction);
        Vector3 outPos = finalPos + new Vector3(dir.x, dir.y, 0) * cellSize;

        view.transform.DOMove(outPos, 0.2f)
            .SetEase(Ease.InQuad);

        yield return new WaitForSeconds(0.2f);

       
        grid[start.x, start.y].isRemoved = true;
        view.transform.DOKill();
        Destroy(view.gameObject);
        viewMap.Remove(start);

        isMoving = false;

        if (CheckWin())
        {
            int stars = CalculateStars();

            if (currentLevelIndex >= unlockedLevel)
            {
                unlockedLevel = currentLevelIndex + 1;
                PlayerPrefs.SetInt("UnlockedLevel", unlockedLevel);
                PlayerPrefs.Save();
            }

            cubeController.gameObject.SetActive(false); // 👈 thêm dòng này

            uIManager.ShowWin(stars);
        }
    }

    public void NextLevel()
    {
        currentLevelIndex++;

        if (currentLevelIndex >= levels.Count)
        {
            currentLevelIndex = 0;
        }

        LoadLevel(currentLevelIndex); 
    }
    public void ResetLevel()
    {
        cubeController.gameObject.SetActive(true);
        stepCount = 0;
        isMoving = false;

        StopAllCoroutines();
        DOTween.KillAll();

        foreach (var v in viewMap.Values)
        {
            if (v != null)
                Destroy(v.gameObject);
        }

        viewMap.Clear();

        InitGrid();

        uIManager.UpdateStep(stepCount, currentLevel.optimalSteps);
    }


}
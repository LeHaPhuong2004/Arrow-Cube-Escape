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
    public int unlockedLevel;
    public GameObject arrowPrefab;
    public float cellSize = 1.5f;
    public CubeController cubeController;
    public Dictionary<FaceType, ArrowData[,]> grids = new Dictionary<FaceType, ArrowData[,]>();
    private List<ArrowView> views = new List<ArrowView>();
    public Transform frontFace;
    public Transform backFace;
    public Transform leftFace;
    public Transform rightFace;
    public Transform topFace;
    public Transform bottomFace;
    void Start()
    {
        PlayerPrefs.DeleteAll();
        unlockedLevel = PlayerPrefs.GetInt("UnlockedLevel", 0);
    }
  Transform GetFaceRoot(FaceType face)
    {
        switch (face)
        {
            case FaceType.Front: return frontFace;
            case FaceType.Back: return backFace;
            case FaceType.Left: return leftFace;
            case FaceType.Right: return rightFace;
            case FaceType.Top: return topFace;
            case FaceType.Bottom: return bottomFace;
            default: return frontFace;
        }
    }
    void InitGrid()
    {
        grids.Clear();

        foreach (FaceType face in System.Enum.GetValues(typeof(FaceType)))
        {
            grids[face] = new ArrowData[currentLevel.gridSize, currentLevel.gridSize];
        }
        foreach (var arrow in currentLevel.arrows)
        {
            grids[arrow.face][arrow.position.x, arrow.position.y] =
                new ArrowData(arrow.direction);
        }
        SpawnAllArrows();
    }
    void SpawnAllArrows()
    {
        foreach (var arrow in currentLevel.arrows)
        {
            SpawnArrow(arrow);
        }
    }
    void SpawnArrow(ArrowSpawnData arrow)
    {

        GameObject obj = Instantiate(arrowPrefab);
        Transform face = GetFaceRoot(arrow.face);
        obj.transform.SetParent(face, false);
        Vector3 localPos = GridToLocal(arrow.position);
        localPos.z += arrow.zOffset;
        obj.transform.localPosition = localPos;
        obj.transform.localRotation = Quaternion.identity;
        ArrowView view = obj.GetComponent<ArrowView>();
        view.gridPos = arrow.position;
        view.face = arrow.face;
       view.SetDirection(arrow.direction);

        views.Add(view);
    }
    Vector3 GridToLocal(Vector2Int pos)
    {
        float offset = (currentLevel.gridSize - 1) * cellSize / 2f;
        float x = pos.x * cellSize - offset;
        float y = pos.y * cellSize - offset;
        return new Vector3(x, y, 0);
    }
    public void OnArrowClicked(Vector2Int pos, FaceType face)
    {
        if (isMoving) return;
        var v = GetView(pos, face);
        if (v != null)
        {
            v.transform.DOPunchScale(Vector3.one * 0.2f, 0.15f, 5, 0.5f);
        }
        var path = GetSlidePath(pos, face);
        stepCount++;
        uIManager.UpdateStep(stepCount, currentLevel.optimalSteps);
        if (path == null)
        {
            AudioManager.Instance.PlayBlock();
            if (v != null) v.PlayBlockedFeedback();
            return;
        }
        AudioManager.Instance.PlayClick();
        StartCoroutine(MoveArrowAnimated(pos, face, path));
    }
    public List<Vector2Int> GetSlidePath(Vector2Int start, FaceType face)
    {
        List<Vector2Int> path = new List<Vector2Int>();
        var grid = grids[face];
        ArrowData arrow = grid[start.x, start.y];
        if (arrow == null || arrow.isRemoved)
            return null;
        Vector2Int dir = GetDirectionVector(arrow.direction);
        Vector2Int current = start;
        while (true)
        {
            current += dir;
            if (IsOutOfBounds(current))
                return path;
            if (grid[current.x, current.y] != null &&
                !grid[current.x, current.y].isRemoved)
                return null;
            path.Add(current);
        }
    }
    IEnumerator MoveArrowAnimated(Vector2Int start, FaceType face, List<Vector2Int> path)
    {
        isMoving = true;
        ArrowView view = GetView(start, face);
        if (view == null) yield break;

        // 1. Kích hoạt hiệu ứng rắn
        view.StartSnakeEffect();

        Vector3 finalPos = GridToLocal(path[path.Count - 1]);
        float duration = Mathf.Clamp(path.Count * 0.2f, 0.4f, 1.2f);


        // 2. Di chuyển: Chỉ nên di chuyển visualRoot hoặc dùng transform nhưng phải cẩn thận
        // vì LineRenderer đang dùng WorldSpace nên nó sẽ tự "để lại đuôi" khi đầu di chuyển
        view.transform.DOLocalMove(finalPos, duration).SetEase(Ease.Linear);

        yield return new WaitForSeconds(duration);

        // 3. Xử lý thoát ra ngoài map
        Vector2Int dirVec = GetDirectionVector(grids[face][start.x, start.y].direction);
        Vector3 outPos = finalPos + new Vector3(dirVec.x, dirVec.y, 0) * cellSize;

        view.transform.DOLocalMove(outPos, 0.2f).SetEase(Ease.InQuad);
        yield return new WaitForSeconds(0.2f);

        // Dọn dẹp
        grids[face][start.x, start.y].isRemoved = true;
        views.Remove(view);
        Destroy(view.gameObject);
        isMoving = false;

        if (CheckWin())
        {
            int stars = CalculateStars();

            if (VFXManager.Instance != null)
            {
                VFXManager.Instance.PlayWinEffect();
            }

            AudioManager.Instance.PlayWin();

            if (currentLevelIndex >= unlockedLevel)
            {
                unlockedLevel = currentLevelIndex + 1;
                PlayerPrefs.SetInt("UnlockedLevel", unlockedLevel);
                PlayerPrefs.Save();
            }

            cubeController.gameObject.SetActive(false); 
            yield return new WaitForSeconds(0.2f);

            uIManager.ShowWin(stars);
        }
    }

    ArrowView GetView(Vector2Int pos, FaceType face)
    {
        foreach (var v in views)
        {
            if (v.gridPos == pos && v.face == face)
                return v;
        }
        return null;
    }
    bool CheckWin()
    {
        foreach (var pair in grids)
        {
            var grid = pair.Value;

            for (int x = 0; x < currentLevel.gridSize; x++)
            {
                for (int y = 0; y < currentLevel.gridSize; y++)
                {
                    if (grid[x, y] != null && !grid[x, y].isRemoved)
                        return false;
                }
            }
        }
        return true;
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
    public void LoadLevel(int index)
    {
        currentLevelIndex = index;
        currentLevel = levels[index];
        ResetLevel();
    }
    public void NextLevel()
    {
        currentLevelIndex++;

        if (currentLevelIndex >= levels.Count)
            currentLevelIndex = 0;

        LoadLevel(currentLevelIndex);
    }
    public void RestartLevel()
    {
        ResetLevel();
    }
    public void ResetLevel()
    {
        cubeController.gameObject.SetActive(true);

        stepCount = 0;
        isMoving = false;

        StopAllCoroutines();
        DOTween.KillAll();

        foreach (var v in views)
        {
            if (v != null)
                Destroy(v.gameObject);
        }

        views.Clear();
        InitGrid();
        uIManager.UpdateStep(stepCount, currentLevel.optimalSteps);
    }
}
using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class ArrowView : MonoBehaviour
{
    [Header("Settings")]
    public Vector2Int gridPos;
    public FaceType face;
    public int pointsCount = 60;            // Tăng lên để thân dài hơn khi bò
    public float distanceBetweenPoints = 0.04f;

    [Header("References")]
    public LineRenderer line;
    public Transform head;         // Cái Triangle
    public Transform visualRoot;   // Cha của Triangle và Line
    public SpriteRenderer colorHead;

    private List<Vector3> historyPositions = new List<Vector3>();
    private SpriteRenderer[] renderers;
    private bool isSliding = false;
    private Vector3 lastHeadPos;

    [Header("Color Settings")]
    public Color blockedColor = Color.red;
    private Color originalColor = Color.black; // Màu gốc của mũi tên
    private static readonly int ShaderColorId = Shader.PropertyToID("_BaseColor");

    private MaterialPropertyBlock propBlock;
    private Renderer headRenderer; // Dùng Renderer chung cho cả Sprite/Mesh
    private Vector3 lastInsertPos;

    private float sampleTimer;
    public float sampleRate = 0.02f; // 50fps sampling
    void Awake()
    {
        propBlock = new MaterialPropertyBlock();
        if (head != null) headRenderer = head.GetComponent<Renderer>();

        // Lấy màu gốc từ Material của LineRenderer lúc đầu
        if (line != null && line.sharedMaterial != null)
            originalColor = line.sharedMaterial.GetColor(ShaderColorId);
    }

    // Hàm phụ trợ để đổi màu Material cho cả Line và Head
    private void SetVisualColor(Color targetColor)
    {
        propBlock.SetColor(ShaderColorId, targetColor);

        if (line != null) line.SetPropertyBlock(propBlock);
        if (headRenderer != null) headRenderer.SetPropertyBlock(propBlock);
    }

    void Start()
    {
        // KHÔNG nạp historyPositions ở đây nữa. 
        // Hãy để LineRenderer tự hiển thị các điểm Local bạn đã vẽ trong Prefab.
        if (line != null)
        {
            line.useWorldSpace = false;
        }
    }

    public void StartSnakeEffect()
    {
        if (line == null || head == null) return;

        int existingCount = line.positionCount;
        Vector3[] worldPoints = new Vector3[existingCount];

        for (int i = 0; i < existingCount; i++)
        {
            worldPoints[i] = line.transform.TransformPoint(line.GetPosition(i));
        }

        historyPositions.Clear();

        // ❗ GIỮ NGUYÊN THỨ TỰ
        for (int i = 0; i < existingCount; i++)
        {
            historyPositions.Add(worldPoints[i]);
        }

        // ❗ ÉP ĐIỂM ĐẦU = HEAD
        historyPositions[0] = head.position;

        while (historyPositions.Count < pointsCount)
        {
            historyPositions.Add(historyPositions[historyPositions.Count - 1]);
        }

        line.useWorldSpace = true;
        isSliding = true;
        lastInsertPos = head.position; // Gán vị trí mốc ngay khi bắt đầu
    
    }

    void Update()
    {
        if (!isSliding) return;

        sampleTimer += Time.deltaTime;

        if (sampleTimer >= sampleRate)
        {
            sampleTimer = 0f;

            // chỉ INSERT, không overwrite
            historyPositions.Insert(0, head.position);

            if (historyPositions.Count > pointsCount)
            {
                historyPositions.RemoveAt(historyPositions.Count - 1);
            }
        }

        line.positionCount = historyPositions.Count;

        for (int i = 0; i < historyPositions.Count; i++)
        {
            line.SetPosition(i, historyPositions[i]);
        }
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
        if (visualRoot != null)
            visualRoot.localRotation = Quaternion.Euler(0, 0, angle);
    }

    public void PlayBlockedFeedback()
    {
        StopAllCoroutines();
        StartCoroutine(BlockedEffect());
    }

    IEnumerator BlockedEffect()
    {
        // 1. Đổi sang màu đỏ
        SetVisualColor(blockedColor);

        Vector3 originalPos = transform.localPosition;
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


}
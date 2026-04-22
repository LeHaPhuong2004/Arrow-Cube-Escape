using UnityEngine;

public class CameraZoom : MonoBehaviour
{
    public float zoomSpeedMouse = 10f;
    public float zoomSpeedTouch = 0.01f; 

    public float minZoom = 5f;
    public float maxZoom = 10f;

    public float minFOV = 35f;
    public float maxFOV = 95f;
    public static bool isZooming = false;
    Camera cam;

    void Start()
    {
        cam = GetComponent<Camera>();

        if (cam == null)
        {
            Debug.LogError("No cam");
        }
    }

    void Update()
    {
        isZooming = Input.touchCount == 2 || Mathf.Abs(Input.mouseScrollDelta.y) > 0.01f;

        if (Input.touchCount == 2)
        {
            isZooming = true;
            HandleTouchZoom();
        }
        else
        {
            HandleMouseZoom();
        }
    }

    void HandleMouseZoom()
    {
        float scroll = Input.mouseScrollDelta.y;

        if (Mathf.Abs(scroll) > 0.01f)
        {
            isZooming = true; // ✅ thêm dòng này
            ApplyZoom(scroll * zoomSpeedMouse);
        }
    }

    void HandleTouchZoom()
    {
        Touch t0 = Input.GetTouch(0);
        Touch t1 = Input.GetTouch(1);

        Vector2 t0Prev = t0.position - t0.deltaPosition;
        Vector2 t1Prev = t1.position - t1.deltaPosition;

        float prevDist = Vector2.Distance(t0Prev, t1Prev);
        float currDist = Vector2.Distance(t0.position, t1.position);

        float delta = currDist - prevDist;

        ApplyZoom(delta * zoomSpeedTouch);
    }

    void ApplyZoom(float delta)
    {
        if (cam.orthographic)
        {
            cam.orthographicSize += delta;
            cam.orthographicSize = Mathf.Clamp(cam.orthographicSize, minZoom, maxZoom);
        }
        else
        {
            cam.fieldOfView += delta;
            cam.fieldOfView = Mathf.Clamp(cam.fieldOfView, minFOV, maxFOV);
        }
    }
}
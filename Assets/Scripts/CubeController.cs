using UnityEngine;

public class CubeController : MonoBehaviour
{
    public Transform cube;
    public float rotateDuration = 0.25f;
    private bool isRotating = false;
    private Vector3 lastMousePos;
    public float sensitivity = 0.2f;
    public Transform faceRoot;
    private Vector2 velocity;
    public float smoothTime = 0.1f;
    public float damping = 5f;
    void Update()
    {
        faceRoot.rotation = cube.rotation;

        if (Input.GetMouseButtonDown(0))
        {
            lastMousePos = Input.mousePosition;
        }

        if (Input.GetMouseButton(0))
        {
            Vector3 delta = Input.mousePosition - lastMousePos;

            velocity = new Vector2(-delta.y, delta.x) * sensitivity;

            lastMousePos = Input.mousePosition;
        }
 
        cube.Rotate(Vector3.right, velocity.x * Time.deltaTime * 100f, Space.World);
        cube.Rotate(Vector3.up, velocity.y * Time.deltaTime * 100f, Space.World);

        velocity = Vector2.Lerp(velocity, Vector2.zero, Time.deltaTime * damping);
    }
    public void RotateLeft()
    {
        if (isRotating) return;
        StartCoroutine(Rotate(Vector3.up, 90));
    }

    public void RotateRight()
    {
        if (isRotating) return;
        StartCoroutine(Rotate(Vector3.up, -90));
    }

    public void RotateUp()
    {
        if (isRotating) return;
        StartCoroutine(Rotate(Vector3.right, 90));
    }

    public void RotateDown()
    {
        if (isRotating) return;
        StartCoroutine(Rotate(Vector3.right, -90));
    }

    System.Collections.IEnumerator Rotate(Vector3 axis, float angle)
    {
        isRotating = true;

        Quaternion startRot = cube.rotation;
        Quaternion endRot = Quaternion.AngleAxis(angle, axis) * cube.rotation;

        float t = 0;

        while (t < 1)
        {
            t += Time.deltaTime / rotateDuration;

            // 🎯 ease-out
            float easedT = Mathf.SmoothStep(0, 1, t);

            cube.rotation = Quaternion.Slerp(startRot, endRot, easedT);

            yield return null;
        }

        Vector3 euler = cube.rotation.eulerAngles;

        euler.x = Mathf.Round(euler.x / 90f) * 90f;
        euler.y = Mathf.Round(euler.y / 90f) * 90f;
        euler.z = Mathf.Round(euler.z / 90f) * 90f;

        cube.rotation = Quaternion.Euler(euler);

        isRotating = false;
    }

}
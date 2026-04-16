using UnityEngine;

public class CubeController : MonoBehaviour
{
    public Transform cube;
    public float rotateDuration = 0.25f;
    private bool isRotating = false;
    private Vector3 lastMousePos;
    public float sensitivity = 0.2f;
    public Transform faceRoot;

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

            float rotX = -delta.y * sensitivity;
            float rotY = delta.x * sensitivity;

            cube.Rotate(Vector3.right, rotX, Space.World);
            cube.Rotate(Vector3.up, rotY, Space.World);

            lastMousePos = Input.mousePosition;
        }
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
            cube.rotation = Quaternion.Slerp(startRot, endRot, t);
            yield return null;
        }

        cube.rotation = endRot;
        isRotating = false;
    }

}
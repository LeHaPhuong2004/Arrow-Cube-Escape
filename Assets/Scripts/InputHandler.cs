using UnityEngine;

public class InputHandler : MonoBehaviour
{
    public Camera cam;
    public GridManager gridManager;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                ArrowView arrow = hit.collider.GetComponent<ArrowView>();

                if (arrow != null)
                {
                
                    gridManager.OnArrowClicked(arrow.gridPos, arrow.face);
                }
            }
        }
    }
}
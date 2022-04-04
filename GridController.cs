using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridController : MonoBehaviour
{
    public Vector2Int gridSize;
    public float cellRadius = 0.5f;
    public FlowField curFlowField;

    private void InitializeFlowField()
    {
        curFlowField = new FlowField(cellRadius, gridSize);
        curFlowField.CreateGrid();
    }


    private void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            InitializeFlowField();

            curFlowField.CreateCostField();

            Vector3 mousePos = Input.mousePosition;
            Vector3 worldMousePos = Camera.main.ScreenToWorldPoint(mousePos);
            Cell destinationCell = curFlowField.GetCellFromWorldPos(worldMousePos);
            curFlowField.CreateIntegrationField(destinationCell);

            curFlowField.CreateFlowField();

            GridDebug.SetCurFlowField(curFlowField);
            GridDebug.DrawFlowField();
            //Debug.Log("Mouse Clicked at Screen Coordinates " + mousePos.x + " " + mousePos.y + " " + mousePos.z);
            //Debug.Log("Mouse clicked at world coordinates " + worldMousePos.x + " " + worldMousePos.y + " " + worldMousePos.z);
            //Debug.Log("Cell clicked is " + destinationCell.gridPosition.x + " " + destinationCell.gridPosition.y);
        }

        if (Input.GetMouseButtonDown(1))
        {
            curFlowField = null;
            GridDebug.SetCurFlowField(null);
            GridDebug.ClearChild();
            //Debug.Log("Mouse Right Clicked");
        }
    }


}

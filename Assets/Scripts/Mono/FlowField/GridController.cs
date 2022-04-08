using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GridController : MonoBehaviour
{
    public Vector2Int gridSize;
    public float cellRadius = 0.5f;
    public static List<FlowField> flowFieldList;

    private void Awake()
    {
        flowFieldList = new List<FlowField>();
    }

    private void Start()
    {
        
    }

    private void Update()
    {
        Debug.Log(flowFieldList.Count);
    }

    public void AddNewFlowField(Vector3 _worldMousePos, ref Action _action)
    {
        FlowField newFlowField = new FlowField(cellRadius, gridSize);
        newFlowField.CreateGrid();

        newFlowField.CreateCostField();

        Cell destinationCell = newFlowField.GetCellFromWorldPos(_worldMousePos);

        newFlowField.CreateIntegrationField(destinationCell);
        newFlowField.CreateFlowField();


        GridDebug.SetCurFlowField(newFlowField);
        GridDebug.DrawFlowField();
        //Debug.Log("Mouse Clicked at Screen Coordinates " + mousePos.x + " " + mousePos.y + " " + mousePos.z);
        //Debug.Log("Mouse clicked at world coordinates " + worldMousePos.x + " " + worldMousePos.y + " " + worldMousePos.z);
        //Debug.Log("Cell clicked is " + destinationCell.gridPosition.x + " " + destinationCell.gridPosition.y);

        flowFieldList.Add(newFlowField);

        _action.flowFieldIndex = flowFieldList.Count - 1;
    }

}

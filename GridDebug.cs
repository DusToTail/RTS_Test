using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridDebug : MonoBehaviour
{
    [SerializeField]
    private bool displayGrid;
    [SerializeField]
    private GridController gridController;
    private static FlowField curFlowField;


    public static void SetCurFlowField(FlowField flowField)
    {
        curFlowField = flowField;
    }

    private void OnDrawGizmos()
    {
        if(displayGrid)
        {
            if(curFlowField == null)
                DrawGrid(gridController.gridSize, Color.yellow, gridController.cellRadius);
            else
                DrawGrid(curFlowField.gridSize, Color.green, curFlowField.cellRadius);
        }
    }

    private void DrawGrid(Vector2Int drawGridSize, Color drawColor, float drawCellRadius)
    {
        Gizmos.color = drawColor;
        for(int x = 0; x < drawGridSize.x; x++)
        {
            for(int y = 0; y < drawGridSize.y; y++)
            {
                Vector3 center = new Vector3(drawCellRadius * 2 * x + drawCellRadius, 0, drawCellRadius * 2 * y + drawCellRadius);
                Vector3 size = Vector3.one * drawCellRadius * 2;
                Gizmos.DrawWireCube(center, size);
            }
        }
    }

}

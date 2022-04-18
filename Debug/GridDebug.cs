using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class GridDebug : MonoBehaviour
{
    [SerializeField]
    private bool displayGrid;
    [SerializeField]
    private GridController gridController;

    enum FlowFieldDisplayType {None, FlowField, CostField, IntegrationField };
    [SerializeField]
    private FlowFieldDisplayType curDisplayType;

    [SerializeField]
    private Sprite[] sprites;


    private static FlowField curFlowField;

    private static GridDebug instance;

    private void Start()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this.gameObject);
    }

    public static void SetCurFlowField(FlowField flowField)
    {
        curFlowField = flowField;
    }

    private void OnDrawGizmos()
    {
        if(Application.isPlaying == false) { return; }
        if(displayGrid)
        {
            if(curFlowField == null)
                DrawGrid(gridController.gridSize, Color.yellow, gridController.cellRadius);
            else
                DrawGrid(curFlowField.gridSize, Color.green, curFlowField.cellRadius);

            if(curFlowField == null) { return; }

            GUIStyle style = new GUIStyle(GUI.skin.label);
            style.alignment = TextAnchor.MiddleCenter;
            
            switch(curDisplayType)
            {
                case FlowFieldDisplayType.CostField:
                    foreach(Cell cell in curFlowField.grid)
                    {
                        Handles.Label(cell.worldPosition, cell.cost.ToString(), style);
                    }
                    break;

                case FlowFieldDisplayType.IntegrationField:
                    foreach (Cell cell in curFlowField.grid)
                    {
                        Handles.Label(cell.worldPosition, cell.bestCost.ToString(), style);
                    }
                    break;

                default:
                    break;
            }
        }
    }

    private void DrawGrid(Vector2Int drawGridSize, Color drawColor, float drawCellRadius)
    {
        ClearChild();
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

    public static void DrawFlowField()
    {
        ClearChild();

       foreach(Cell cell in curFlowField.grid)
        {
            DrawIcon(cell);
        }
    }
    public static void ClearChild()
    {
        for (int i = 0; i < instance.transform.childCount; i++)
        {
            Destroy(instance.transform.GetChild(i).gameObject);
        }
    }

    public static void DrawIcon(Cell cell)
    {
        GameObject icon = new GameObject();
        SpriteRenderer sr = icon.AddComponent<SpriteRenderer>();
        icon.transform.parent = instance.transform;
        icon.transform.position = cell.worldPosition + Vector3.up;

        if (cell.cost == 0)
        {
            sr.sprite = instance.sprites[0];
            Quaternion newRot = Quaternion.Euler(new Vector3(90, 0, 0));
            icon.transform.rotation = newRot;
        }
        else if(cell.cost == byte.MaxValue)
        {
            sr.sprite = instance.sprites[1];
            Quaternion newRot = Quaternion.Euler(new Vector3(90, 0, 0));
            icon.transform.rotation = newRot;
        }
        else if (cell.bestDirection == GridDirection.North)
        {
            sr.sprite = instance.sprites[2];
            Quaternion newRot = Quaternion.Euler(new Vector3(90, 0, 0));
            icon.transform.rotation = newRot;
        }
        else if(cell.bestDirection == GridDirection.NorthEast)
        {
            sr.sprite = instance.sprites[2];
            Quaternion newRot = Quaternion.Euler(new Vector3(90, 45f, 0));
            icon.transform.rotation = newRot;
        }
        else if (cell.bestDirection == GridDirection.East)
        {
            sr.sprite = instance.sprites[2];
            Quaternion newRot = Quaternion.Euler(new Vector3(90, 90f, 0));
            icon.transform.rotation = newRot;
        }
        else if (cell.bestDirection == GridDirection.SouthEast)
        {
            sr.sprite = instance.sprites[2];
            Quaternion newRot = Quaternion.Euler(new Vector3(90, 135f, 0));
            icon.transform.rotation = newRot;
        }
        else if (cell.bestDirection == GridDirection.South)
        {
            sr.sprite = instance.sprites[2];
            Quaternion newRot = Quaternion.Euler(new Vector3(90, 180f, 0));
            icon.transform.rotation = newRot;
        }
        else if (cell.bestDirection == GridDirection.SouthWest)
        {
            sr.sprite = instance.sprites[2];
            Quaternion newRot = Quaternion.Euler(new Vector3(90, 225f, 0));
            icon.transform.rotation = newRot;
        }
        else if (cell.bestDirection == GridDirection.West)
        {
            sr.sprite = instance.sprites[2];
            Quaternion newRot = Quaternion.Euler(new Vector3(90, 270f, 0));
            icon.transform.rotation = newRot;
        }
        else if (cell.bestDirection == GridDirection.NorthWest)
        {
            sr.sprite = instance.sprites[2];
            Quaternion newRot = Quaternion.Euler(new Vector3(90, 315f, 0));
            icon.transform.rotation = newRot;
        }
        
    }

}

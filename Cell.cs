using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell
{
    private Vector3 worldPosition;
    private Vector2Int gridPosition;

    public Cell(Vector3 _worldPos, Vector2Int _gridPos)
    {
        worldPosition = _worldPos;
        gridPosition = _gridPos;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell
{
    public Vector3 worldPosition;
    public Vector2Int gridPosition;
    public byte cost;
    public ushort bestCost;
    public GridDirection bestDirection;

    public Cell(Vector3 _worldPos, Vector2Int _gridPos)
    {
        worldPosition = _worldPos;
        gridPosition = _gridPos;
        cost = 1;
        bestCost = ushort.MaxValue;
        bestDirection = GridDirection.None;
    }

    public void IncreaseCost(int amount)
    {
        if(cost == byte.MaxValue) { return; }
        if(amount + cost >= 255) { cost = byte.MaxValue; }
        else { cost += (byte)amount; }
    }
}

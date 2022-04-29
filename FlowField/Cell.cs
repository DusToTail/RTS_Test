using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// DESCRIPTION: A cell is a basic unit in a flowfield structure, containing position, traversing cost, 
/// best cost to travel to a destination and the consequent best direction
/// </summary>
public class Cell
{
    
    
    public Vector3 worldPosition;
    public Vector2Int gridPosition;
    public byte cost;
    public ushort bestCost;
    public GridDirection bestDirection;
    public bool bestCostIsCalculated;

    /// <summary>
    /// Constructor of a cell in the grid used by flowfield. The flowfield is controlled by GridController
    /// cost is the cost for traversing on that cell
    /// bestCost is the cost to traverse from this cell to the destination (sum of the costs on the path)
    /// bestDirection is the direction pointing from this cell to the neighboring cell with the lowest bestCost
    /// </summary>
    /// <param name="_worldPos"></param>
    /// <param name="_gridPos"></param>
    public Cell(Vector3 _worldPos, Vector2Int _gridPos)
    {
        worldPosition = _worldPos;
        gridPosition = _gridPos;
        cost = 1;
        bestCost = ushort.MaxValue;
        bestDirection = GridDirection.None;
        bestCostIsCalculated = false;
    }

    /// <summary>
    /// Increase cost of a cell by an amount
    /// </summary>
    /// <param name="amount"></param>
    public void IncreaseCost(int amount)
    {
        if(cost == byte.MaxValue) { return; }
        if(amount + cost >= 255) { cost = byte.MaxValue; }
        else { cost += (byte)amount; }
    }
}

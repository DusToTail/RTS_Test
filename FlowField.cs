using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowField
{
    public Cell[,] grid { get; private set; }
    public Vector2Int gridSize { get; private set; }
    public float cellRadius { get; private set; }
    public Cell destinationCell;

    private float cellDiameter;
    private Vector3 cellHalfExtents;

    public FlowField(float _cellRadius, Vector2Int _gridSize)
    {
        cellRadius = _cellRadius;
        cellDiameter = cellRadius * 2f;
        cellHalfExtents = Vector3.one * cellRadius;
        gridSize = _gridSize;
    }

    public void CreateGrid()
    {
        grid = new Cell[gridSize.x, gridSize.y];
        for(int x = 0; x < gridSize.x; x++)
        {
            for(int y = 0; y < gridSize.y; y++)
            {
                Vector3 worldPos = new Vector3(cellDiameter * x + cellRadius, 0, cellDiameter * y + cellRadius);
                grid[x, y] = new Cell(worldPos, new Vector2Int(x, y));
            }
        }
    }

    public void CreateCostField()
    {
        LayerMask terrainMask = LayerMask.GetMask("Impassible", "RoughTerrain");
        foreach (Cell cell in grid)
        {
            Collider[] hits = Physics.OverlapBox(cell.worldPosition, cellHalfExtents, Quaternion.identity, terrainMask);
            bool hasIncreased = false;
            foreach (Collider hit in hits)
            {
                if(hit.gameObject.layer == LayerMask.NameToLayer("Impassible"))
                {
                    cell.IncreaseCost(255);
                    continue;
                }
                else if(!hasIncreased && hit.gameObject.layer == LayerMask.NameToLayer("RoughTerrain"))
                {
                    cell.IncreaseCost(3);
                    hasIncreased = true;
                }
            }
        }
    }

    public void CreateIntegrationField(Cell _destinationCell)
    {
        destinationCell = _destinationCell;
        destinationCell.cost = 0;
        destinationCell.bestCost = 0;

        Queue<Cell> cellsToCheck = new Queue<Cell>();

        cellsToCheck.Enqueue(destinationCell);

        while(cellsToCheck.Count > 0)
        {
            Cell curCell = cellsToCheck.Dequeue();
            List<Cell> curNeighbors = GetNeighborCells(curCell.gridPosition, GridDirection.CardinalDirections);
            foreach(Cell curNeighbor in curNeighbors)
            {
                if(curNeighbor.cost == byte.MaxValue) { continue; }
                if(curNeighbor.cost + curCell.bestCost < curNeighbor.bestCost)
                {
                    curNeighbor.bestCost = (ushort)(curNeighbor.cost + curCell.bestCost);
                    cellsToCheck.Enqueue(curNeighbor);
                }
            }
        }
    }

    public void CreateFlowField()
    {
        foreach(Cell curCell in grid)
        {
            List<Cell> curNeighbors = GetNeighborCells(curCell.gridPosition, GridDirection.AllDirections);

            int bestCost = curCell.bestCost;
            
            foreach(Cell curNeighbor in curNeighbors)
            {
                if(curNeighbor.bestCost < bestCost)
                {
                    bestCost = curNeighbor.bestCost;
                    curCell.bestDirection = GridDirection.GetDirectionFromV2I(curNeighbor.gridPosition - curCell.gridPosition);
                }
            }
        }
    }

    private List<Cell> GetNeighborCells(Vector2Int nodePosition, List<GridDirection> directions)
    {
        List<Cell> neighborCells = new List<Cell>();

        foreach(Vector2Int curDirection in directions)
        {
            Cell newNeighbor = GetCellAtRelativePos(nodePosition, curDirection);
            if (newNeighbor != null)
                neighborCells.Add(newNeighbor);
        }
        return neighborCells;
    }

    private Cell GetCellAtRelativePos(Vector2Int originPos, Vector2Int relativePos)
    {
        Vector2Int finalPos = originPos + relativePos;

        if(finalPos.x < 0 || finalPos.x >= gridSize.x || finalPos.y < 0 || finalPos.y >= gridSize.y)
        {
            return null;
        }
        else { return grid[finalPos.x, finalPos.y]; }
    }

    public Cell GetCellFromWorldPos(Vector3 worldPos)
    {
        float percentX = worldPos.x / (gridSize.x * cellDiameter);
        float percentY = worldPos.z / (gridSize.y * cellDiameter);

        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        int x = Mathf.Clamp(Mathf.FloorToInt(gridSize.x * percentX), 0, gridSize.x - 1);
        int y = Mathf.Clamp(Mathf.FloorToInt(gridSize.y * percentY), 0, gridSize.y - 1);
        return grid[x, y];

    }



}

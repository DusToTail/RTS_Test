using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowField
{
    /// <summary>
    /// DESCRIPTION: Flow Field is a structure that provide the information of the best cost to take and the consequent direction 
    /// towards the destination on the grid for each cell.
    /// Declared and Initialized in SelectGroup ( ONE flow field for MANY actions), which is created as a game object in SelectSystem
    /// </summary>
    
    public Cell[,] grid { get; private set; }
    public Vector2Int gridSize { get; private set; }
    public float cellRadius { get; private set; }
    public Cell destinationCell;


    private float cellDiameter;
    private Vector3 cellHalfExtents;


    /// <summary>
    /// Constructor of a flowfield with specified cell radius and grid size
    /// </summary>
    /// <param name="_cellRadius"></param>
    /// <param name="_gridSize"></param>
    public FlowField(float _cellRadius, Vector2Int _gridSize)
    {
        cellRadius = _cellRadius;
        cellDiameter = cellRadius * 2f;
        cellHalfExtents = Vector3.one * cellRadius;
        gridSize = _gridSize;
    }

    /// <summary>
    /// Create a grid that has the origin {0,0} at bottom left. Therefore, there is no negative index/position in the grid.
    /// </summary>
    public void CreateGrid()
    {
        grid = new Cell[gridSize.x, gridSize.y];
        for(int x = 0; x < gridSize.x; x++)
        {
            for(int y = 0; y < gridSize.y; y++)
            {
                //An offset of cellRadius is applied to both axis to provide world position right at the center of each cell
                
                Vector3 worldPos = new Vector3(cellDiameter * x + cellRadius, 0, cellDiameter * y + cellRadius);
                grid[x, y] = new Cell(worldPos, new Vector2Int(x, y));
            }
        }
    }

    /// <summary>
    /// Create a costfield using the grid created. 
    /// A costfield update the information about the traversing cost to each cell according to the layer defined.
    /// </summary>
    public void CreateCostField()
    {
        LayerMask terrainMask = LayerMask.GetMask(Tags.Impassible_Terrain, Tags.Rough_Terrain);
        foreach (Cell cell in grid)
        {
            //Check for colliders that identify the property of the terrain at the cell's world position. Here is Impassible OR Rough
            //Additional bool var is added to ensure that each cell's traversing cost only increase ONCE (there can be multiple colliders slightly overlapping)
            Collider[] hits = Physics.OverlapBox(cell.worldPosition, cellHalfExtents, Quaternion.identity, terrainMask);
            bool hasIncreased = false;
            foreach (Collider hit in hits)
            {
                if(hit.gameObject.layer == LayerMask.NameToLayer(Tags.Impassible_Terrain))
                {
                    cell.IncreaseCost(255);
                    continue;
                }
                else if(!hasIncreased && hit.gameObject.layer == LayerMask.NameToLayer(Tags.Rough_Terrain))
                {
                    cell.IncreaseCost(3);
                    hasIncreased = true;
                }
            }
        }
    }

    /// <summary>
    /// Create an integration field with a specifed destination (per click / command of one or a group of entities)
    /// An integration field will have calculated all cell's best cost to traverse to the destination, by summing up the cost of the cells on the way
    /// </summary>
    /// <param name="_destinationCell"></param>
    public void CreateIntegrationField(Cell _destinationCell)
    {
        //As at the destination cell, the destination is reached, thus its cost and best cost is 0
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

    /// <summary>
    /// Create a flow field that determines each cell's direction from itself to the neighboring cell with the lowest best cost
    /// </summary>
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

    /// <summary>
    /// Get a list of neighboring cells basing on the directions from the specified position
    /// </summary>
    /// <param name="nodePosition"></param>
    /// <param name="directions"></param>
    /// <returns></returns>
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

    /// <summary>
    /// Get a neightboring cell basing on the direction from the specified position
    /// </summary>
    /// <param name="originPos"></param>
    /// <param name="relativePos"></param>
    /// <returns></returns>
    private Cell GetCellAtRelativePos(Vector2Int originPos, Vector2Int relativePos)
    {
        Vector2Int finalPos = originPos + relativePos;

        if(finalPos.x < 0 || finalPos.x >= gridSize.x || finalPos.y < 0 || finalPos.y >= gridSize.y)
        {
            return null;
        }
        else { return grid[finalPos.x, finalPos.y]; }
    }

    /// <summary>
    /// Return the world position of a cell from the grid
    /// Note: this grid system only work of positions with positive values on each axis.
    /// </summary>
    /// <param name="worldPos"></param>
    /// <returns></returns>
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

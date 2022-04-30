using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// DESCRIPTION: Flow Field is a structure that provide the information of the best cost to take and the consequent direction 
/// towards the destination on the grid for each cell.
/// Declared and Initialized in SelectGroup ( ONE flow field for MANY actions), which is created as a game object in SelectSystem
/// </summary>
public class FlowField
{
    
    
    public Cell[,] grid { get; private set; }
    public Vector2Int gridSize { get; private set; }
    public float cellRadius { get; private set; }
    public Cell destinationCell;
    public Vector3 startPosition;
    public Vector2 moveDirection;

    private float cellDiameter;
    private Vector3 cellHalfExtents;

    /// <summary>
    /// Constructor of a flowfield being centered at the specified position
    /// </summary>
    /// <param name="_centerPosition"></param>
    /// <param name="_cellRadius"></param>
    /// <param name="_gridSize"></param>
    public FlowField(Vector3 _centerPosition, float _cellRadius, Vector2Int _gridSize)
    {
        Debug.Log("FlowField constructed at center of " + _centerPosition);
        cellRadius = _cellRadius;
        cellDiameter = cellRadius * 2f;
        cellHalfExtents = Vector3.one * cellRadius;
        gridSize = _gridSize;

        moveDirection = Vector2.one;
        startPosition = _centerPosition - new Vector3(gridSize.x, 0, gridSize.y) * cellRadius;
    }

    /// <summary>
    /// Constructor of a flowfield at a specific position with a specific direction
    /// </summary>
    /// <param name="_cellRadius"></param>
    /// <param name="_gridSize"></param>
    public FlowField(Vector3 _curPosition, Vector2 _moveDir, float _cellRadius, Vector2Int _gridSize)
    {
        Debug.Log("FlowField constructed at " + _curPosition + " with direction " + _moveDir.normalized);
        cellRadius = _cellRadius;
        cellDiameter = cellRadius * 2f;
        cellHalfExtents = Vector3.one * cellRadius;
        gridSize = _gridSize;

        moveDirection = _moveDir.normalized;
        /*
        if(Vector2.Dot(moveDirection, Vector2.left) > 0.8f)
        {
            if(moveDirection.y < 0)
            {
                startPosition = _curPosition + Vector3.forward * cellDiameter * gridSize.y / 2;
            }
            else
            {
                startPosition = _curPosition - Vector3.forward * cellDiameter * gridSize.y / 2;
            }
            startPosition += Vector3.right * cellDiameter * gridSize.x / 10;
        }
        else if(Vector2.Dot(moveDirection, Vector2.right) > 0.8f)
        {
            if (moveDirection.y < 0)
            {
                startPosition = _curPosition + Vector3.forward * cellDiameter * gridSize.y / 2;
            }
            else
            {
                startPosition = _curPosition + Vector3.forward * cellDiameter * gridSize.y / 2;
            }
            startPosition += Vector3.left * cellDiameter * gridSize.x / 10;
        }
        else if(Vector2.Dot(moveDirection, Vector2.up) > 0.8f)
        {
            if (moveDirection.x < 0)
            {
                startPosition = _curPosition + Vector3.right * cellDiameter * gridSize.x / 2;
            }
            else
            {
                startPosition = _curPosition - Vector3.right * cellDiameter * gridSize.x / 2;
            }
            startPosition += Vector3.back * cellDiameter * gridSize.y / 10;
        }
        else if (Vector2.Dot(moveDirection, Vector2.down) > 0.8f)
        {
            if (moveDirection.x < 0)
            {
                startPosition = _curPosition + Vector3.right * cellDiameter * gridSize.x / 2;
            }
            else
            {
                startPosition = _curPosition - Vector3.right * cellDiameter * gridSize.x / 2;
            }
            startPosition += Vector3.forward * cellDiameter * gridSize.y / 10;
        }
        else { startPosition = _curPosition; }
        */
        startPosition = _curPosition;

    }

    /// <summary>
    /// Create a grid with direction considered
    /// </summary>
    public void CreateGrid()
    {
        grid = new Cell[gridSize.x, gridSize.y];
        for(int x = 0; x < gridSize.x; x++)
        {
            for(int y = 0; y < gridSize.y; y++)
            {
                //An offset of cellRadius is applied to both axis to provide world position right at the center of each cell
                
                Vector3 worldPos = new Vector3((cellDiameter * x + cellRadius) * Mathf.Sign(moveDirection.x), 0, (cellDiameter * y + cellRadius) * Mathf.Sign(moveDirection.y)) + startPosition;
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
        LayerMask terrainMask = LayerMask.GetMask(Tags.Impassible_Terrain, Tags.Rough_Terrain, Tags.Selectable, Tags.Obstacle);
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
                else if (hit.gameObject.layer == LayerMask.NameToLayer(Tags.Obstacle))
                {
                    cell.IncreaseCost(255);
                    break;
                }
                else if (hit.gameObject.layer == LayerMask.NameToLayer(Tags.Selectable))
                {
                    if (hit.gameObject.GetComponent<IEntity>().GetEntityType() == IEntity.EntityType.Structure)
                    {
                        cell.IncreaseCost(255);
                        break;
                    }
                }
                else if (!hasIncreased && hit.gameObject.layer == LayerMask.NameToLayer(Tags.Rough_Terrain))
                {
                    cell.IncreaseCost(3);
                    hasIncreased = true;
                }
            }
        }
    }

    /// <summary>
    /// Create an integration field with a specifed destination
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
                if(curNeighbor.cost == byte.MaxValue && curNeighbor.bestCostIsCalculated == false)
                {
                    curNeighbor.bestCostIsCalculated = true;
                }
                if(curNeighbor.cost + curCell.bestCost < curNeighbor.bestCost && curNeighbor.bestCostIsCalculated == false)
                {
                    curNeighbor.bestCost = (ushort)(curNeighbor.cost + curCell.bestCost);
                    curNeighbor.bestCostIsCalculated = true;
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
                    Vector3 dirV3 = (curNeighbor.worldPosition - curCell.worldPosition) / cellDiameter;
                    Vector2Int dir = new Vector2Int(Mathf.FloorToInt(dirV3.x), Mathf.FloorToInt(dirV3.z));
                    curCell.bestDirection = GridDirection.GetDirectionFromV2I(dir);
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
        float percentX = Mathf.Abs(worldPos.x - startPosition.x) / (gridSize.x * cellDiameter);
        float percentY = Mathf.Abs(worldPos.z - startPosition.z) / (gridSize.y * cellDiameter);

        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        int x = Mathf.Clamp(Mathf.FloorToInt(gridSize.x * percentX), 0, gridSize.x - 1);
        int y = Mathf.Clamp(Mathf.FloorToInt(gridSize.y * percentY), 0, gridSize.y - 1);
        return grid[x, y];

    }

}

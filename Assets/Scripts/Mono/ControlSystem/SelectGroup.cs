using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectGroup : MonoBehaviour
{
    /// <summary>
    /// DESCRIPTION: A select group is a game object that connects ALL the actions declared by SelectSystem and ONE Flow Field.
    /// All Actions will have reference to ONE SINGLE Flow Field (to reduce memory usage) when navigating towards the destination
    /// Select group will have reference to ALL Actions and related Units using its Flow Field to check if they are all finished to self-destruct
    /// </summary>
    public FlowField groupFlowField { get; private set; }

    public List<Action> actionList { get; private set; }

    public List<Unit> unitList { get; private set; }

    //public Vector3 centerPosition { get; private set; }
    //public Vector3 frontPosition { get; private set; }
    //public Vector3 backPosition { get; private set; }

    //public Unit frontUnit { get; private set; }
    //public Unit backUnit { get; private set; }

    private bool flowFieldReset;

    private void Awake()
    {
        actionList = new List<Action>();
        unitList = new List<Unit>();
        groupFlowField = new FlowField(FindObjectOfType<GridController>().cellRadius, FindObjectOfType<GridController>().gridSize);
        groupFlowField.CreateGrid();
        groupFlowField.CreateCostField();

        flowFieldReset = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (CheckIfAllActionsIsFinished() == true)
            Destroy(this.gameObject);

        //UpdateRelativePositionAndUnit();

        foreach(Unit curUnit in unitList)
        {
            if(CheckIfUnitCollidesWithObstacle(curUnit) && flowFieldReset == true)
            {
                Debug.Log("FlowField Changed");
                flowFieldReset = false;
                InitializeGrid();
                InitializeCostField();
                InitializeIntegrationField(groupFlowField.destinationCell.worldPosition);
                InitializeFlowField();
                flowFieldReset = true;

                GridDebug.SetCurFlowField(groupFlowField);
            }
        }

    }

    /// <summary>
    /// Add ONE Action of ONE Unit to the list. Called in SelectSystem
    /// </summary>
    /// <param name="_action"></param>
    public void AddToActionList(Action _action)
    {
        if(_action == null) { return; }
        actionList.Add(_action);
    }

    public void AddToUnitList(Unit _unit)
    {
        if(_unit == null) { return; }
        unitList.Add(_unit);
    }

    /// <summary>
    /// Returns false if AT LEAST ONE Action is not finished. If not, true
    /// </summary>
    /// <returns></returns>
    private bool CheckIfAllActionsIsFinished()
    {
        for (int index = 0; index < actionList.Count; index++)
        {
            //Debug.Log("Action index " + index + " isFinished: " + actionList[index].isFinished);
            if (actionList[index].isFinished == false)
            {
                return false;
            }
        }
        return true;
    }

    //private void UpdateRelativePositionAndUnit()
    //{
    //    centerPosition = Vector3.zero;
    //    frontPosition = groupFlowField.destinationCell.worldPosition;
    //    float closestDistance = float.MaxValue;
    //    float furthestDistance = float.MinValue;

    //    if (unitList.Count == 0) { return; }
    //    for (int index = 0; index < unitList.Count; index++)
    //    {
    //        if(actionList[index].isFinished == true) { continue; }
    //        centerPosition += unitList[index].gameObject.GetComponent<Rigidbody>().position;

    //        float curDistance = (unitList[index].gameObject.GetComponent<Rigidbody>().position - groupFlowField.destinationCell.worldPosition).magnitude;
    //        if (curDistance < closestDistance)
    //        {
    //            frontPosition = unitList[index].gameObject.GetComponent<Rigidbody>().position;
    //            frontUnit = unitList[index];
    //            closestDistance = curDistance;
    //        }

    //        if(curDistance > furthestDistance)
    //        {
    //            backPosition = unitList[index].gameObject.GetComponent<Rigidbody>().position;
    //            backUnit = unitList[index];
    //            furthestDistance = curDistance;
    //        }
    //    }

    //    centerPosition = centerPosition / unitList.Count;
    //}

    //public bool CenterSameCellWithDestination()
    //{
    //    if(unitList.Count == 0) { return false; }

    //    if (groupFlowField.GetCellFromWorldPos(centerPosition) == groupFlowField.destinationCell)
    //        return true;
    //    else
    //        return false;
    //}

    //public Vector3 GetCenterToFrontVector()
    //{
    //    return (frontPosition - centerPosition);
    //}
    //public Vector3 GetCenterToBackVector()
    //{
    //    return (backPosition - centerPosition);
    //}
    //public Vector3 GetCenterToDestinationVector()
    //{
    //    return (groupFlowField.destinationCell.worldPosition - centerPosition);
    //}
    //public Vector3 GetFrontToDestinationVector()
    //{
    //    return (groupFlowField.destinationCell.worldPosition - frontPosition);
    //}
    //public Vector3 GetBackToDestinationVector()
    //{
    //    return (groupFlowField.destinationCell.worldPosition - backPosition);
    //}

    
    /// <summary>
    /// Initialize the Flow Field. Called in SelectSystem, using mousePosition on the Grid to find the cell as parameter
    /// </summary>
    /// <param name="_destinationPos"></param>
    public void InitializeFlowField()
    {
        groupFlowField.CreateFlowField();
    }
    public void InitializeIntegrationField(Vector3 _destinationPos)
    {
        Cell destinationCell = groupFlowField.GetCellFromWorldPos(_destinationPos);

        groupFlowField.CreateIntegrationField(destinationCell);
    }
    public void InitializeCostField()
    {
        groupFlowField.CreateCostField();
    }
    public void InitializeGrid()
    {
        groupFlowField.CreateGrid();
    }

    /// <summary>
    /// Return true if the front of the unit contains an unexpected obstacle compared with the group grid.
    /// Comparison is through cost.
    /// </summary>
    /// <param name="unit"></param>
    /// <returns></returns>
    private bool CheckIfUnitCollidesWithObstacle(Unit unit)
    {
        if(unit == null) { return false; }

        //Check for colliders that identify the property of the terrain at the cell's world position. Here is Impassible OR Rough
        //Additional bool var is added to ensure that each cell's traversing cost only increase ONCE (there can be multiple colliders slightly overlapping)
        Vector3 forwardPosition = unit.GetComponent<Rigidbody>().position + unit.transform.forward * groupFlowField.cellRadius;
        LayerMask terrainMask = LayerMask.GetMask(Tags.Impassible_Terrain, Tags.Rough_Terrain);
        Vector3 cellHalfExtents = Vector3.one * groupFlowField.cellRadius;

        Cell cell = new Cell(groupFlowField.GetCellFromWorldPos(forwardPosition).worldPosition, groupFlowField.GetCellFromWorldPos(forwardPosition).gridPosition);

        Collider[] hits = Physics.OverlapBox(cell.worldPosition, cellHalfExtents, Quaternion.identity, terrainMask);
        foreach (Collider hit in hits)
        {
            if (hit.gameObject.layer == LayerMask.NameToLayer(Tags.Impassible_Terrain))
            {
                cell.IncreaseCost(255);
                break;
            }
            else if (hit.gameObject.layer == LayerMask.NameToLayer(Tags.Rough_Terrain))
            {
                cell.IncreaseCost(3);
                break;
            }
        }
        
        if (cell.cost != groupFlowField.grid[cell.gridPosition.x, cell.gridPosition.y].cost &&
            cell.gridPosition != groupFlowField.destinationCell.gridPosition)
        {
            Debug.DrawLine(cell.worldPosition, cell.worldPosition + Vector3.up * 100, Color.yellow);
            return true;
        }

        return false;
    }











}

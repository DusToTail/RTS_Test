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
    /// 
   
    public List<FlowField> flowFieldList;
    public FlowField groupFlowField { get; private set; }
    public IEntity groupTarget { get; private set; }

    public List<IAction> actionList { get; private set; }

    public List<IEntity> entityList { get; private set; }

    private bool flowFieldReset;

    private void Awake()
    {
        flowFieldList = new List<FlowField>();
        actionList = new List<IAction>();
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

        for(int index = 0; index < entityList.Count; index++)
        {
            if(entityList[index] is IUnit unitEntity)
            {
                if (CheckIfUnitCollidesWithObstacle(unitEntity, index) && flowFieldReset == true)
                {
                    Debug.Log("FlowField Changed");
                    flowFieldReset = false;
                    groupFlowField = ReInitializeFlowField(groupFlowField);

                    for (int i = 0; i < flowFieldList.Count; i++)
                    {
                        flowFieldList[i] = ReInitializeFlowField(flowFieldList[i]);
                    }

                    for (int i = 0; i < actionList.Count; i++)
                    {
                        if (actionList[i].GetSelf().type != actionList[i].GetSelf().Type.MoveTowards)
                        {
                            actionList[i].GetSelf().selfFlowField = ReInitializeFlowField(actionList[i].GetSelf().selfFlowField);
                        }

                        actionList[i].GetSelf().curFlowField = ReInitializeFlowField(actionList[i].GetSelf().curFlowField);
                    }
                }
            }
        }

        // Update Flow Field Towards the target ONLY when the group is attacking a SPECIFIC target
        if(groupTarget == null) { return; }
        for(int index = 0; index < entityList.Count; index++)
        {
            if(entityList[index] == null) { continue; }

            groupFlowField = ReInitializeFlowFieldWithTarget(groupFlowField, groupTarget);
            break;
        }
        


    }

    /// <summary>
    /// Add ONE Action of ONE Unit to the list. Called in SelectSystem
    /// </summary>
    /// <param name="_action"></param>
    public void AddToActionList(IAction _action)
    {
        if(_action == null) { return; }
        actionList.Add(_action);
    }

    public void AddToEntityList(IEntity _entity)
    {
        if(_entity == null) { return;}
        entityList.Add(_entity);
    }
    public void AddToFlowFieldList(FlowField _flowField)
    {
        if(_flowField == null) { return; }
        flowFieldList.Add(_flowField);
    }

    public void AssignGroupTarget(IEntity _target)
    {
        groupTarget = _target;
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

    
    /// <summary>
    /// Initialize the Flow Field. Called in SelectSystem, using mousePosition on the Grid to find the cell as parameter
    /// </summary>
    /// <param name="_destinationPos"></param>
    public void InitializeCurrentFlowField()
    {
        groupFlowField.CreateFlowField();
    }
    public void InitializeCurrentIntegrationField(Vector3 _destinationPos)
    {
        Cell destinationCell = groupFlowField.GetCellFromWorldPos(_destinationPos);

        groupFlowField.CreateIntegrationField(destinationCell);
    }
    public void InitializeCurrentCostField()
    {
        groupFlowField.CreateCostField();
    }
    public void InitializeCurrentGrid()
    {
        groupFlowField.CreateGrid();
    }
    public FlowField ReInitializeFlowField(FlowField _flowField)
    {
        if(_flowField == null) { Debug.Log("Flow Field Ref is null"); return null; }

        FlowField newFlowField = new FlowField(_flowField.cellRadius, _flowField.gridSize);
        newFlowField.CreateGrid();
        newFlowField.CreateCostField();
        Cell destinationCell = newFlowField.GetCellFromWorldPos(_flowField.destinationCell.worldPosition);
        newFlowField.CreateIntegrationField(destinationCell);
        newFlowField.CreateFlowField();
        return newFlowField;
    }
    public FlowField ReInitializeFlowFieldWithTarget(FlowField _flowField, IEntity _target)
    {
        if (_flowField == null) { Debug.Log("Flow Field Ref is null"); return null; }
        if(_target == null) { Debug.Log("Target is null"); return null;}

        FlowField newFlowField = new FlowField(_flowField.cellRadius, _flowField.gridSize);
        newFlowField.CreateGrid();
        newFlowField.CreateCostField();
        Cell destinationCell = newFlowField.GetCellFromWorldPos(_target.GetWorldPosition());
        newFlowField.CreateIntegrationField(destinationCell);
        newFlowField.CreateFlowField();
        return newFlowField;
    }

    // FlowField List for Patrolling and other complex movement
    public void AddNewFlowFieldToList(Vector3 _destinationPosition)
    {
        FlowField newFlowField = new FlowField(FindObjectOfType<GridController>().cellRadius, FindObjectOfType<GridController>().gridSize);
        newFlowField.CreateGrid();
        newFlowField.CreateCostField();
        newFlowField.CreateIntegrationField(newFlowField.GetCellFromWorldPos(_destinationPosition));
        newFlowField.CreateFlowField();
        flowFieldList.Add(newFlowField);
    }

    /// <summary>
    /// Return true if the front of the unit contains an unexpected obstacle compared with the group grid.
    /// Comparison is through cost.
    /// </summary>
    /// <param name="_unit"></param>
    /// <returns></returns>
    private bool CheckIfUnitCollidesWithObstacle(IUnit _unit, int _unitIndex)
    {
        if(_unit == null) { return false; }

        //Check for colliders that identify the property of the terrain at the cell's world position. Here is Impassible OR Rough
        //Additional bool var is added to ensure that each cell's traversing cost only increase ONCE (there can be multiple colliders slightly overlapping)
        Vector3 forwardPosition = _unit.GetWorldPosition() + _unit.GetTransform().forward * actionList[_unitIndex].GetSelf().curFlowField.cellRadius;
        LayerMask terrainMask = LayerMask.GetMask(Tags.Impassible_Terrain, Tags.Rough_Terrain, Tags.Selectable);
        Vector3 cellHalfExtents = Vector3.one * actionList[_unitIndex].GetSelf().curFlowField.cellRadius;

        Cell cell = new Cell(actionList[_unitIndex].GetSelf().curFlowField.GetCellFromWorldPos(forwardPosition).worldPosition, actionList[_unitIndex].GetSelf().curFlowField.GetCellFromWorldPos(forwardPosition).gridPosition);

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
            else if (hit.gameObject.layer == LayerMask.NameToLayer(Tags.Selectable))
            {
                if(hit.gameObject.GetComponent<IEntity>() is IStructure)
                {
                    cell.IncreaseCost(255);
                    break;
                }
            }
        }
        
        if (cell.cost != actionList[_unitIndex].GetSelf().curFlowField.grid[cell.gridPosition.x, cell.gridPosition.y].cost &&
            cell.gridPosition != actionList[_unitIndex].GetSelf().curFlowField.destinationCell.gridPosition)
        {
            Debug.DrawLine(cell.worldPosition, cell.worldPosition + Vector3.up * 100, Color.yellow);
            return true;
        }

        return false;
    }











}

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

    public Vector3 centerPosition { get; private set; }
    public Vector3 frontPosition { get; private set; }

    private void Awake()
    {
        actionList = new List<Action>();
        unitList = new List<Unit>();
        groupFlowField = new FlowField(FindObjectOfType<GridController>().cellRadius, FindObjectOfType<GridController>().gridSize);
        groupFlowField.CreateGrid();
        groupFlowField.CreateCostField();
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

        UpdateRelativePosition();

    }

    /// <summary>
    /// Initialize the Flow Field. Called in SelectSystem, using mousePosition on the Grid to find the cell as parameter
    /// </summary>
    /// <param name="_destinationPos"></param>
    public void InitializeFlowField(Vector3 _destinationPos)
    {
        Cell destinationCell = groupFlowField.GetCellFromWorldPos(_destinationPos);

        groupFlowField.CreateIntegrationField(destinationCell);
        groupFlowField.CreateFlowField();
    }

    /// <summary>
    /// Add ONE Action of ONE Unit to the list. Called in SelectSystem
    /// </summary>
    /// <param name="_action"></param>
    public void AddToActionList(ref Action _action)
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

    private void UpdateRelativePosition()
    {
        centerPosition = Vector3.zero;
        frontPosition = groupFlowField.destinationCell.worldPosition;
        float closestDistance = float.MaxValue;

        if (unitList.Count == 0) { return; }
        for (int index = 0; index < unitList.Count; index++)
        {
            if(actionList[index].isFinished == true) { continue; }
            centerPosition += unitList[index].gameObject.GetComponent<Rigidbody>().position;

            float curDistance = (unitList[index].gameObject.GetComponent<Rigidbody>().position - groupFlowField.destinationCell.worldPosition).magnitude;
            if (curDistance < closestDistance)
            {
                frontPosition = unitList[index].gameObject.GetComponent<Rigidbody>().position;
                closestDistance = curDistance;
            }
        }

        centerPosition = centerPosition / unitList.Count;
    }

    public bool CenterSameCellWithDestination()
    {
        if(unitList.Count == 0) { return false; }

        if (groupFlowField.GetCellFromWorldPos(centerPosition) == groupFlowField.destinationCell)
            return true;
        else
            return false;
    }

    public Vector3 GetCenterToFrontDirection()
    {
        return (frontPosition - centerPosition).normalized;
    }

    public Vector3 GetCenterToDestinationDirection()
    {
        return (groupFlowField.destinationCell.worldPosition - centerPosition).normalized;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(frontPosition + Vector3.up * 2, centerPosition + Vector3.up * 2);
    }
}

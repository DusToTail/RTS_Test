using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectGroup : MonoBehaviour
{
    /// <summary>
    /// DESCRIPTION: A select group is a game object that connects ALL the actions declared by SelectSystem and ONE Flow Field.
    /// All Actions will have reference to ONE SINGLE Flow Field (to reduce memory usage) when navigating towards the destination
    /// Select group will have reference to ALL Actions using its Flow Field to check if they are all finished to self-destruct
    /// </summary>
    public FlowField groupFlowField { get; private set; }

    private List<Action> actionList;

    private void Awake()
    {
        actionList = new List<Action>();
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
        actionList.Add(_action);
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
}

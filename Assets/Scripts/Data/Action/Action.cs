using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action
{
    /// <summary>
    /// DESCRIPTION: An Action is used by ActionController (component of a Unit gameObject) to determine what to do.
    /// An action is unique for each unit and for each declaration.
    /// </summary>
    
    //Different actions can be taken. Used in ActionController's [CommitCurrentAction] method as an argument in switch statement
    public enum ActionTypes
    {
        MoveTowards,
        StopAction,
        None
    }

    public ActionTypes type;

    //To determine if the action is finished (if move, then destination is reached).
    //Used in SelectGroup to see if all refering actions is finished to self-destruct.
    //Used in ActionController component to see if it should Dequeue the next action or not.
    public bool isFinished;

    public Vector3 mousePosition;

    //The select group (containing a flow field) to navigate with
    public GameObject selectGroup;

    public Action(ActionTypes _type)
    {
        type = _type;
        isFinished = false;
        mousePosition = Vector3.zero;
        selectGroup = null;
    }

    /// <summary>
    /// MOVE Action: move according to the flowfield in SelectGroup by changing the rigidbody attached
    /// Executed by ActionController
    /// </summary>
    /// <param name="rb"></param>
    /// <param name="flowField"></param>
    /// <param name="movementSpeed"></param>
    public void MoveInFlowField(ref Rigidbody rb, FlowField flowField, float movementSpeed)
    {
        //Validity Check
        if(flowField == null) { return; }
        if(isFinished == true) { return; }

        //Goal Check
        if (flowField.GetCellFromWorldPos(rb.position) == flowField.destinationCell) 
        {
            //Debug.Log("Action is finished");
            StopMoving(ref rb);
            FinishAction();
            return; 
        }

        //Move according to the flowfield
        int x = flowField.GetCellFromWorldPos(rb.gameObject.transform.position).bestDirection.Vector.x;
        int z = flowField.GetCellFromWorldPos(rb.gameObject.transform.position).bestDirection.Vector.y;
        Vector3 dir = new Vector3(x, 0, z);
        rb.velocity = dir.normalized * movementSpeed;
    }


    public void FinishAction() { isFinished = true; }

    public void StopMoving(ref Rigidbody rb) { rb.velocity = Vector3.zero; }

    public void InitializeMousePosition(Vector3 position) { mousePosition = position; }

    public void InitializeSelectGroupObject(ref GameObject _selectGroup) { selectGroup = _selectGroup; }

}

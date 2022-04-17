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
    /// MOVE Action: move according to the flowfield and flocking (alignment + separation) with others in SelectGroup by changing the rigidbody attached
    /// Executed by ActionController
    /// </summary>
    /// <param name="rb"></param>
    /// <param name="flowField"></param>
    /// <param name="movementSpeed"></param>
    public void MoveInFlowField(ref Rigidbody rb, float movementSpeed)
    {
        // Goal Check
        if (selectGroup.GetComponent<SelectGroup>().groupFlowField.GetCellFromWorldPos(rb.position) == selectGroup.GetComponent<SelectGroup>().groupFlowField.destinationCell)
        {
            //Debug.Log("Action is finished");
            StopMoving(ref rb);
            FinishAction();
            return;
        }

        // Move with Rigidbody
        Vector3 flowFieldVector = GetFlowFieldVector(rb.position) * GetFlowFieldWeight(rb.position);
        Vector3 alignmentVector = GetAlignmentVector(rb.position, 1f, 3f) * GetAlignmentWeight(rb.position, 1f);
        Vector3 separationVector = GetSeparationVector(rb.position, 1f, 3f) * GetSeparationWeight(rb.position, 1f);


        Vector3 moveDir = flowFieldVector + alignmentVector + separationVector;
        // rb.velocity cause other unit's transform to change slightly. Reason unknown
        // rb.velocity = moveDir.normalized * movementSpeed;
        rb.MovePosition(rb.position + moveDir.normalized * movementSpeed * Time.deltaTime);
        rb.MoveRotation(Quaternion.LookRotation(moveDir.normalized));

    }

    private Vector3 GetFlowFieldVector(Vector3 curWorldPos)
    {
        // Validity Check
        if (selectGroup.GetComponent<SelectGroup>().groupFlowField == null) { return Vector3.zero; }
        if (isFinished == true) { return Vector3.zero; }

        // Goal Check
        if (selectGroup.GetComponent<SelectGroup>().groupFlowField.GetCellFromWorldPos(curWorldPos) == 
            selectGroup.GetComponent<SelectGroup>().groupFlowField.destinationCell)
        {
            return Vector3.zero;
        }

        // Calculate direction from flowfield
        int x = selectGroup.GetComponent<SelectGroup>().groupFlowField.GetCellFromWorldPos(curWorldPos).bestDirection.Vector.x;
        int z = selectGroup.GetComponent<SelectGroup>().groupFlowField.GetCellFromWorldPos(curWorldPos).bestDirection.Vector.y;
        Vector3 flowFieldDir = new Vector3(x, 0, z);
        return flowFieldDir.normalized;
    }

    private float GetFlowFieldWeight(Vector3 curWorldPos)
    {
        float weight = 1;
        weight = Mathf.Clamp01((selectGroup.GetComponent<SelectGroup>().groupFlowField.destinationCell.worldPosition - curWorldPos).magnitude);

        return weight;
    }

    private Vector3 GetAlignmentVector(Vector3 curWorldPos, float minDistance, float maxDistance)
    {
        // Validity Check
        if (selectGroup.GetComponent<SelectGroup>().groupFlowField == null) { return Vector3.zero; }
        if (isFinished == true) { return Vector3.zero; }

        Vector3 alignmentDir = new Vector3(0, 0, 0);

        int excludeCount = 0;
        for(int index = 0; index < selectGroup.GetComponent<SelectGroup>().unitList.Count; index++)
        {
            if(selectGroup.GetComponent<SelectGroup>().actionList[index].isFinished == true) { excludeCount++; continue; }

            Vector3 curUnitPos = selectGroup.GetComponent<SelectGroup>().unitList[index].gameObject.GetComponent<Rigidbody>().position;
            Vector3 flowFieldVector = GetFlowFieldVector(curUnitPos);
            float distance = Vector3.Distance(curWorldPos, curUnitPos);
            float weight = ((maxDistance - Mathf.Clamp(distance, minDistance, maxDistance))) / (maxDistance - minDistance);
            alignmentDir += flowFieldVector * weight;
        }
        alignmentDir = alignmentDir / (selectGroup.GetComponent<SelectGroup>().unitList.Count - excludeCount);

        return alignmentDir.normalized;
    }

    private float GetAlignmentWeight(Vector3 curWorldPos, float minDistance)
    {
        float weight = 1;
        for (int index = 0; index < selectGroup.GetComponent<SelectGroup>().unitList.Count; index++)
        {
            Vector3 curUnitPos = selectGroup.GetComponent<SelectGroup>().unitList[index].gameObject.GetComponent<Rigidbody>().position;
            if ((curUnitPos - curWorldPos).magnitude < minDistance)
                weight ++;
        }

        return weight;
    }

    private Vector3 GetSeparationVector(Vector3 curWorldPos, float minDistance, float maxDistance)
    {
        // Validity Check
        if (selectGroup.GetComponent<SelectGroup>().groupFlowField == null) { return Vector3.zero; }
        if (isFinished == true) { return Vector3.zero; }

        Vector3 separationDir = new Vector3(0, 0, 0);

        int excludeCount = 0;
        for (int index = 0; index < selectGroup.GetComponent<SelectGroup>().unitList.Count; index++)
        {
            if (selectGroup.GetComponent<SelectGroup>().actionList[index].isFinished == true) { excludeCount++; continue; }

            Vector3 curUnitPos = selectGroup.GetComponent<SelectGroup>().unitList[index].gameObject.GetComponent<Rigidbody>().position;
            Vector3 separationVector = curWorldPos - curUnitPos;
            float weight = ((maxDistance - Mathf.Clamp(separationVector.magnitude, minDistance, maxDistance))) / (maxDistance - minDistance);
            separationVector =  weight * separationVector.normalized;
            separationDir += separationVector;
        }

        separationDir = separationDir / (selectGroup.GetComponent<SelectGroup>().unitList.Count - excludeCount);

        return separationDir.normalized;
    }

    private float GetSeparationWeight(Vector3 curWorldPos, float minDistance)
    {
        float weight = 1;
        for (int index = 0; index < selectGroup.GetComponent<SelectGroup>().unitList.Count; index++)
        {
            Vector3 curUnitPos = selectGroup.GetComponent<SelectGroup>().unitList[index].gameObject.GetComponent<Rigidbody>().position;
            if ((curUnitPos - curWorldPos).magnitude < minDistance)
                weight ++;
        }

        return weight;
    }

    public void FinishAction() { isFinished = true; }

    public void StopMoving(ref Rigidbody rb) { rb.velocity = Vector3.zero; }

    public void InitializeMousePosition(Vector3 position) { mousePosition = position; }

    public void InitializeSelectGroupObject(ref GameObject _selectGroup) { selectGroup = _selectGroup; }

}

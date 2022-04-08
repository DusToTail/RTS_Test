using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action
{
    public enum ActionTypes
    {
        MoveTowards,
        StopAction
    }

    public ActionTypes type;
    public bool isFinished;

    public Vector3 mousePosition;

    public int flowFieldIndex;

    public Action(ActionTypes _type)
    {
        type = _type;
        isFinished = false;
        mousePosition = Vector3.zero;
        flowFieldIndex = -1;
    }

    public void MoveInFlowField(ref Rigidbody rb, float _movementSpeed)
    {
        if (Vector3.Distance(mousePosition, rb.gameObject.transform.position) < 0.1f) 
        {
            isFinished = true;
            return; 
        }
        int x = GridController.flowFieldList[flowFieldIndex].GetCellFromWorldPos(rb.gameObject.transform.position).bestDirection.Vector.x;
        int z = GridController.flowFieldList[flowFieldIndex].GetCellFromWorldPos(rb.gameObject.transform.position).bestDirection.Vector.y;
        Vector3 dir = new Vector3(x, 0, z);
        rb.velocity = dir * _movementSpeed;
    }

    public void StopAction(ref Rigidbody rb)
    {
        rb.velocity = Vector3.zero;
        isFinished = true;
    }

    public void InitializeMousePosition(Vector3 position)
    {
        mousePosition = position;
    }

    public void InitializeFlowFieldIndex(int index)
    {
        flowFieldIndex = index;
    }

}

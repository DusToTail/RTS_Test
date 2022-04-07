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

    public Action(ActionTypes _type)
    {
        type = _type;
    }

    public void MoveTowards(ref Rigidbody rb, Vector3 velocity, Vector3 destination)
    {
        if (Vector3.Distance(destination, rb.gameObject.transform.position) < 0.1f) 
        {
            isFinished = true;
            return; 
        }
        rb.velocity = velocity;
    }

    public void StopAction(ref Rigidbody rb)
    {
        rb.velocity = Vector3.zero;
        isFinished = true;
    }

}

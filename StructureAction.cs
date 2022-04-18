using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StructureAction
{
    /// <summary>
    /// DESCRIPTION: An Action is used by ActionController (component of a Structure gameObject) to determine what to do.
    /// An action is unique for each structure and for each declaration.
    /// </summary>

    //Different actions can be taken. Used in ActionController's [CommitCurrentAction] method as an argument in switch statement
    public enum ActionTypes
    {
        ProduceUnit,
        Upgrade,
        StopAction,
        None
    }

    public ActionTypes type;

    //To determine if the action is finished (if move, then destination is reached).
    //Used in StructureActionController component to see if it should Dequeue the next action or not.
    public bool isFinished;

    public StructureAction(ActionTypes _type)
    {
        type = _type;
        isFinished = false;
    }

    public void ProduceUnit(GameObject prefab)
    {

    }

    public void Upgrade()
    {

    }



    public void FinishAction() { isFinished = true; }



}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StructureAction
{
    public enum ActionTypes
    {
        Produce,
        Upgrade,
        Attack,
        StopAction,
        None
    }

    public ActionTypes type;

    //To determine if the action is finished (if move, then destination is reached).
    public bool isFinished;

    public void FinishAction() { isFinished = true; }

}

public class ProductionStructureAction : StructureAction
{
    public ProductionStructureAction(ActionTypes _type)
    {
        type = _type;
        isFinished = false;
    }

    public void ProduceUnit(GameObject _prefab)
    {

    }

}

public class TechStructureAction : StructureAction
{
    public TechStructureAction(ActionTypes _type)
    {
        type = _type;
        isFinished = false;
    }

    public void Upgrade()
    {

    }


}

public class DefenseStructureAction : StructureAction
{
    public DefenseStructureAction(ActionTypes _type)
    {
        type = _type;
        isFinished = false;
    }

    public void AttackTarget()
    {

    }

}

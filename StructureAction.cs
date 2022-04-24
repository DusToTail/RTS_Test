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
        Cancel,
        StopAction,
        None
    }

    public ActionTypes type;

    //To determine if the action is finished
    public bool isFinished;

    public virtual void ActionOne()
    {
        Debug.Log("Perform Action One");
    }

    public virtual void ActionTwo()
    {
        Debug.Log("Perform Action Two");
    }

    public virtual void ActionThree()
    {
        Debug.Log("Perform Action Three");
    }

    public virtual void ActionFour()
    {
        Debug.Log("Perform Action Four");
    }

    public void FinishAction() { isFinished = true; }

    public void Cancel()
    {

    }

}

public class ProductionStructureAction : StructureAction
{
    public GameObject prefab;
    public Vector3 spawnPosition;
    public ProductionStructure structureInfo;
    public ProductionStructureAction(ActionTypes _type)
    {
        type = _type;
        isFinished = false;
        prefab = null;
        spawnPosition = Vector3.zero;
        structureInfo = null;
    }

    public override void ActionOne() { ProduceUnit(); }

    private void ProduceUnit()
    {
        Debug.Log($"{structureInfo.gameObject.name} producing {prefab.name}");
    }

}

public class TechStructureAction : StructureAction
{
    public TechStructureAction(ActionTypes _type)
    {
        type = _type;
        isFinished = false;
    }

    public override void ActionOne()
    {
        Upgrade();
    }

    private void Upgrade()
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

    public override void ActionOne()
    {
        AttackTarget();
    }

    private void AttackTarget()
    {

    }

}

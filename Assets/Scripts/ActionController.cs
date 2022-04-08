using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Unit))]
public class ActionController : MonoBehaviour
{
    public Queue<Action> actionQueue { get; private set; }
    private Action curAction;

    //Components
    private Rigidbody rb;
    private Unit unitInfo;

    private void Awake()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        unitInfo = gameObject.GetComponent<Unit>();

        actionQueue = new Queue<Action>();

        curAction = null;

    }
    private void Start()
    {
        
    }

    private void Update()
    {
        DequeueActionToCurrentAction();
        CommitCurrentAction();
    }

    public void EnqueueAction(Action _action)
    {
        actionQueue.Enqueue(_action);
    }

    private void CommitCurrentAction()
    {
        if (curAction == null) { return; }
        switch (curAction.type)
        {
            case Action.ActionTypes.MoveTowards:
                curAction.MoveInFlowField(ref rb, unitInfo.GetMovementSpeed());
                break;
            case Action.ActionTypes.StopAction:
                curAction.StopAction(ref rb);
                actionQueue.Clear();
                break;


            default:
                break;
        }
    }

    private void DequeueActionToCurrentAction()
    {
        if (actionQueue.Count == 0) { return; }
        curAction = actionQueue.Dequeue();
    }

    
}

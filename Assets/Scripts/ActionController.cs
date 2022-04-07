using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Unit))]
public class ActionController : MonoBehaviour
{
    public Queue<Action> actionQueue { get; private set; }
    private Action curAction;

    private FlowField refFlowField;
    private Vector3 curDirection;

    //Components
    private Rigidbody rb;
    private Unit unitInfo;

    private void Awake()
    {
        rb = gameObject.GetComponent<Rigidbody>();

        actionQueue = new Queue<Action>();

    }
    private void Start()
    {
        
    }

    private void Update()
    {
        DequeueActionToCurrentAction();
        CommitCurrentAction();
    }

    public void CommitCurrentAction()
    {
        if (curAction == null) { return; }
        switch (curAction.type)
        {
            case Action.ActionTypes.MoveTowards:
                Vector3 dir = ((Vector3Int)refFlowField.GetCellFromWorldPos(gameObject.transform.position).bestDirection.Vector);
                Vector3 velocity = dir * unitInfo.GetMovementSpeed();
                curAction.MoveTowards(ref rb, velocity, refFlowField.destinationCell.worldPosition);
                break;
            case Action.ActionTypes.StopAction:
                curAction.StopAction(ref rb);
                actionQueue.Clear();
                break;


            default:
                break;
        }
    }

    public void DequeueActionToCurrentAction()
    {
        if (actionQueue.Count == 0) { return; }
        if (curAction.isFinished == false) { return; }
        curAction = actionQueue.Dequeue();
    }

    public void EnqueueAction(Action _action)
    {
        actionQueue.Enqueue(_action);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Unit))]
public class ActionController : MonoBehaviour
{
    /// <summary>
    /// DESCRIPTION: An Action Controller is a component attached to a Unit gameObject.
    /// Used to QUEUE and EXECUTE actions
    /// </summary>
    /// 
    
    public Queue<Action> actionQueue { get; private set; }
    private Action curAction;

    //Components
    private Rigidbody rb;
    private Unit unitInfo;

    private void Awake()
    {
        //Initialization of components, queue and variables;
        rb = gameObject.GetComponent<Rigidbody>();
        unitInfo = gameObject.GetComponent<Unit>();

        actionQueue = new Queue<Action>();

        curAction = new Action(Action.ActionTypes.None);
        curAction.FinishAction();

    }
    private void Start()
    {
        
    }

    private void Update()
    {
        DequeueActionToCurrentAction();
        ExecuteCurrentAction();
    }

    /// <summary>
    /// Perform the current action (dequeue from the queue) per frame updated.
    /// </summary>
    private void ExecuteCurrentAction()
    {
        //Validity Check
        if (curAction == null) { return; }
        if(curAction.isFinished == true) { return; }

        //Switch statement basing on types of actions
        switch (curAction.type)
        {
            case Action.ActionTypes.MoveTowards:
                //Debug.Log(gameObject.name + " is moving");

                curAction.MoveInFlowField(ref rb, curAction.selectGroup.GetComponent<SelectGroup>().groupFlowField, unitInfo.GetMovementSpeed());
                break;

            case Action.ActionTypes.StopAction:
                //Debug.Log(gameObject.name + " stopped");

                curAction.StopMoving(ref rb);
                curAction.FinishAction();

                actionQueue.Clear();
                actionQueue.TrimExcess();
                break;


            default:
                break;
        }
    }

    //[Enqueue] OR [Clear queue, then Enqueue] an action to simulate chain of commands.
    // Called in SelectSystem when right click
    // bool isInstant is false if SHIFT is held when right click. if not, true
    public void EnqueueAction(ref Action _action, bool isInstant)
    {
        if (isInstant == true)
        {
            if (curAction != null) { curAction.FinishAction(); }
            actionQueue.Clear();
            actionQueue.TrimExcess();
        }
        actionQueue.Enqueue(_action);

        //Debug.Log("Action Enqueued");
    }

    // Dequeue an action from the list to current action variable
    private void DequeueActionToCurrentAction()
    {
        //Validity Check
        if (curAction.isFinished == false) { return; }
        if(actionQueue.Count == 0) { return; }

        curAction = actionQueue.Dequeue();

        //Debug.Log("Action Dequeued");

    }


}

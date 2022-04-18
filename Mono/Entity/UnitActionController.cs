using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Unit))]
public class UnitActionController : MonoBehaviour
{
    /// <summary>
    /// DESCRIPTION: An Action Controller is a component attached to a Unit gameObject.
    /// Used to QUEUE and EXECUTE actions
    /// </summary>
    /// 
    
    public Queue<UnitAction> actionQueue { get; private set; }
    private UnitAction curAction;

    //Components
    private Rigidbody rb;
    private Unit unitInfo;

    private void Awake()
    {
        //Initialization of components, queue and variables;
        rb = gameObject.GetComponent<Rigidbody>();
        unitInfo = gameObject.GetComponent<Unit>();

        actionQueue = new Queue<UnitAction>();

        curAction = new UnitAction(UnitAction.ActionTypes.None);
        curAction.FinishAction();

    }
    private void Start()
    {
        
    }

    private void Update()
    {
        DequeueActionToCurrentAction();
    }

    private void FixedUpdate()
    {
        ExecuteCurrentAction();
    }

    /// <summary>
    /// Perform the current action (dequeue from the queue) per frame updated.
    /// </summary>
    private void ExecuteCurrentAction()
    {
        //Validity Check
        if (curAction == null || curAction.isFinished == true) { return; }

        //Switch statement basing on types of actions
        switch (curAction.type)
        {
            case UnitAction.ActionTypes.MoveTowards:
                //Debug.Log(gameObject.name + " is moving");

                curAction.MoveInFlowField(ref rb, unitInfo.GetMovementSpeed());
                break;

            case UnitAction.ActionTypes.Patrol:
                //Debug.Log(gameObject.name + " is patrolling");

                curAction.Patrol(ref rb, unitInfo.GetMovementSpeed());
                break;

            case UnitAction.ActionTypes.AttackMove:
                //Debug.Log(gameObject.name + " is atack moving");

                curAction.MoveInFlowField(ref rb, unitInfo.GetMovementSpeed());
                break;

            case UnitAction.ActionTypes.StopAction:
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
    public void EnqueueAction(ref UnitAction _action, bool isInstant)
    {
        if (isInstant == true)
        {
            if (curAction != null) { curAction.FinishAction(); }

            
            while(actionQueue.Count > 0)
            {
                curAction = actionQueue.Dequeue();
                curAction.FinishAction();
            }
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

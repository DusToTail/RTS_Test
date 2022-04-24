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
    
    public Queue<IAction> actionQueue { get; private set; }
    private IAction curAction;

    //Components
    private Unit unitInfo;

    private void Awake()
    {
        //Initialization of components, queue and variables;
        unitInfo = gameObject.GetComponent<Unit>();

        actionQueue = new Queue<IAction>();

        curAction = new MarineAction(MarineAction.Type.None, null);
        curAction.Stop();

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

        if(curAction.GetSelf().type == curAction.GetSelf().Type.MoveTowards)
        {
            curAction.GetSelf().MoveTowards(unitInfo.GetRigidbody(), unitInfo.GetSetMovementSpeed());
        }
        else if (curAction.GetSelf().type == curAction.GetSelf().Type.Patrol)
        {
            curAction.GetSelf().Patrol(unitInfo.GetRigidbody(), unitInfo.GetSetMovementSpeed());
        }
        else if (curAction.GetSelf().type == curAction.GetSelf().Type.AttackTarget)
        {
            curAction.GetSelf().AttackTarget(unitInfo.GetRigidbody(), unitInfo.GetSetMovementSpeed(), unitInfo.GetSetAttackDamage(), unitInfo.GetSetAttackRange(), unitInfo.GetSetVisionRange(), unitInfo.CanAttack());
        }
        else if (curAction.GetSelf().type == curAction.GetSelf().Type.AttackMove)
        {
            curAction.GetSelf().AttackTarget(unitInfo.GetRigidbody(), unitInfo.GetSetMovementSpeed(), unitInfo.GetSetAttackDamage(), unitInfo.GetSetAttackRange(), unitInfo.GetSetVisionRange(), unitInfo.CanAttack());
        }
        else if (curAction.GetSelf().type == curAction.GetSelf().Type.StopMoving)
        {
            curAction.GetSelf().StopMoving(unitInfo.GetRigidbody());
        }
        else if (curAction.GetSelf().type == curAction.GetSelf().Type.Stop)
        {
            curAction.GetSelf().Stop();
        }
    }

    //[Enqueue] OR [Clear queue, then Enqueue] an action to simulate chain of commands.
    // Called in SelectSystem when right click
    // bool isInstant is false if SHIFT is held when right click. if not, true
    public void EnqueueAction(IAction _action, bool isInstant)
    {
        if (isInstant == true)
        {
            if (curAction != null) { curAction.Stop(); }

            
            while(actionQueue.Count > 0)
            {
                curAction = actionQueue.Dequeue();
                curAction.Stop();
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

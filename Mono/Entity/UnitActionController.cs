using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(IUnit))]
public class UnitActionController : MonoBehaviour
{
    /// <summary>
    /// DESCRIPTION: An Action Controller is a component attached to a Unit gameObject.
    /// Used to QUEUE and EXECUTE actions
    /// </summary>
    /// 
    
    public Queue<IAction> actionQueue { get; private set; }
    private IAction curAction;

    // Components
    private IUnit unitInfo;

    private void Awake()
    {
        //Initialization of components, queue and variables;
        unitInfo = gameObject.GetComponent<IUnit>();
        actionQueue = new Queue<IAction>();

        curAction = unitInfo.ReturnNewAction();
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
        switch ((int)curAction.GetSelf().type)
        {
            case 0:
                curAction.GetSelf().MoveTowards(unitInfo.GetRigidbody(), unitInfo.GetSetMovementSpeed());
                //Debug.Log($"{gameObject.name} is moving");
                break;
            case 1:
                curAction.GetSelf().Patrol(unitInfo.GetRigidbody(), unitInfo.GetSetMovementSpeed());
                //Debug.Log($"{gameObject.name} is patrolling");
                break;
            case 2:
                curAction.GetSelf().AttackTarget(unitInfo.GetRigidbody(), unitInfo.GetSetMovementSpeed(), unitInfo.GetSetAttackDamage(), unitInfo.GetSetAttackRange(), unitInfo.GetSetVisionRange(), unitInfo.CanAttack());
                //Debug.Log($"{gameObject.name} is attacking target");
                break;
            case 3:
                curAction.GetSelf().AttackMove(unitInfo.GetRigidbody(), unitInfo.GetSetMovementSpeed(), unitInfo.GetSetAttackDamage(), unitInfo.GetSetAttackRange(), unitInfo.GetSetVisionRange(), unitInfo.CanAttack());
                //Debug.Log($"{gameObject.name} is attack moving");
                break;
            case 4:
                curAction.GetSelf().StopMoving(unitInfo.GetRigidbody());
                //Debug.Log($"{gameObject.name} is stop moving");
                break;
            case 5:
                curAction.GetSelf().Stop();
                //Debug.Log($"{gameObject.name} is stopping");
                break;

            case 6:
                curAction.GetSelf().Cancel();
                break;

            default:
                break;

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
            Debug.Log("Clear Action List");
        }
        actionQueue.Enqueue(_action);

        Debug.Log("Action Enqueued");
    }

    // Dequeue an action from the list to current action variable
    private void DequeueActionToCurrentAction()
    {
        //Validity Check
        if (curAction.isFinished == false) { return; }
        if(actionQueue.Count == 0) { return; }

        curAction = actionQueue.Dequeue();

        Debug.Log("Action Dequeued");

    }


}

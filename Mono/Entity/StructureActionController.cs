using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(IStructure))]
public class StructureActionController : MonoBehaviour
{
    /// <summary>
    /// DESCRIPTION: An Action Controller is a component attached to a Structure gameObject.
    /// Used to QUEUE and EXECUTE actions
    /// </summary>
    /// 

    public Queue<IAction> actionQueue { get; private set; }
    private IAction curAction;

    // Components
    private IStructure structureInfo;

    private void Awake()
    {
        //Initialization of components, queue and variables;
        structureInfo = gameObject.GetComponent<IStructure>();
        Debug.Log(structureInfo.GetType());
        actionQueue = new Queue<IAction>();

        curAction = structureInfo.ReturnNewAction();
        curAction.Stop();
    }

    private void Start()
    {
        
    }

    private void Update()
    {
        DequeueActionToCurrentAction();

        ExecuteCurrentActionBasedOnStructureType();
    }

    /// <summary>
    /// Perform the current action (dequeue from the queue) per frame updated.
    /// </summary>
    private void ExecuteCurrentActionBasedOnStructureType()
    {
        //Validity Check
        if (curAction == null || curAction.isFinished == true) { return; }

        switch((int)curAction.GetSelf().type)
        {
            case 0:
                curAction.GetSelf().MoveTowards(structureInfo.GetRigidbody(), 0);
                //Debug.Log($"{gameObject.name} is moving");
                break;
            case 1:
                curAction.GetSelf().Patrol(structureInfo.GetRigidbody(), 0);
                //Debug.Log($"{gameObject.name} is patrolling");
                break;
            case 2:
                curAction.GetSelf().AttackTarget(structureInfo.GetRigidbody(), 0, structureInfo.GetSetAttackDamage(), structureInfo.GetSetAttackRange(), structureInfo.GetSetVisionRange(), structureInfo.CanAttack());
                //Debug.Log($"{gameObject.name} is attacking target");
                break;
            case 3:
                curAction.GetSelf().AttackMove(structureInfo.GetRigidbody(), 0, structureInfo.GetSetAttackDamage(), structureInfo.GetSetAttackRange(), structureInfo.GetSetVisionRange(), structureInfo.CanAttack());
                //Debug.Log($"{gameObject.name} is attack moving");
                break;
            case 4:
                curAction.GetSelf().StopMoving(structureInfo.GetRigidbody());
                //Debug.Log($"{gameObject.name} is stop moving");
                break;
            case 5:
                curAction.GetSelf().Stop();
                //Debug.Log($"{gameObject.name} is stopping");
                break;

            case 6:
                curAction.GetSelf().Cancel();
                //Debug.Log($"{gameObject.name} canceled");
                break;

            case 100:
                curAction.GetSelf().SpawnUnit();
                //Debug.Log($"{gameObject.name} is spawning unit");
                break;
            case 101:
                curAction.GetSelf().Upgrade();
                //Debug.Log($"{gameObject.name} is upgrading");
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


            while (actionQueue.Count > 0)
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
        if (actionQueue.Count == 0) { return; }

        curAction = actionQueue.Dequeue();

        Debug.Log("Action Dequeued");

    }

}

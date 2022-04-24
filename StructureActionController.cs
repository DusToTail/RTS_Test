using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Structure))]
public class StructureActionController : MonoBehaviour
{
    /// <summary>
    /// DESCRIPTION: An Action Controller is a component attached to a Structure gameObject.
    /// Used to QUEUE and EXECUTE actions
    /// </summary>
    /// 

    public Queue<object> actionQueue { get; private set; }
    [SerializeField]
    private StructureAction.ActionTypes[] actionTypes;
    private object curAction;

    // Components
    private object structureInfo;

    private void Awake()
    {
        if(GetComponent<ProductionStructure>() != null)
            structureInfo = GetComponent<ProductionStructure>();
        else if(GetComponent<TechStructure>() != null)
            structureInfo = GetComponent<TechStructure>();
        else if(GetComponent<DefenseStructure>() != null)
            structureInfo = GetComponent<DefenseStructure>();
        else
            structureInfo = GetComponent<Structure>();

    }

    private void Start()
    {
        
    }

    private void Update()
    {
        
    }

    /// <summary>
    /// Perform the current action (dequeue from the queue) per frame updated.
    /// </summary>
    private void ExecuteCurrentActionBasedOnStructureType()
    {
        //Validity Check
        if (curAction == null || ((StructureAction)curAction).isFinished == true) { return; }

        if(structureInfo.GetType() == typeof(ProductionStructure))
        {
            switch (((StructureAction)curAction).type)
            {
                case StructureAction.ActionTypes.Produce:
                    Debug.Log($"{gameObject.name} producing");

                    ((ProductionStructureAction)curAction).ActionOne();
                    break;

                case StructureAction.ActionTypes.Cancel:
                    Debug.Log($"{gameObject.name} canceled");

                    ((ProductionStructureAction)curAction).Cancel();
                    break;

                case StructureAction.ActionTypes.StopAction:
                    Debug.Log($"{gameObject.name} stopped");

                    ((ProductionStructureAction)curAction).FinishAction();
                    break;

                case StructureAction.ActionTypes.None:
                    break;

                default:
                    break;
            }
        }
        else if(structureInfo.GetType() == typeof(TechStructure))
        {
            switch (((StructureAction)curAction).type)
            {

                case StructureAction.ActionTypes.Upgrade:
                    Debug.Log($"{gameObject.name} upgrading");

                    ((TechStructureAction)curAction).ActionOne();
                    break;

                case StructureAction.ActionTypes.Cancel:
                    Debug.Log($"{gameObject.name} canceled");

                    ((TechStructureAction)curAction).Cancel();
                    break;

                case StructureAction.ActionTypes.StopAction:
                    Debug.Log($"{gameObject.name} stopped");

                    ((TechStructureAction)curAction).FinishAction();
                    break;

                case StructureAction.ActionTypes.None:
                    break;

                default:
                    break;
            }
        }
        else if(structureInfo.GetType() == typeof(DefenseStructure))
        {
            switch (((StructureAction)curAction).type)
            {

                case StructureAction.ActionTypes.Attack:
                    Debug.Log($"{gameObject.name} attacking");

                    ((DefenseStructureAction)curAction).ActionOne();
                    break;

                case StructureAction.ActionTypes.Cancel:
                    Debug.Log($"{gameObject.name} canceled");

                    ((DefenseStructureAction)curAction).Cancel();
                    break;

                case StructureAction.ActionTypes.StopAction:
                    Debug.Log($"{gameObject.name} stopped");

                    ((DefenseStructureAction)curAction).FinishAction();
                    break;

                case StructureAction.ActionTypes.None:
                    break;

                default:
                    break;
            }
        }
        else if(structureInfo.GetType() == typeof(Structure))
        {
            switch (((StructureAction)curAction).type)
            {
                case StructureAction.ActionTypes.Cancel:
                    Debug.Log($"{gameObject.name} canceled");

                    ((StructureAction)curAction).Cancel();
                    break;

                case StructureAction.ActionTypes.StopAction:
                    Debug.Log($"{gameObject.name} stopped");

                    ((StructureAction)curAction).FinishAction();
                    break;

                case StructureAction.ActionTypes.None:
                    break;

                default:
                    break;
            }
        }
        
    }

    //[Enqueue] OR [Clear queue, then Enqueue] an action to simulate chain of commands.
    // Called in SelectSystem when right click
    // bool isInstant is false if SHIFT is held when right click. if not, true
    public void EnqueueAction(ref StructureAction _action, bool isInstant)
    {
        if (isInstant == true)
        {
            if (curAction != null) { ((StructureAction)curAction).FinishAction(); }


            while (actionQueue.Count > 0)
            {
                curAction = actionQueue.Dequeue();
                ((StructureAction)curAction).FinishAction();
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
        if (((StructureAction)curAction).isFinished == false) { return; }
        if (actionQueue.Count == 0) { return; }

        curAction = actionQueue.Dequeue();

        //Debug.Log("Action Dequeued");

    }

}

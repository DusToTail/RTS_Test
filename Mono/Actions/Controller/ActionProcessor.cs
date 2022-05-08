using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionProcessor : MonoBehaviour
{
    public IAction curAction { get; private set; }
    public Queue<IAction> actionQueue { get; private set; }


    private void Start()
    {
        actionQueue = new Queue<IAction>();
        curAction = new StopAction();
        curAction.Stop();
    }

    private void Update()
    {
        DequeueAction();
    }

    private void FixedUpdate()
    {
        if (curAction == null) 
        { 
            //Debug.Log("CurrentAction is Null");
        }
        else if(curAction.isFinished == false)
        {
            //Debug.Log("Not yet finish current action");
            //Debug.Log("Executing Action");
            ExecuteCurrentAction();
        }
        else if(curAction.isFinished == true)
        {
            //Debug.Log("Current Action is finished");
        }
        else
        {
            //Debug.Log("Current Action others");
        }
    }

    
    public void EnqueueAction(IAction _action, bool _isQueued)
    {
        if(_action is IProgress == true) { return; }
        if(_isQueued) 
        { 
            actionQueue.Enqueue(_action);
            Debug.Log($"Action enqueued in {gameObject.name}");
        }
        else
        {
            foreach(var action in actionQueue)
            {
                action.Stop();
            }
            actionQueue.Clear();
            actionQueue.TrimExcess();
            curAction.Stop();
            Debug.Log($"Action Queue cLeared in {gameObject.name}");
            actionQueue.Enqueue(_action);
            Debug.Log($"Action enqueued in {gameObject.name}");
        }

    }

    private void ExecuteCurrentAction()
    {
        curAction.Execute();
    }


    private void DequeueAction()
    {
        //Validity Check
        if (actionQueue.Count == 0) { return; }
        if (curAction.isFinished == false) { return; }

        curAction = actionQueue.Dequeue();

        Debug.Log($"Action dequeued in {gameObject.name}");

    }
}

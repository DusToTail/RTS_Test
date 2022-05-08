using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProductionProcessor : MonoBehaviour
{
    public int maxQueueCount;
    public IAction curProduction { get; private set; }
    public Queue<IAction> productionQueue { get; private set; }

    private void Start()
    {
        productionQueue = new Queue<IAction>();
        curProduction = new StopAction();
        curProduction.Stop();
    }

    private void Update()
    {
        DequeueProduction();
    }

    private void FixedUpdate()
    {
        ExecuteCurrentProduction();
    }



    public void EnqueueAction(IAction _action)
    {
        if (_action is IProgress == false) { return; }
        productionQueue.Enqueue(_action);
        Debug.Log($"Production enqueued in {gameObject.name}");
    }

    public bool QueueIsFull() { return productionQueue.Count >= maxQueueCount; }

    private void ExecuteCurrentProduction()
    {
        if (curProduction == null) { return; }
        curProduction.Execute();
    }

    private void DequeueProduction()
    {
        //Validity Check
        if (productionQueue.Count == 0) { return; }
        if (curProduction.isFinished == false) { return; }

        curProduction = productionQueue.Dequeue();

        Debug.Log($"Production dequeued in {gameObject.name}");

    }
}

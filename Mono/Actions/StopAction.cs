using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StopAction : IAction
{
    public bool isFinished { get; set; }

    public StopAction()
    {
        isFinished = false;
    }

    public void Stop()
    {
        isFinished = true;
    }

    public void Execute()
    {
        Stop();
    }
}

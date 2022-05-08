using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CancelAction : ICancel
{
    public bool isFinished { get; set; }

    public void Stop()
    {
        isFinished = true;
    }

    public void Execute()
    {
        Stop();
    }
}

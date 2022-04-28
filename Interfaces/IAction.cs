using System.Collections.Generic;
using UnityEngine;

public interface IAction
{
    public List<IAction> subActions { get; set; }

    public bool isFinished { get; set; }
    public SelectGroup group { get; set; }

    public void Stop();
    public void Cancel();

    public void AssignSubActions(List<IAction> _actions);
    public SelectGroup GetGroup();
    public List<IAction> GetSubActions();

    public dynamic GetSelf();
}

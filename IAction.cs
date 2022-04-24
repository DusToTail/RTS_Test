using UnityEngine;

public interface IAction
{
    public bool isFinished { get; set; }
    public SelectGroup group { get; set; }

    public void Stop();
    public void Cancel();
    public SelectGroup GetGroup();

    public dynamic GetSelf();
}

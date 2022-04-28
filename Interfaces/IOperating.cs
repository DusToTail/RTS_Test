using UnityEngine;

public interface IOperating : IAction
{
    public float maxProgressTime { get; set; }
    public float curProgressTime { get; set; }

    public void AddProgress(float _amount);

    public bool IsRunning();
}

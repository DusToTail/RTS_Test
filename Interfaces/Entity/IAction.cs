using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Base Interface for any actions
/// </summary>
public interface IAction
{
    public bool isFinished { get; set; }

    public void Execute();

    /// <summary>
    /// English: Mark the action is finished.
    /// 日本語：行動が完了したようにする。
    /// </summary>
    public void Stop();
}

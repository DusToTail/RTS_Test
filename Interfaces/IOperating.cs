using UnityEngine;

/// <summary>
/// English: Interface for actions that have incremental progress
/// 日本語：進捗のある行動用のインターフェース
/// </summary>
public interface IOperating : IAction
{
    public float maxProgressTime { get; set; }
    public float curProgressTime { get; set; }
    /// <summary>
    /// English: Add time to current progress.
    /// 日本語：進捗に時間を加える。
    /// </summary>
    /// <param name="_time"></param>
    public void AddProgress(float _amount);
    /// <summary>
    /// ※　NOT USED　※
    /// </summary>
    /// <returns></returns>
    public bool IsRunning();
}

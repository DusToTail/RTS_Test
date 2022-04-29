using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Base Interface for any actions
/// </summary>
public interface IAction
{
    public List<IAction> subActions { get; set; }

    public bool isFinished { get; set; }
    public SelectGroup group { get; set; }

    /// <summary>
    /// English: Mark the action is finished.
    /// 日本語：行動が完了したようにする。
    /// </summary>
    public void Stop();
    /// <summary>
    /// English: Cancel the current action and any of its progress.
    /// 日本語：現在の行動を停止し、進捗をクリアする。
    /// </summary>
    public void Cancel();
    /// <summary>
    /// English: Assign the list of actions associated with this action during execution
    /// 日本語：この行動を実行している間に現れる行動のリストを割り当てる。
    /// </summary>
    /// <param name="_actions"></param>
    public void AssignSubActions(List<IAction> _actions);
    /// <summary>
    /// English: Return the group this action is in.
    /// 日本語：所属しているグループを返す
    /// </summary>
    /// <returns></returns>
    public SelectGroup GetGroup();
    /// <summary>
    /// English: Return the list of actions associated with this action during execution
    /// 日本語：この行動を実行している間に現れる行動のリストを返す。
    /// </summary>
    /// <returns></returns>
    public List<IAction> GetSubActions();
    /// <summary>
    /// English: Return this action (run-time type)
    /// 日本語：この行動を返す（run-time type)
    /// </summary>
    /// <returns></returns>
    public dynamic GetSelf();
}

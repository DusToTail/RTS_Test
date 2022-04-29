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
    /// ���{��F�s�������������悤�ɂ���B
    /// </summary>
    public void Stop();
    /// <summary>
    /// English: Cancel the current action and any of its progress.
    /// ���{��F���݂̍s�����~���A�i�����N���A����B
    /// </summary>
    public void Cancel();
    /// <summary>
    /// English: Assign the list of actions associated with this action during execution
    /// ���{��F���̍s�������s���Ă���ԂɌ����s���̃��X�g�����蓖�Ă�B
    /// </summary>
    /// <param name="_actions"></param>
    public void AssignSubActions(List<IAction> _actions);
    /// <summary>
    /// English: Return the group this action is in.
    /// ���{��F�������Ă���O���[�v��Ԃ�
    /// </summary>
    /// <returns></returns>
    public SelectGroup GetGroup();
    /// <summary>
    /// English: Return the list of actions associated with this action during execution
    /// ���{��F���̍s�������s���Ă���ԂɌ����s���̃��X�g��Ԃ��B
    /// </summary>
    /// <returns></returns>
    public List<IAction> GetSubActions();
    /// <summary>
    /// English: Return this action (run-time type)
    /// ���{��F���̍s����Ԃ��irun-time type)
    /// </summary>
    /// <returns></returns>
    public dynamic GetSelf();
}

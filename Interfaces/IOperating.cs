using UnityEngine;

/// <summary>
/// English: Interface for actions that have incremental progress
/// ���{��F�i���̂���s���p�̃C���^�[�t�F�[�X
/// </summary>
public interface IOperating : IAction
{
    public float maxProgressTime { get; set; }
    public float curProgressTime { get; set; }
    /// <summary>
    /// English: Add time to current progress.
    /// ���{��F�i���Ɏ��Ԃ�������B
    /// </summary>
    /// <param name="_time"></param>
    public void AddProgress(float _amount);
    /// <summary>
    /// ���@NOT USED�@��
    /// </summary>
    /// <returns></returns>
    public bool IsRunning();
}

using UnityEngine;

/// <summary>
/// English: Interface for upgrading action
/// ���{��F�A�b�v�O���[�h�s���̃C���^�[�t�F�[�X
/// </summary>
public interface IUpgrade : IProgress
{
    public Upgrade upgrade { get; set; }
    /// <summary>
    /// English: Update the current progress time. Send an event to all listener about the upgrade when finished.
    /// ���{��F�i���̎��Ԃ��X�V����B����������A�C�x���g�𔭐M����B
    /// </summary>
    public void UpdateProgress();

    /// <summary>
    /// English: Set a prefab upgrade to the action.
    /// ���{��F�s���̃A�b�v�O���[�h�̕ϐ������蓖�Ă�B
    /// </summary>
    /// <param name="_upgrade"></param>
    public void SetUpgrade(Upgrade _upgrade);
}

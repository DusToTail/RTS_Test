using UnityEngine;

/// <summary>
/// Interface for upgrading action
/// </summary>
public interface IUpgrade : IOperating
{
    public Upgrade prefabUpgrade { get; set; }
    /// <summary>
    /// English: Update the current progress time. Send an event to all listener about the upgrade when finished.
    /// ���{��F�i���̎��Ԃ��X�V����B����������A�C�x���g�𔭐M����B
    /// </summary>
    public void Upgrade();

    public float GetUpgradeTime();
    /// <summary>
    /// English: Assign an upgrade to the action.
    /// ���{��F�s���̃A�b�v�O���[�h�ϐ������蓖�Ă�B
    /// </summary>
    /// <param name="_prefab"></param>
    public void AssignUpgrade(Upgrade _upgrade);
}

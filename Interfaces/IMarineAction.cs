using UnityEngine;

/// <summary>
/// English: Interface for a Marine action. Involves Movement and Attacking
/// ���{��F�}���[���s���p�̃C���^�[�t�F�[�X�B�ړ��ƍU��������
/// </summary>
public interface IMarineAction : IMove, IAttack
{
    public MarineAction.Type type { get; set; }
}

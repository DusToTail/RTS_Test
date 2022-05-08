using UnityEngine;

/// <summary>
/// English: Interface for actions that involve movement of a Rigidbody
/// ���{��FRigidbody�ňړ�����s���p�̃C���^�[�t�F�[�X
/// </summary>
public interface IMove : IAction
{
    public Vector3 movePosition { get; set; }

    /// <summary>
    /// English: For each frame�AMove the rigidbody position and its rotation towards the destination, one subwaypoint (flowfield reinitialized) at a time
    /// ���{��F�t���[�����ƂɁARigidbody�̈ʒu�ƕ�����ړI�n�֍X�V���Ȃ���A�q�E�F�C�|�C���g���i������FlowField���Ăэ쐬����j
    /// </summary>
    /// <param name="_rb"></param>
    /// <param name="_speed"></param>
    public void MoveTowards(Rigidbody _rb, float _speed);

    /// <summary>
    /// English: Make Rigidbody's velocity to zero.
    /// ���{��FRigidbody�̑��x���[���ɂ���B
    /// </summary>
    /// <param name="_rb"></param>
    public void StopMoving(Rigidbody _rb);

    /// <summary>
    /// English: Assign move world position
    /// ���{��F�ړ��̍s���̖ړI�n�����蓖�Ă�B
    /// </summary>
    /// <param name="_vector"></param>
    public void SetMovePosition(Vector3 _vector);

    /// <summary>
    /// English: Return move world position
    /// ���{��F�ړ��̍s���̖ړI�n��Ԃ��B
    /// </summary>
    /// <param name="_vector"></param>
    public Vector3 GetMovePosition();

}

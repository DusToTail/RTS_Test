using UnityEngine;

/// <summary>
/// English: Interface for actions that involve patrolling of a Rigidbody
/// ���{��FRigidbody�Ńp�g���[������s���p�̃C���^�[�t�F�[�X
/// </summary>
public interface IPatrol : IMove
{
    public Vector3[] moveWaypoints { get; set; }
    public int curWaypointIndex { get; set; }

    /// <summary>
    /// English: Use MoveTowards() function for one waypoint at a time. Update waypoint when reached
    /// ���{��FMoveTowards()�֐��ŃE�F�C�|�C���g���ړ�������B������A�E�F�C�|�C���g���X�V����B
    /// </summary>
    /// <param name="_rb"></param>
    /// <param name="_speed"></param>
    public void Patrol(Rigidbody _rb, float _speed);


    /// <summary>
    /// English: Assign waypoints for Patrol Action
    /// ���{��F�p�g���[���s���̃E�F�C�|�C���g�����蓖�Ă�B
    /// </summary>
    /// <param name="_vectors"></param>
    public void SetWaypoints(Vector3[] _vectors);

}

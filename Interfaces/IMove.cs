using UnityEngine;

/// <summary>
/// English: Interface for actions that involve movement of a Rigidbody
/// ���{��FRigidbody�ňړ�����s���p�̃C���^�[�t�F�[�X
/// </summary>
public interface IMove : IAction
{
    public Vector3 moveMousePosition { get; set; }
    public Vector3[] moveWaypoints { get; set; }
    public FlowField curFlowField { get; set; }
    public FlowField selfFlowField { get; set; }
    public int curWaypointIndex { get; set; }

    /// <summary>
    /// English: For each frame�AMove the rigidbody position and its rotation towards the destination, one subwaypoint (flowfield reinitialized) at a time
    /// ���{��F�t���[�����ƂɁARigidbody�̈ʒu�ƕ�����ړI�n�֍X�V���Ȃ���A�q�E�F�C�|�C���g���i������FlowField���Ăэ쐬����j
    /// </summary>
    /// <param name="_rb"></param>
    /// <param name="_speed"></param>
    public void MoveTowards(Rigidbody _rb, float _speed);
    /// <summary>
    /// English: Use MoveTowards() function for one waypoint at a time. Update waypoint when reached
    /// ���{��FMoveTowards()�֐��ŃE�F�C�|�C���g���ړ�������B������A�E�F�C�|�C���g���X�V����B
    /// </summary>
    /// <param name="_rb"></param>
    /// <param name="_speed"></param>
    public void Patrol(Rigidbody _rb, float _speed);
    /// <summary>
    /// English: Make Rigidbody's velocity to zero.
    /// ���{��FRigidbody�̑��x���[���ɂ���B
    /// </summary>
    /// <param name="_rb"></param>
    public void StopMoving(Rigidbody _rb);

    /// <summary>
    /// English: Return direction of the cell on the flowfield, retrieved from the position on it.
    /// ���{��F���݂̈ʒu�Ŏ擾����FlowField��Cell�̕�����Ԃ��B
    /// </summary>
    /// <param name="_curWorldPos"></param>
    /// <returns></returns>
    public Vector3 GetFlowFieldDirection(Vector3 _curWorldPos);
    /// <summary>
    /// ���@NOT USED�@��
    /// For Flocking Behavior
    /// </summary>
    /// <param name="_curWorldPos"></param>
    /// <param name="_minDistance"></param>
    /// <param name="_maxDistance"></param>
    /// <returns></returns>
    public Vector3 GetAlignmentDirection(Vector3 _curWorldPos, float _minDistance, float _maxDistance);
    /// <summary>
    /// ���@NOT USED�@��
    /// For Flocking Behavior
    /// </summary>
    /// <param name="_curWorldPos"></param>
    /// <param name="_minDistance"></param>
    /// <param name="_maxDistance"></param>
    /// <returns></returns>
    public Vector3 GetSeparationDirection(Vector3 _curWorldPos, float _minDistance, float _maxDistance);

    /// <summary>
    /// English: Assign move position for Move Action
    /// ���{��F�ړ��̍s���̖ړI�n�����蓖�Ă�B
    /// </summary>
    /// <param name="_vector"></param>
    public void AssignMoveMousePosition(Vector3 _vector);
    /// <summary>
    /// English: Assign waypoints for Patrol Action
    /// ���{��F�p�g���[���s���̃E�F�C�|�C���g�����蓖�Ă�B
    /// </summary>
    /// <param name="_vectors"></param>
    public void AssignWaypointPosition(Vector3[] _vectors);
    /// <summary>
    /// English: Assign currently used flowfield to use in Move Action
    /// ���{��F�ړ��̍s���Ŏ��ۂ̌v�Z�ȂǂŎg�p�����FlowField�����蓖�Ă�B
    /// </summary>
    /// <param name="_flowField"></param>
    public void AssignCurrentFlowField(FlowField _flowField);
    /// <summary>
    /// English: Assign self flowfield to use in Move Action
    /// ���{��F�ړ��̍s���œƎ���FlowField�����蓖�Ă�B
    /// </summary>
    /// <param name="_flowField"></param>
    public void AssignSelfFlowField(FlowField _flowField);
}

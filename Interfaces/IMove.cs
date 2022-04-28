using UnityEngine;

public interface IMove : IAction
{
    public Vector3 moveMousePosition { get; set; }
    public Vector3[] moveWaypoints { get; set; }
    public FlowField curFlowField { get; set; }
    public FlowField selfFlowField { get; set; }
    public int curWaypointIndex { get; set; }

    public void MoveTowards(Rigidbody _rb, float _speed);
    public void Patrol(Rigidbody _rb, float _speed);
    public void StopMoving(Rigidbody _rb);

    public Vector3 GetFlowFieldDirection(Vector3 _curWorldPos);
    public Vector3 GetAlignmentDirection(Vector3 _curWorldPos, float _minDistance, float _maxDistance);
    public Vector3 GetSeparationDirection(Vector3 _curWorldPos, float _minDistance, float _maxDistance);

    public void AssignMoveMousePosition(Vector3 _vector);
    public void AssignWaypointPosition(Vector3[] _vectors);
    public void AssignCurrentFlowField(FlowField _flowField);
    public void AssignSelfFlowField(FlowField _flowField);
}

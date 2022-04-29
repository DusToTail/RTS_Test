using UnityEngine;

/// <summary>
/// English: Interface for actions that involve movement of a Rigidbody
/// 日本語：Rigidbodyで移動する行動用のインターフェース
/// </summary>
public interface IMove : IAction
{
    public Vector3 moveMousePosition { get; set; }
    public Vector3[] moveWaypoints { get; set; }
    public FlowField curFlowField { get; set; }
    public FlowField selfFlowField { get; set; }
    public int curWaypointIndex { get; set; }

    /// <summary>
    /// English: For each frame、Move the rigidbody position and its rotation towards the destination, one subwaypoint (flowfield reinitialized) at a time
    /// 日本語：フレームごとに、Rigidbodyの位置と方向を目的地へ更新しながら、子ウェイポイントずつ（到着後FlowFieldを再び作成する）
    /// </summary>
    /// <param name="_rb"></param>
    /// <param name="_speed"></param>
    public void MoveTowards(Rigidbody _rb, float _speed);
    /// <summary>
    /// English: Use MoveTowards() function for one waypoint at a time. Update waypoint when reached
    /// 日本語：MoveTowards()関数でウェイポイントずつ移動させる。到着後、ウェイポイントを更新する。
    /// </summary>
    /// <param name="_rb"></param>
    /// <param name="_speed"></param>
    public void Patrol(Rigidbody _rb, float _speed);
    /// <summary>
    /// English: Make Rigidbody's velocity to zero.
    /// 日本語：Rigidbodyの速度をゼロにする。
    /// </summary>
    /// <param name="_rb"></param>
    public void StopMoving(Rigidbody _rb);

    /// <summary>
    /// English: Return direction of the cell on the flowfield, retrieved from the position on it.
    /// 日本語：現在の位置で取得したFlowFieldのCellの方向を返す。
    /// </summary>
    /// <param name="_curWorldPos"></param>
    /// <returns></returns>
    public Vector3 GetFlowFieldDirection(Vector3 _curWorldPos);
    /// <summary>
    /// ※　NOT USED　※
    /// For Flocking Behavior
    /// </summary>
    /// <param name="_curWorldPos"></param>
    /// <param name="_minDistance"></param>
    /// <param name="_maxDistance"></param>
    /// <returns></returns>
    public Vector3 GetAlignmentDirection(Vector3 _curWorldPos, float _minDistance, float _maxDistance);
    /// <summary>
    /// ※　NOT USED　※
    /// For Flocking Behavior
    /// </summary>
    /// <param name="_curWorldPos"></param>
    /// <param name="_minDistance"></param>
    /// <param name="_maxDistance"></param>
    /// <returns></returns>
    public Vector3 GetSeparationDirection(Vector3 _curWorldPos, float _minDistance, float _maxDistance);

    /// <summary>
    /// English: Assign move position for Move Action
    /// 日本語：移動の行動の目的地を割り当てる。
    /// </summary>
    /// <param name="_vector"></param>
    public void AssignMoveMousePosition(Vector3 _vector);
    /// <summary>
    /// English: Assign waypoints for Patrol Action
    /// 日本語：パトロール行動のウェイポイントを割り当てる。
    /// </summary>
    /// <param name="_vectors"></param>
    public void AssignWaypointPosition(Vector3[] _vectors);
    /// <summary>
    /// English: Assign currently used flowfield to use in Move Action
    /// 日本語：移動の行動で実際の計算などで使用されるFlowFieldを割り当てる。
    /// </summary>
    /// <param name="_flowField"></param>
    public void AssignCurrentFlowField(FlowField _flowField);
    /// <summary>
    /// English: Assign self flowfield to use in Move Action
    /// 日本語：移動の行動で独自のFlowFieldを割り当てる。
    /// </summary>
    /// <param name="_flowField"></param>
    public void AssignSelfFlowField(FlowField _flowField);
}

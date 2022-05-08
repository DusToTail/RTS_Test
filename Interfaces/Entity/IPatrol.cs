using UnityEngine;

/// <summary>
/// English: Interface for actions that involve patrolling of a Rigidbody
/// 日本語：Rigidbodyでパトロールする行動用のインターフェース
/// </summary>
public interface IPatrol : IMove
{
    public Vector3[] moveWaypoints { get; set; }
    public int curWaypointIndex { get; set; }

    /// <summary>
    /// English: Use MoveTowards() function for one waypoint at a time. Update waypoint when reached
    /// 日本語：MoveTowards()関数でウェイポイントずつ移動させる。到着後、ウェイポイントを更新する。
    /// </summary>
    /// <param name="_rb"></param>
    /// <param name="_speed"></param>
    public void Patrol(Rigidbody _rb, float _speed);


    /// <summary>
    /// English: Assign waypoints for Patrol Action
    /// 日本語：パトロール行動のウェイポイントを割り当てる。
    /// </summary>
    /// <param name="_vectors"></param>
    public void SetWaypoints(Vector3[] _vectors);

}

using UnityEngine;

/// <summary>
/// English: Interface for actions that involve movement of a Rigidbody
/// 日本語：Rigidbodyで移動する行動用のインターフェース
/// </summary>
public interface IMove : IAction
{
    public Vector3 movePosition { get; set; }

    /// <summary>
    /// English: For each frame、Move the rigidbody position and its rotation towards the destination, one subwaypoint (flowfield reinitialized) at a time
    /// 日本語：フレームごとに、Rigidbodyの位置と方向を目的地へ更新しながら、子ウェイポイントずつ（到着後FlowFieldを再び作成する）
    /// </summary>
    /// <param name="_rb"></param>
    /// <param name="_speed"></param>
    public void MoveTowards(Rigidbody _rb, float _speed);

    /// <summary>
    /// English: Make Rigidbody's velocity to zero.
    /// 日本語：Rigidbodyの速度をゼロにする。
    /// </summary>
    /// <param name="_rb"></param>
    public void StopMoving(Rigidbody _rb);

    /// <summary>
    /// English: Assign move world position
    /// 日本語：移動の行動の目的地を割り当てる。
    /// </summary>
    /// <param name="_vector"></param>
    public void SetMovePosition(Vector3 _vector);

    /// <summary>
    /// English: Return move world position
    /// 日本語：移動の行動の目的地を返す。
    /// </summary>
    /// <param name="_vector"></param>
    public Vector3 GetMovePosition();

}

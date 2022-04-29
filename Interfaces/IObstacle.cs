using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// English: Interface for obstacles
/// 日本語：障害物用のインターフェース
/// </summary>
public interface IObstacle
{
    /// <summary>
    /// English: Return a Vector2Int that represents the size of a x-y grid, covered by the obstacle
    /// 日本語：障害物が占めるグリッドのサイズを表すVector2Intの戻り値を返す。
    /// </summary>
    /// <returns></returns>
    public Vector2Int GetObstacleGridSize();
    /// <summary>
    /// English; Return world position
    /// 日本語：ワールド位置を返す
    /// </summary>
    /// <returns></returns>
    public Vector3 GetWorldPosition();
}

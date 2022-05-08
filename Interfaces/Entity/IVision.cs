using UnityEngine;

/// <summary>
/// English: Interface for vision
/// 日本語：視界用のインターフェース
/// </summary>
public interface IVision
{
    /// <summary>
    /// English: Return the vision range 
    /// 日本語：ビジョンの距離を返す
    /// </summary>
    /// <returns></returns>
    public float GetVisionRange();
}

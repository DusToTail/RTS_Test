using UnityEngine;

/// <summary>
/// English: Interface for a Marine action. Involves Movement and Attacking
/// 日本語：マリーン行動用のインターフェース。移動と攻撃がある
/// </summary>
public interface IMarineAction : IMove, IAttack
{
    public MarineAction.Type type { get; set; }
}

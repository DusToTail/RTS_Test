using UnityEngine;

/// <summary>
/// English: Interface for attacking. Involve Movement (for chasing)
/// 日本語：攻撃の行動用のインターフェース。移動がある（追及できるため）
/// </summary>
public interface IAttackMove : IMove, IAttack
{
    public Vector3 attackPosition { get; set; }

    /// <summary>
    /// English: Attack any nearby enemy whilst moving towards the specified position. Finish if destination is reached and there is no enemy nearby
    /// 日本語：周辺の敵を攻撃しながら、指定した位置まで移動する。目的地に到着し、周辺に敵がいない場合：完了する。
    /// </summary>
    /// <param name="_rb"></param>
    /// <param name="_movementSpeed"></param>
    /// <param name="_attackDamage"></param>
    /// <param name="_attackRange"></param>
    /// <param name="_visionRange"></param>
    /// <param name="_canAttack"></param>
    public void AttackMove(Rigidbody _rb, float _movementSpeed, float _attackDamage, float _attackRange, float _visionRange);

}

using UnityEngine;

/// <summary>
/// English: Interface for attacking. Involve Movement (for chasing)
/// 日本語：攻撃の行動用のインターフェース。移動がある（追及できるため）
/// </summary>
public interface IAttack : IMove
{
    public IEntity curTarget { get; set; }
    public Vector3 attackMousePosition { get; set; }
    /// <summary>
    /// English: Move towards the target if outside attack range. Attack if within attack range
    /// 日本語：攻撃範囲外の場合：ターゲットに近づく。範囲内の場合：攻撃する
    /// </summary>
    /// <param name="_rb"></param>
    /// <param name="_movementSpeed"></param>
    /// <param name="_attackDamage"></param>
    /// <param name="_attackRange"></param>
    /// <param name="_visionRange"></param>
    /// <param name="_canAttack"></param>
    public void AttackTarget(Rigidbody _rb, float _movementSpeed, float _attackDamage, float _attackRange, float _visionRange, bool _canAttack);
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
    public void AttackMove(Rigidbody _rb, float _movementSpeed, float _attackDamage, float _attackRange, float _visionRange, bool _canAttack);
    /// <summary>
    /// English: Return the closest enemy basing on the unit vision by Physics.OverlapCapsule(). Return null if no enemy is detected.
    /// 日本語：Physics.OverlapCapsule()と視界の距離で最も近い敵を返す。敵がなければ、nullを返す。
    /// </summary>
    /// <param name="_rb"></param>
    /// <param name="_visionRange"></param>
    /// <returns></returns>
    public IEntity ReturnNearbyEnemy(Rigidbody _rb, float _visionRange);

    /// <summary>
    /// English: Assign a target for Attack Action
    /// 日本語：攻撃の行動のターゲットを割り当てる。
    /// </summary>
    /// <param name="_target"></param>
    public void AssignTarget(IEntity _target);

}

using UnityEngine;

/// <summary>
/// English: Interface for attacking
/// 日本語：攻撃の行動用のインターフェース
/// </summary>
public interface IAttack : IAction
{
    public IEntity target { get; set; }

    /// <summary>
    /// English: Attack if within attack range
    /// 日本語：範囲内の場合：攻撃する
    /// </summary>
    /// <param name="_rb"></param>
    /// <param name="_movementSpeed"></param>
    /// <param name="_attackDamage"></param>
    /// <param name="_attackRange"></param>
    /// <param name="_visionRange"></param>
    /// <param name="_canAttack"></param>
    public void AttackTarget(Vector3 _curPosition, float _attackDamage, float _attackRange, float _visionRange);
    
    /// <summary>
    /// English: Return the closest enemy basing on the unit vision by Physics.OverlapCapsule(). Return null if no enemy is detected.
    /// 日本語：Physics.OverlapCapsule()と視界の距離で最も近い敵を返す。敵がなければ、nullを返す。
    /// </summary>
    /// <param name="_rb"></param>
    /// <param name="_visionRange"></param>
    /// <returns></returns>
    public IEntity ReturnNearbyEnemy(Vector3 _curPosition, float _visionRange);
    

    /// <summary>
    /// English: Assign a target for Attack Action
    /// 日本語：攻撃の行動のターゲットを割り当てる。
    /// </summary>
    /// <param name="_target"></param>
    public void SetTarget(IEntity _target);

    

    

}

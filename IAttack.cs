using UnityEngine;

public interface IAttack : IMove
{
    public IEntity curTarget { get; set; }
    public Vector3 attackMousePosition { get; set; }

    public void AttackTarget(Rigidbody _rb, float _movementSpeed, float _attackDamage, float _attackRange, float _visionRange, bool _canAttack);

    public void AttackMove(Rigidbody _rb, float _movementSpeed, float _attackDamage, float _attackRange, float _visionRange, bool _canAttack);

    public IEntity ReturnNearbyEnemy(Rigidbody _rb, float _visionRange);

    public void AssignTarget(IEntity _target);

}

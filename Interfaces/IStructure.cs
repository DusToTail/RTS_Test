using UnityEngine;

public interface IStructure : IEntity
{
    public void MinusHealth(float _amount);
    public void PlusHealth(float _amount);

    public bool HealthIsZero();

    public void RenderSelectedCircle(bool _isOn);

    public void SetCurrentHealth(float _setAmount);
    public void SetCurrentAttackCooldown(float _setTime);

    public bool CanAttack();

    public float GetSetHealth();
    public float GetSetMovementSpeed();
    public float GetSetVisionRange();
    public float GetSetAttackDamage();
    public float GetSetAttackRange();
    public float GetSetAttackCooldown();
    public float GetSetBuildingTime();

    public float GetCurrentHealth();
    public float GetCurrentAttackCooldown();

    public GameObject GetSelectedCircle();

    public Collider GetCollider();
    public Rigidbody GetRigidbody();
}

using UnityEngine;

public interface IStructure : IEntity
{
    public void MinusHealth(float _amount);
    public void PlusHealth(float _amount);

    public bool HealthIsZero();

    public void RenderSelectedCircle(bool _isOn);

    public float GetSetHealth();
    public float GetSetVisionRange();
    public float GetSetBuildingTime();

    public float GetCurrentHealth();

    public GameObject GetSelectedCircle();

    public Rigidbody GetRigidbody();
    public Collider GetCollider();
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoldAction : IHold
{
    public IEntity target { get; set; }
    public bool isFinished { get; set; }

    private IEntity self;
    private SelectGroup group;
    private AttackController controller;

    public HoldAction(IEntity _self, SelectGroup _group, AttackController _controller)
    {
        isFinished = false;
        self = _self;
        group = _group;
        controller = _controller;
    }

    public void Execute()
    {

    }

    public void AttackTarget(Vector3 _curPosition, float _attackDamage, float _attackRange, float _visionRange)
    {
    }

    public IEntity ReturnNearbyEnemy(Vector3 _curPosition, float _visionRange)
    {
        Collider[] colliders = Physics.OverlapCapsule(_curPosition + Vector3.down * 50, _curPosition + Vector3.up * 50, _visionRange, LayerMask.GetMask(Tags.Selectable));
        List<IEntity> enemyList = new List<IEntity>();

        foreach (Collider collider in colliders)
        {
            if (collider.gameObject.GetComponent<IEntity>().GetPlayerIndex() != collider.gameObject.GetComponent<IEntity>().GetPlayerIndex())
            {
                enemyList.Add(collider.gameObject.GetComponent<IEntity>());
            }
        }

        float closestDistance = float.MaxValue;
        IEntity closestEnemy = null;

        foreach (IEntity curEnemy in enemyList)
        {
            float distance = Vector3.Distance(curEnemy.GetWorldPosition(), _curPosition);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestEnemy = curEnemy;
            }
        }

        return closestEnemy;
    }

    public void SetTarget(IEntity _target)
    {
        target = _target;
    }

    public void Stop()
    {
        isFinished = true;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Actions
{
    
    public static CancelAction GetCancelAction()
    {
        CancelAction action = new CancelAction();

        return action;
    }

    public static MoveAction GetMoveAction(IEntity _self, SelectGroup _group, MovementController _controller, Vector3 _curPosition, Vector3 _movePosition)
    {
        MoveAction action = new MoveAction(_self, _group, _controller, _curPosition, _movePosition);

        return action;
    }

    public static PatrolAction GetPatrolAction(IEntity _self, SelectGroup _group, MovementController _controller, Vector3 _curPosition, Vector3[] _waypoints)
    {
        PatrolAction action = new PatrolAction(_self, _group, _controller, _curPosition, _waypoints);

        return action;
    }

    public static AttackAction GetAttackAction(IEntity _self, SelectGroup _group, AttackController _controller, IEntity _target)
    {
        AttackAction action = new AttackAction(_self, _group, _controller, _target);

        return action;
    }

    public static AttackAction GetAttackAction(IEntity _self, SelectGroup _group, AttackController _controller, Vector3 _attackPosition)
    {
        AttackAction action = new AttackAction(_self, _group, _controller, _attackPosition);

        return action;
    }

    public static HoldAction GetHoldAction(IEntity _self, SelectGroup _group, AttackController _controller)
    {
        HoldAction action = new HoldAction(_self, _group, _controller);

        return action;
    }

    public static BuildAction GetBuildAction(GameObject _prefab, SelectGroup _group, Vector3 _spawnPosition, Vector3 _rallyPosition, float _buildTime)
    {
        BuildAction action = new BuildAction(_group, _prefab, _spawnPosition, _rallyPosition, _buildTime);

        return action;
    }

    public static UpgradeAction GetUpgradeAction(Upgrade _upgrade, SelectGroup _group, float _upgradeTime)
    {
        UpgradeAction action = new UpgradeAction(_group, _upgrade, _upgradeTime);

        return action;
    }
}

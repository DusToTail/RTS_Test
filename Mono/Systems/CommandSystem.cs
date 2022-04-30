using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// English：A class that contains commands (static methods) that mainly used by SelectSystem class (with mouse position, select group, etc), used to
///  create and enqueue action for each individual entity's ActionController class (each action will be run-time type) with in the group.
/// 日本語：SelectSystemクラス（マウスの位置やUnitグループなどと共に）で使用するコマンド（静的関数）をもつクラス。
/// コマンドは、グループ内のEntityごとに行動（run-timeタイプ）を作って、そのEntityのActionControllerクラスにEnqueueする。
/// </summary>
public class CommandSystem
{
    /// <summary>
    /// English: Command the group of entities to attack a target OR attack move towards the clicked position.
    /// 日本語：Entityグループを特定のターゲット、または指定した位置に攻撃していくことを指示する。
    /// </summary>
    /// <param name="_clickMousePosition"></param>
    /// <param name="_group"></param>
    /// <param name="_isInstant"></param>
    public static void AttackCommand(Vector3 _clickMousePosition, SelectGroup _group, bool _isInstant)
    {
        IEntity target = ReturnEntityAtWorldPosition(_clickMousePosition);
        if(target != null) 
        {
            Debug.Log($"Try to Attack {target.GetTransform().name}");
            AttackTargetCommand(target, _group, _isInstant); 
        }
        else 
        {
            Debug.Log($"Try to Attack Move to {_clickMousePosition}");
            AttackMoveCommand(_clickMousePosition, _group, _isInstant); 
        }
    }

    /// <summary>
    /// English: Command the group of entities to attack a target.
    /// 日本語：Entityグループを特定のターゲットを攻撃することを指示する。
    /// </summary>
    /// <param name="_target"></param>
    /// <param name="_group"></param>
    /// <param name="_isInstant"></param>
    public static void AttackTargetCommand(IEntity _target, SelectGroup _group, bool _isInstant)
    {
        if (_group == null) { return; }
        if (_target == null) { return; }

        if (_target != null)
        {
            Debug.Log($"Attack Enemy: {_target.GetTransform().gameObject.name}");

            for(int index = 0; index < _group.entityList.Count; index++)
            {
                dynamic newAction = _group.entityList[index].ReturnNewAction();
                newAction.InitializeType(Tags.AttackTargetInt);
                newAction.InitializeGroup(_group);
                newAction.InitializeAttackTarget(_target);

                _group.actionList.Add(newAction);

                _group.entityList[index].GetActionController().EnqueueAction(newAction, _isInstant);
            }
            _group.StartUpdateGroup(true);
        }
    }

    /// <summary>
    /// English: Command the group of entities to attack move towards the clicked position.
    /// 日本語：Entityグループを指定した位置に攻撃していくことを指示する。
    /// </summary>
    /// <param name="_destination"></param>
    /// <param name="_group"></param>
    /// <param name="_isInstant"></param>
    public static void AttackMoveCommand(Vector3 _destination, SelectGroup _group, bool _isInstant)
    {
        if (_group == null) { return; }

        // Check if the clicked position is traversible
        Debug.Log($"Attack Move Towards: {_destination}");

        for (int index = 0; index < _group.entityList.Count; index++)
        {
            dynamic newAction = _group.entityList[index].ReturnNewAction();
            newAction.InitializeType(3);
            newAction.InitializeGroup(_group);
            newAction.InitializeAttackMove(_group.entityList[index].GetWorldPosition(), _destination);

            _group.actionList.Add(newAction);

            _group.entityList[index].GetActionController().EnqueueAction(newAction, _isInstant);
        }
        _group.StartUpdateGroup(true);
    }


    /// <summary>
    /// English: Command the group of entities to patrol between its current location and the clicked position.
    /// 日本語：Entityグループを指定した位置と各々のEntityの現在の位置をパトロールすることを指示する。
    /// </summary>
    /// <param name="_moveMousePosition"></param>
    /// <param name="_group"></param>
    /// <param name="_isInstant"></param>
    public static void PatrolCommand(Vector3 _moveMousePosition, SelectGroup _group, bool _isInstant)
    {
        if(_group == null) { return; }

        // Check if there is any impassible obstacles in the way to account for an offset in the move position (the obstacle's size)
        IEntity entity = ReturnEntityAtWorldPosition(IEntity.EntityType.Structure, _moveMousePosition);
        if (entity == null)
        {
            Debug.Log($"Simple Patrol Towards {_moveMousePosition}");
            for (int index = 0; index < _group.entityList.Count; index++)
            {
                //Vector3 dir = _group.entityList[index].GetWorldPosition() - _group.entityList[index].GetWorldPosition();
                //Vector3 offset = new Vector3(-dir.x, 0, -dir.z).normalized * _group.entityList[index].GetSelectedCircleRadius();
                Vector3[] moveWaypoints = new Vector3[2];
                moveWaypoints[0] = _moveMousePosition;
                moveWaypoints[1] = _group.entityList[index].GetWorldPosition();

                dynamic newAction = _group.entityList[index].ReturnNewAction();
                newAction.InitializeType(Tags.PatrolInt);
                newAction.InitializeGroup(_group);
                newAction.InitializePatrol(_group.entityList[index].GetWorldPosition(), moveWaypoints);

                _group.actionList.Add(newAction);
                _group.entityList[index].GetActionController().EnqueueAction(newAction, _isInstant);
            }

            _group.StartUpdateGroup(true);
        }
        else
        {
            Debug.Log($"Move Towards {entity.GetTransform().gameObject.name}");
            for (int index = 0; index < _group.entityList.Count; index++)
            {
                Vector3 dir = entity.GetWorldPosition() - _group.entityList[index].GetWorldPosition();
                Vector3 offset = new Vector3(-dir.x, 0, -dir.z).normalized * entity.GetSelectedCircleRadius();
                Vector3[] moveWaypoints = new Vector3[2];
                moveWaypoints[0] = _moveMousePosition + offset;
                moveWaypoints[1] = _group.entityList[index].GetWorldPosition();

                dynamic newAction = _group.entityList[index].ReturnNewAction();
                newAction.InitializeType(Tags.PatrolInt);
                newAction.InitializeGroup(_group);
                newAction.InitializePatrol(_group.entityList[index].GetWorldPosition(), moveWaypoints);

                _group.actionList.Add(newAction);
                _group.entityList[index].GetActionController().EnqueueAction(newAction, _isInstant);
            }

            _group.StartUpdateGroup(true);
        }
    }

    /// <summary>
    /// English: Command the group of entities to move towards the clicked position.
    /// 日本語：Entityグループを指定した位置まで移動することを指示する。
    /// </summary>
    public static void MoveCommand(Vector3 _destinationPosition, SelectGroup _group, bool _isInstant)
    {
        if (_group == null) { return; }

        // Check if there is any impassible obstacles in the way to account for an offset in the move position (the obstacle's size)
        IEntity entity = ReturnEntityAtWorldPosition(IEntity.EntityType.Structure, _destinationPosition);
        if (entity == null)
        {
            Debug.Log($"Move Towards {_destinationPosition}");
            for (int index = 0; index < _group.entityList.Count; index++)
            {
                if (_group.entityList[index] == null) { continue; }

                dynamic newAction = _group.entityList[index].ReturnNewAction();
                newAction.InitializeType(Tags.MoveTowardsInt);
                newAction.InitializeGroup(_group);
                newAction.InitializeMoveTowards(_group.entityList[index].GetWorldPosition(), _destinationPosition);

                _group.actionList.Add(newAction);
                _group.entityList[index].GetActionController().EnqueueAction(newAction, _isInstant);
            }
            
            _group.StartUpdateGroup(true);
            return;
        }
        else
        {
            Debug.Log($"Move Towards {entity.GetTransform().gameObject.name}");
            for (int index = 0; index < _group.entityList.Count; index++)
            {
                Vector3 dir = entity.GetWorldPosition() - _group.entityList[index].GetWorldPosition();
                Vector3 offset = new Vector3(-dir.x, 0, -dir.z).normalized * entity.GetSelectedCircleRadius();

                dynamic newAction = _group.entityList[index].ReturnNewAction();
                newAction.InitializeType(Tags.MoveTowardsInt);
                newAction.InitializeGroup(_group);
                newAction.InitializeMoveTowards(_group.entityList[index].GetWorldPosition(), _destinationPosition + offset);

                _group.actionList.Add(newAction);
                _group.entityList[index].GetActionController().EnqueueAction(newAction, _isInstant);
            }

            _group.StartUpdateGroup(true);
            return;
        }
    }


    /// <summary>
    /// English: Return an entity with specified relationship type at the world position.
    /// 日本語：ワールド位置にあり、指定した関係性を持つEntityを返す。
    /// </summary>
    /// <param name="_relationshipType"></param>
    /// <param name="_worldPosition"></param>
    /// <returns></returns>
    public static IEntity ReturnEntityAtWorldPosition(IEntity.RelationshipType _relationshipType, Vector3 _worldPosition)
    {
        IEntity enemy = null;

        Collider[] colliders = Physics.OverlapBox(_worldPosition, Vector3.one + Vector3.up * 100, Quaternion.identity, LayerMask.GetMask(Tags.Selectable));
        foreach (Collider collider in colliders)
        {
            if (collider.gameObject.GetComponent<IEntity>().GetSelectionType() == IEntity.SelectionType.Selectable)
            {
                if (collider.gameObject.GetComponent<IEntity>().GetRelationshipType() == _relationshipType)
                {
                    enemy = collider.gameObject.GetComponent<IEntity>();
                    break;
                }
            }
        }

        return enemy;
    }

    /// <summary>
    /// English: Return an entity with specified entity type at the world position.
    /// 日本語：ワールド位置にあり、指定したentityタイプを持つEntityを返す。
    /// </summary>
    /// <param name="_entityType"></param>
    /// <param name="_worldPosition"></param>
    /// <returns></returns>
    public static IEntity ReturnEntityAtWorldPosition(IEntity.EntityType _entityType, Vector3 _worldPosition)
    {
        IEntity entity = null;
        Collider[] colliders = Physics.OverlapBox(_worldPosition, Vector3.one + Vector3.up * 100, Quaternion.identity, LayerMask.GetMask(Tags.Selectable));
        foreach (Collider collider in colliders)
        {
            if (collider.gameObject.GetComponent<IEntity>().GetSelectionType() == IEntity.SelectionType.Selectable)
            {
                Debug.Log("Structure found");
                if (collider.gameObject.GetComponent<IEntity>().GetEntityType() == _entityType)
                {
                    entity = collider.gameObject.GetComponent<IEntity>();
                    break;
                }
            }
        }

        return entity;
    }

    /// <summary>
    /// English: Return an entity at the world position.
    /// 日本語：マウスの位置にあるEntityを返す。
    /// </summary>
    /// <param name="_worldPosition"></param>
    /// <returns></returns>
    public static IEntity ReturnEntityAtWorldPosition(Vector3 _worldPosition)
    {
        IEntity entity = null;
        Collider[] colliders = Physics.OverlapBox(_worldPosition, Vector3.one + Vector3.up * 100, Quaternion.identity, LayerMask.GetMask(Tags.Selectable));
        foreach (Collider collider in colliders)
        {
            if (collider.gameObject.GetComponent<IEntity>().GetSelectionType() == IEntity.SelectionType.Selectable)
            {
                entity = collider.gameObject.GetComponent<IEntity>();
                break;
            }
        }

        return entity;
    }

    /// <summary>
    /// English: Return an obstacle at world position.
    /// 日本語：ワールド位置にある障害物を返す。
    /// </summary>
    /// <param name="_clickMousePosition"></param>
    /// <returns></returns>
    public static IObstacle ReturnObstacleAtWorldPosition(Vector3 _worldPosition)
    {
        IObstacle obstacle = null;
        Collider[] colliders = Physics.OverlapBox(_worldPosition, Vector3.one + Vector3.up * 100, Quaternion.identity, LayerMask.GetMask(Tags.Obstacle));
        foreach (Collider collider in colliders)
        {
            obstacle = collider.gameObject.GetComponent<IObstacle>();
            Debug.Log(collider.gameObject.name);
            break;
        }

        return obstacle;
    }


}

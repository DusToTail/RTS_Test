using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandSystem
{
    public static void AttackCommand(Vector3 _clickMousePosition, SelectGroup _group, bool _isInstant)
    {
        IEntity target = ReturnEntityAtMouse(_clickMousePosition);
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

    // 2
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
            //_group.AssignGroupTarget(_target);
            _group.StartUpdateGroup(true);
        }
        else
        {
            Debug.Log("No Target!");
        }

    }

    // 3
    public static void AttackMoveCommand(Vector3 _destination, SelectGroup _group, bool _isInstant)
    {
        if (_group == null) { return; }

        // Determine to Queue command OR Overwrite past commands by holding SHIFT or not

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

    
    // 1
    public static void PatrolCommand(Vector3 _moveMousePosition, SelectGroup _group, bool _isInstant)
    {
        if(_group == null) { return; }
        Collider[] colliders = Physics.OverlapBox(_moveMousePosition, Vector3.one + Vector3.up * 1000, Camera.main.transform.rotation, LayerMask.GetMask(Tags.Impassible_Terrain), QueryTriggerInteraction.Ignore);
        if (colliders.Length == 0)
        {
            Debug.Log($"Simple Patrol Towards {_moveMousePosition}");

            for (int index = 0; index < _group.entityList.Count; index++)
            {
                dynamic newAction = _group.entityList[index].ReturnNewAction();
                newAction.InitializeType(Tags.PatrolInt);
                newAction.InitializeGroup(_group);
                newAction.InitializePatrol(_group.entityList[index].GetWorldPosition(), _moveMousePosition, _group.entityList[index].GetWorldPosition());

                _group.actionList.Add(newAction);

                _group.entityList[index].GetActionController().EnqueueAction(newAction, _isInstant);
            }

            _group.StartUpdateGroup(true);
        }
        else
        {
            IEntity entity = ReturnEntityAtMouse(_moveMousePosition);
            if (entity != null)
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
            else
            {
                Debug.Log("Cant Move There!");
            }
        }
    }

    // 0
    /// <summary>
    /// Command the group of units to move towards a destination specified by mouse position
    /// </summary>
    public static void MoveCommand(Vector3 _destinationPosition, SelectGroup _group, bool _isInstant)
    {
        if (_group == null) { return; }

        // Check if the clicked position is traversible
        Collider[] colliders = Physics.OverlapBox(_destinationPosition, Vector3.one + Vector3.up * 1000, Camera.main.transform.rotation, LayerMask.GetMask(Tags.Impassible_Terrain), QueryTriggerInteraction.Ignore);
        if (colliders.Length == 0)
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
            IEntity entity = ReturnEntityAtMouse(_destinationPosition);
            if(entity != null)
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
            }
            else
            {
                Debug.Log("Cant Move There!");
            }
        }
    }



    public static IEntity ReturnEntityAtMouse(IEntity.RelationshipType _relationshipType, Vector3 _clickMousePosition)
    {
        // Check for enemy at mouse when right click
        IEntity enemy = null;

        Collider[] colliders = Physics.OverlapBox(_clickMousePosition, Vector3.one + Vector3.up * 100, Quaternion.identity, LayerMask.GetMask(Tags.Selectable));
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

    public static IEntity ReturnEntityAtMouse(IEntity.EntityType _entityType, Vector3 _clickMousePosition)
    {
        // Check for enemy at mouse when right click
        IEntity entity = null;

        Collider[] colliders = Physics.OverlapBox(_clickMousePosition, Vector3.one + Vector3.up * 100, Quaternion.identity, LayerMask.GetMask(Tags.Selectable));
        foreach (Collider collider in colliders)
        {
            if (collider.gameObject.GetComponent<IEntity>().GetSelectionType() == IEntity.SelectionType.Selectable)
            {
                if (collider.gameObject.GetComponent<IEntity>().GetEntityType() == _entityType)
                {
                    entity = collider.gameObject.GetComponent<IEntity>();
                    break;
                }
            }
        }

        return entity;
    }

    public static IEntity ReturnEntityAtMouse(Vector3 _clickMousePosition)
    {
        // Check for enemy at mouse when right click
        IEntity entity = null;

        Collider[] colliders = Physics.OverlapBox(_clickMousePosition, Vector3.one + Vector3.up * 100, Quaternion.identity, LayerMask.GetMask(Tags.Selectable));
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




}

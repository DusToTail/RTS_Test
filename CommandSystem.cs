using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandSystem
{
    public static void AttackCommand(Vector3 _clickMousePosition, SelectGroup _group)
    {
        IEntity enemy = ReturnEntityAtMouse(_clickMousePosition);

        if(enemy != null) { AttackTargetCommand(enemy, _group); }
        else { AttackMoveCommand(_clickMousePosition, _group); }
    }

    public static void AttackTargetCommand(IEntity _target, SelectGroup _group)
    {
        if (_group == null) { return; }
        if (_target == null) { return; }

        // Determine to Queue command OR Overwrite past commands by holding SHIFT or not
        bool isInstant = true;
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
            isInstant = false;

        // Check if the clicked position is traversible
        if (_target != null)
        {
            Debug.Log($"Attack Enemy: {_target.GetTransform().gameObject.name}");
        }
        else
        {
            Debug.Log("Cant Attack Target There!");
        }

    }

    public static void AttackMoveCommand(Vector3 _destination, SelectGroup _group)
    {
        if (_group == null) { return; }

        // Determine to Queue command OR Overwrite past commands by holding SHIFT or not
        bool isInstant = true;
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
            isInstant = false;


        // Check if the clicked position is traversible
        if (_group.groupFlowField.GetCellFromWorldPos(_destination).cost < byte.MaxValue)
        {
            Debug.Log($"Attack Move Towards: {_destination}");
        }
        else
        {
            Debug.Log("Cant Attack Target There!");
        }
    }

    public static IEntity ReturnEntityAtMouse(Vector3 _clickMousePosition)
    {
        // Check for enemy at mouse when right click
        IEntity enemy = null;

        Collider[] colliders = Physics.OverlapBox(_clickMousePosition, Vector3.one + Vector3.up * 100, Quaternion.identity, LayerMask.GetMask(Tags.Selectable));
        foreach (Collider collider in colliders)
        {
            if (collider.gameObject.GetComponent<IEntity>().GetSelectionType() == IEntity.SelectionType.Selectable)
            {
                if (collider.gameObject.GetComponent<IEntity>().GetRelationshipType() == IEntity.RelationshipType.Enemy)
                {
                    enemy = collider.gameObject.GetComponent<IEntity>();
                    break;
                }
            }
        }

        return enemy;
    }

    public static void PatrolCommand(Vector3 _destinationPosition, SelectGroup _group)
    {
        if(_group == null) { return; }

        // Determine to Queue command OR Overwrite past commands by holding SHIFT or not
        bool isInstant = true;
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
            isInstant = false;

        if (_group.groupFlowField.GetCellFromWorldPos(_destinationPosition).cost < byte.MaxValue)
        {
            Debug.Log($"Simple Patrol Towards {_destinationPosition}");
        }
        else
        {
            Debug.Log("Cant Move There!");
        }
    }

    /// <summary>
    /// Command the group of units to move towards a destination specified by mouse position
    /// </summary>
    public static void MoveCommand(Vector3 _destinationPosition, SelectGroup _group)
    {
        if (_group == null) { return; }

        // Determine to Queue command OR Overwrite past commands by holding SHIFT or not
        bool isInstant = true;
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
            isInstant = false;

        // Check if the clicked position is traversible
        if (_group.groupFlowField.GetCellFromWorldPos(_destinationPosition).cost < byte.MaxValue)
        {
            Debug.Log($"Move Towards {_destinationPosition}");
        }
        else
        {
            Debug.Log("Cant Move There!");
        }
    }











}

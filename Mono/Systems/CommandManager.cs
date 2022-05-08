using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// English：A class that contains commands (static methods) that mainly used by SelectSystem class (with mouse position, select group, etc), used to
///  create and enqueue action for each individual entity's ActionController class (each action will be run-time type) with in the group.
/// 日本語：SelectSystemクラス（マウスの位置やUnitグループなどと共に）で使用するコマンド（静的関数）をもつクラス。
/// コマンドは、グループ内のEntityごとに行動（run-timeタイプ）を作って、そのEntityのActionControllerクラスにEnqueueする。
/// </summary>
public class CommandManager : MonoBehaviour
{
    public enum CommandInt : int
    {
        None = 0,
        Move = 1,
        Patrol = 2,
        Attack = 3,
        Hold = 4,
        Stop = 5,

        Build = 100,
        Upgrade = 101,

        Cancel = 1000
    }


    public CommandInt commandMode { get; private set; }

    private void Start()
    {
        commandMode = CommandInt.None;
    }

    private void Update()
    {
        // ******* INPUT MODE *******
        {
            if (Input.GetKeyDown(Hotkeys.attackCommand))
            {
                SetCommandMode(CommandInt.Attack);
            }
            else if (Input.GetKeyDown(Hotkeys.moveCommand))
            {
                SetCommandMode(CommandInt.Move);
            }
            else if (Input.GetKeyDown(Hotkeys.patrolCommand))
            {
                SetCommandMode(CommandInt.Patrol);
            }
            else if (Input.GetKeyDown(Hotkeys.holdCommand))
            {
                SetCommandMode(CommandInt.Hold);
            }
            else if (Input.GetKeyDown(Hotkeys.stopCommand))
            {
                SetCommandMode(CommandInt.Stop);
            }
            else if (Input.GetKeyDown(Hotkeys.buildBasicCommand))
            {
                SetCommandMode(CommandInt.Build);
            }
            else if (Input.GetKeyDown(Hotkeys.buildAdvanceCommand))
            {
                SetCommandMode(CommandInt.Build);
            }
            else if (Input.GetKeyDown(Hotkeys.cancelCommand))
            {
                SetCommandMode(CommandInt.Cancel);
            }
        }
    }


    public void SetCommandMode(CommandInt _modeInt)
    {
        commandMode = _modeInt;
    }

    /// <summary>
    /// English: Command the group of entities to attack a target OR attack move towards the clicked position.
    /// 日本語：Entityグループを特定のターゲット、または指定した位置に攻撃していくことを指示する。
    /// </summary>
    /// <param name="_clickMousePosition"></param>
    /// <param name="_group"></param>
    /// <param name="_isQueued"></param>
    public static void GiveAttackCommand(Vector3 _clickMousePosition, SelectGroup _group, bool _isQueued)
    {
        IEntity target = Utilities.ReturnSelectableEntityAtWorldPosition(_clickMousePosition);
        if(target != null) 
        {
            Debug.Log($"Try to Attack {target.GetTransform().name}");
            GiveAttackTargetCommand(target, _group, _isQueued); 
        }
        else 
        {
            Debug.Log($"Try to Attack Move to {_clickMousePosition}");
            GiveAttackMoveCommand(_clickMousePosition, _group, _isQueued); 
        }
    }

    /// <summary>
    /// English: Command the group of entities to attack a target.
    /// 日本語：Entityグループを特定のターゲットを攻撃することを指示する。
    /// </summary>
    /// <param name="_target"></param>
    /// <param name="_group"></param>
    /// <param name="_isQueued"></param>
    public static void GiveAttackTargetCommand(IEntity _target, SelectGroup _group, bool _isQueued)
    {
        if (_group == null) { return; }
        if (_target == null) { return; }

        Debug.Log($"Attack Enemy: {_target.GetTransform().gameObject.name}");

        for(int index = 0; index < _group.entityList.Count; index++)
        {
            AttackController controller = _group.entityList[index].GetTransform().GetComponent<AttackController>();

            AttackAction action = Actions.GetAttackAction(_group.entityList[index], _group, controller, _target);

            _group.actionList.Add(action);
            _group.entityList[index].GetTransform().GetComponent<CommandHandler>().EnqueueCommand(action, _isQueued);
        }
        _group.StartUpdateGroup(true);
    }

    /// <summary>
    /// English: Command the group of entities to attack move towards the clicked position.
    /// 日本語：Entityグループを指定した位置に攻撃していくことを指示する。
    /// </summary>
    /// <param name="_destination"></param>
    /// <param name="_group"></param>
    /// <param name="_isQueued"></param>
    public static void GiveAttackMoveCommand(Vector3 _destination, SelectGroup _group, bool _isQueued)
    {
        if (_group == null) { return; }

        // Check if the clicked position is traversible
        Debug.Log($"Attack Move Towards: {_destination}");

        for (int index = 0; index < _group.entityList.Count; index++)
        {
            AttackController controller = _group.entityList[index].GetTransform().GetComponent<AttackController>();

            AttackAction action = Actions.GetAttackAction(_group.entityList[index], _group, controller, _destination);

            _group.actionList.Add(action);
            _group.entityList[index].GetTransform().GetComponent<CommandHandler>().EnqueueCommand(action, _isQueued);
        }
        _group.StartUpdateGroup(true);
    }


    /// <summary>
    /// English: Command the group of entities to patrol between its current location and the clicked position.
    /// 日本語：Entityグループを指定した位置と各々のEntityの現在の位置をパトロールすることを指示する。
    /// </summary>
    /// <param name="_moveMousePosition"></param>
    /// <param name="_group"></param>
    /// <param name="_isQueued"></param>
    public static void GivePatrolCommand(Vector3 _moveMousePosition, SelectGroup _group, bool _isQueued)
    {
        if(_group == null) { return; }

        // Check if there is any impassible obstacles in the way to account for an offset in the move position (the obstacle's size)
        IEntity entity = Utilities.ReturnSelectableEntityAtWorldPosition(_moveMousePosition);
        if (entity == null)
        {
            Debug.Log($"Simple Patrol Towards {_moveMousePosition}");
            for (int index = 0; index < _group.entityList.Count; index++)
            {
                Vector3[] moveWaypoints = new Vector3[2];
                moveWaypoints[0] = _moveMousePosition;
                moveWaypoints[1] = _group.entityList[index].GetWorldPosition();
                MovementController controller = _group.entityList[index].GetTransform().GetComponent<MovementController>();
                PatrolAction action = Actions.GetPatrolAction(_group.entityList[index], _group, controller, _group.entityList[index].GetWorldPosition(), moveWaypoints);

                _group.actionList.Add(action);
                _group.entityList[index].GetTransform().GetComponent<CommandHandler>().EnqueueCommand(action, _isQueued);

            }

            _group.StartUpdateGroup(true);
        }
        else
        {
            Debug.Log($"Patrol Towards {entity.GetTransform().gameObject.name}");
            for (int index = 0; index < _group.entityList.Count; index++)
            {
                Vector3 dir = entity.GetWorldPosition() - _group.entityList[index].GetWorldPosition();
                Vector3 offset = new Vector3(-dir.x, 0, -dir.z).normalized * entity.GetSelectedCircleRadius();
                Vector3[] moveWaypoints = new Vector3[2];
                moveWaypoints[0] = _moveMousePosition + offset;
                moveWaypoints[1] = _group.entityList[index].GetWorldPosition();
                MovementController controller = _group.entityList[index].GetTransform().GetComponent<MovementController>();
                PatrolAction action = Actions.GetPatrolAction(_group.entityList[index], _group, controller, _group.entityList[index].GetWorldPosition(), moveWaypoints);

                _group.actionList.Add(action);
                _group.entityList[index].GetTransform().GetComponent<CommandHandler>().EnqueueCommand(action, _isQueued);
            }

            _group.StartUpdateGroup(true);
        }
    }

    /// <summary>
    /// English: Command the group of entities to move towards the clicked position.
    /// 日本語：Entityグループを指定した位置まで移動することを指示する。
    /// </summary>
    /// <param name="_destinationPosition"></param>
    /// <param name="_group"></param>
    /// <param name="_isQueued"></param>
    public static void GiveMoveCommand(Vector3 _destinationPosition, SelectGroup _group, bool _isQueued)
    {
        if (_group == null) { return; }

        // Check if there is any impassible obstacles in the way to account for an offset in the move position (the obstacle's size)
        IEntity entity = Utilities.ReturnSelectableEntityAtWorldPosition(_destinationPosition);
        if (entity == null)
        {
            Debug.Log($"Move Towards {_destinationPosition}");
            for (int index = 0; index < _group.entityList.Count; index++)
            {
                if (_group.entityList[index].GetSelf() == null) { Debug.Log($"Entity in list at index {index} is null and skipped for command"); continue; }

                MovementController controller = _group.entityList[index].GetTransform().GetComponent<MovementController>();
                MoveAction action = Actions.GetMoveAction(_group.entityList[index], _group, controller, _group.entityList[index].GetWorldPosition(), _destinationPosition);
                _group.actionList.Add(action);
                _group.entityList[index].GetTransform().GetComponent<CommandHandler>().EnqueueCommand(action, _isQueued);
            }
            _group.StartUpdateGroup(true);
            return;
        }
        else
        {
            Debug.Log($"Move Towards {entity.GetTransform().gameObject.name}");
            for (int index = 0; index < _group.entityList.Count; index++)
            {
                if (_group.entityList[index].GetSelf() == null) { Debug.Log($"Entity in list at index {index} is null and skipped for command"); continue; }

                Vector3 dir = entity.GetWorldPosition() - _group.entityList[index].GetWorldPosition();
                Vector3 offset = new Vector3(-dir.x, 0, -dir.z).normalized * entity.GetSelectedCircleRadius();
                MovementController controller = _group.entityList[index].GetTransform().GetComponent<MovementController>();
                MoveAction action = Actions.GetMoveAction(_group.entityList[index], _group, controller, _group.entityList[index].GetWorldPosition(), _destinationPosition + offset);

                _group.actionList.Add(action);
                _group.entityList[index].GetTransform().GetComponent<CommandHandler>().EnqueueCommand(action, _isQueued);
            }
            _group.StartUpdateGroup(true);
            return;
        }
    }



}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// English: A class that has different types of actions that a Marine can take and those actions implementations.
/// 日本語：マリーンの行動の種類とそれらの実装を持つクラス。
/// </summary>
public class MarineAction : IMarineAction
{
    
    public enum Type : int
    {
        MoveTowards = 0,
        Patrol = 1,
        AttackTarget = 2,
        AttackMove = 3,
        StopMoving = 4,

        Stop = 5,
        Cancel = 6,
        None = 7
    }

    public Type type { get; set; }
    public List<IAction> subActions { get; set; }
    public bool isFinished { get; set; }
    public SelectGroup group { get; set; }

    public IEntity curTarget { get; set; }
    public Vector3 attackMousePosition { get; set; }

    public Vector3 moveMousePosition { get; set; }
    public Vector3[] moveWaypoints { get; set; }
    public FlowField curFlowField { get; set; }
    public FlowField selfFlowField { get; set; }
    public int curWaypointIndex { get; set; }

    private List<IEntity> obstacles;
    private float cellRadius;
    private Vector2Int gridSize;

    /// <summary>
    /// Default Constructor. When used, need separate initialization for each type of Action.
    /// 日本語：デフォルトコンストラクタ. 使用するとき、行動の種類によって個別の初期化が必要。
    /// </summary>
    public MarineAction()
    {
        //Debug.Log("Marine Action Created");
        type = Type.None;
        curTarget = null;
        attackMousePosition = Vector3.zero;
        moveMousePosition = Vector3.zero;
        moveWaypoints = null;
        selfFlowField = null;
        curFlowField = null;

        obstacles = new List<IEntity>();
        cellRadius = GameObject.FindObjectOfType<GridController>().cellRadius;
        gridSize = GameObject.FindObjectOfType<GridController>().gridSize;

        isFinished = false;
        group = null;
    }

    public void AttackMove(Rigidbody _rb, float _movementSpeed, float _attackDamage, float _attackRange, float _visionRange, bool _canAttack)
    {
        // Goal Check and Excution of other simpler action
        // 1st situation: if the destination is reached and there is no enemy nearby
        // 2nd situation: if there is enemy nearby
        // 3rd situation: if there is no target

        float destinationDistance = (_rb.position - attackMousePosition).magnitude;
        IEntity closestEnemy = ReturnNearbyEnemy(_rb, _visionRange);
        if (destinationDistance < cellRadius * 2 && closestEnemy == null && type == Type.AttackMove)
        {
            Debug.Log("Attack move reached destination and no enemy nearby");
            Debug.Log("Action is finished");
            StopMoving(_rb);
            Stop();
            return;
        }
        else if (closestEnemy != null)
        {
            if (curTarget != closestEnemy) { curTarget = closestEnemy; }

            AttackTarget(_rb, _movementSpeed, _attackDamage, _attackRange, _visionRange, _canAttack);

            //Debug.Log($"Trying to Attack {curTarget.name}");
        }
        else
        {
            InitializeMoveTowards(_rb.position, attackMousePosition);
            MoveTowards(_rb, _movementSpeed);
            //Debug.Log($"Move Towards destination: {curFlowField.destinationCell.gridPosition}");
        }
    }

    public void AttackTarget(Rigidbody _rb, float _movementSpeed, float _attackDamage, float _attackRange, float _visionRange, bool _canAttack)
    {
        // Check for target's validity and stop action if unvalid
        if (curTarget == null && type == Type.AttackTarget)
        {
            Debug.Log("Action is finished");
            Debug.Log("Action Attack target is null");
            StopMoving(_rb);
            Stop();
            return;
        }
        if(curTarget == _rb.GetComponent<IEntity>() && type == Type.AttackTarget)
        {
            Debug.Log("Action is finished");
            Debug.Log("Action Attack target is Self");
            StopMoving(_rb);
            Stop();
            return;
        }
        if (curTarget.GetSelectionType() == IEntity.SelectionType.UnSelectable && type == Type.AttackTarget)
        {
            Debug.Log("Action is finished");
            Debug.Log("Action Attack target is Unselectable");
            StopMoving(_rb);
            Stop();
            return;
        }

        // Move towards the target if outside range and return (no attacking)
        float distance = (curTarget.GetWorldPosition() - _rb.position).magnitude - curTarget.GetSelectedCircleRadius();
        if (distance > (float)_attackRange)
        {
            if(true)
            {
                Vector3 offsetDir = new Vector3(_rb.position.x - curTarget.GetWorldPosition().x, 0, _rb.position.z - curTarget.GetWorldPosition().z).normalized;
                InitializeMoveTowards(_rb.position, curTarget.GetWorldPosition() + offsetDir * curTarget.GetSelectedCircleRadius());
            }
            
            MoveTowards(_rb, _movementSpeed);
            return;
        }

        // Attack target if possible (there is cooldown being updated in Marine class, and reset to 0 here when attack)
        if (curTarget is IUnit curTargetUnit)
        {
            if (curTarget == null || curTargetUnit.HealthIsZero() && type == Type.AttackTarget)
            {
                Debug.Log("Action is finished");
                StopMoving(_rb);
                Stop();
                return;
            }

            if (_canAttack)
            {
                curTargetUnit.MinusHealth(_attackDamage);
                _rb.GetComponent<IUnit>().SetCurrentAttackCooldown(0);
                //Debug.Log($"{_rb.gameObject.name} Attack {curTarget.name} for  {amount}");
            }
        }
        else if (curTarget is IStructure curTargetStructure)
        {
            if (curTarget == null || curTargetStructure.HealthIsZero() && type == Type.AttackTarget)
            {
                Debug.Log("Action is finished");
                StopMoving(_rb);
                Stop();
                return;
            }

            if (_canAttack)
            {
                curTargetStructure.MinusHealth(_attackDamage);
                _rb.GetComponent<IUnit>().SetCurrentAttackCooldown(0);
                //Debug.Log($"{_rb.gameObject.name} Attack {curTarget.name} for  {amount}");
            }
        }
    }

    public IEntity ReturnNearbyEnemy(Rigidbody _rb, float _visionRange)
    {
        Collider[] colliders = Physics.OverlapCapsule(_rb.position + Vector3.down * 50, _rb.position + Vector3.up * 50, _visionRange, LayerMask.GetMask(Tags.Selectable));
        List<IEntity> enemyList = new List<IEntity>();

        foreach (Collider collider in colliders)
        {
            if (collider.gameObject.GetComponent<IEntity>().GetSelectionType() == IEntity.SelectionType.Selectable)
            {
                if (collider.gameObject.GetComponent<IEntity>().GetRelationshipType() == IEntity.RelationshipType.Enemy)
                {
                    enemyList.Add(collider.gameObject.GetComponent<IEntity>());
                }
            }
        }

        float closestDistance = float.MaxValue;
        IEntity closestEnemy = null;

        foreach (IEntity curEnemy in enemyList)
        {
            float distance = Vector3.Distance(curEnemy.GetWorldPosition(), _rb.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestEnemy = curEnemy;
            }
        }

        return closestEnemy;
    }

    public void Patrol(Rigidbody _rb, float _speed)
    {
        // Check if the entity has reached the destination to update current waypoint
        //Debug.Log(curWaypointIndex);
        float destinationDistance = new Vector3(_rb.position.x - moveWaypoints[curWaypointIndex].x, 0, _rb.position.z - moveWaypoints[curWaypointIndex].z).magnitude;
        //Debug.Log(destinationDistance);
        if (destinationDistance < cellRadius * 2)
        {
            if (curWaypointIndex == moveWaypoints.Length - 1)
                curWaypointIndex = 0;
            else
                curWaypointIndex++;

            InitializeMoveTowards(_rb.position, moveWaypoints[curWaypointIndex]);
        }
        MoveTowards(_rb, _speed);
    }

    public void MoveTowards(Rigidbody _rb, float _speed)
    {
        // Check if the entity has reached the destination or has reached the temporary position to update self flowfield
        float destinationDistance = new Vector3(_rb.position.x - moveMousePosition.x, 0, _rb.position.z - moveMousePosition.z).magnitude;
        float cellDistance = (curFlowField.GetCellFromWorldPos(_rb.position).gridPosition - curFlowField.destinationCell.gridPosition).magnitude;

        //Debug.Log(destinationDistance);
        if (destinationDistance < cellRadius * 2 && type == Type.MoveTowards)
        {
            Debug.Log("Action is finished");
            StopMoving(_rb);
            Stop();
            return;
        }
        else if(cellDistance < 1)
        {
            Vector2 dir = new Vector2(moveMousePosition.x - curFlowField.destinationCell.worldPosition.x, moveMousePosition.z - curFlowField.destinationCell.worldPosition.z).normalized;
            float distance = cellRadius * 2 * ((gridSize.x < gridSize.y) ? gridSize.y : gridSize.x);
            if(destinationDistance < distance)
                distance = destinationDistance;

            Vector3 nextPosition = _rb.position;
            for (int index = 0; index < distance / (cellRadius * 2); index++)
            {
                if (Utilities.ReturnObstacleAtWorldPosition(nextPosition) != null) 
                    break;
                nextPosition += new Vector3(dir.x, 0, dir.y).normalized * cellRadius * 2;
            }
            // Check for entity at the position to initialize the approriate flowfield
            Vector3 selfBound = _rb.gameObject.GetComponent<Collider>().bounds.extents;
            RaycastHit hit;
            Physics.BoxCast(_rb.position, selfBound, _rb.transform.forward, out hit, _rb.transform.rotation, distance, LayerMask.GetMask(Tags.Obstacle), QueryTriggerInteraction.Ignore);
            
            if(hit.collider == null)
            {
                InitializeSelfFlowField(_rb.position, nextPosition);
                AssignCurrentFlowField(selfFlowField);
                Debug.Log("Updated current flowfield without obstacle");
            }
            else
            {
                InitializeSelfFlowField(_rb.position, nextPosition, hit.collider.gameObject.GetComponent<IObstacle>());
                AssignCurrentFlowField(selfFlowField);
                Debug.Log("Updated current flowfield with obstacle");
            }
        }

        // Move with Rigidbody
        Vector3 flowFieldVector = GetFlowFieldDirection(_rb.position);
        //Vector3 alignmentVector = GetAlignmentDirection(_rb.position, 1f, 3f);
        //Vector3 separationVector = GetSeparationDirection(_rb.position, 1f, 3f);

        Vector3 moveDir = flowFieldVector;
        // rb.velocity cause other unit's transform to change slightly. Reason unknown
        // rb.velocity = moveDir.normalized * movementSpeed;
        if (moveDir == Vector3.zero) { return; }

        _rb.MovePosition(_rb.position + moveDir.normalized * _speed * Time.fixedDeltaTime);
        _rb.MoveRotation(Quaternion.LookRotation(moveDir.normalized));
    }

    public Vector3 GetFlowFieldDirection(Vector3 _curWorldPos)
    {
        // Validity Check
        if (curFlowField == null) { return Vector3.zero; }
        if (isFinished == true) { return Vector3.zero; }

        if (curFlowField.GetCellFromWorldPos(_curWorldPos) == curFlowField.destinationCell) { return Vector3.zero; }
        // Calculate direction from flowfield
        int x = curFlowField.GetCellFromWorldPos(_curWorldPos).bestDirection.Vector.x;
        int z = curFlowField.GetCellFromWorldPos(_curWorldPos).bestDirection.Vector.y;
        Vector3 flowFieldDir = new Vector3(x, 0, z);
        //Debug.Log(flowFieldDir);
        return flowFieldDir.normalized;
    }

    public Vector3 GetAlignmentDirection(Vector3 _curWorldPos, float _minDistance, float _maxDistance)
    {
        // Validity Check
        if (curFlowField == null) { return Vector3.zero; }
        if (isFinished == true) { return Vector3.zero; }

        Vector3 alignmentDir = new Vector3(0, 0, 0);

        int excludeCount = 0;
        for (int index = 0; index < group.entityList.Count; index++)
        {
            if (group.entityList[index] is IUnit)
            {
                if (group.actionList[index].isFinished == true) { excludeCount++; continue; }

                Vector3 curUnitPos = group.entityList[index].GetWorldPosition();
                Vector3 flowFieldVector = GetFlowFieldDirection(curUnitPos);
                float distance = Vector3.Distance(_curWorldPos, curUnitPos);
                float weight = ((_maxDistance - Mathf.Clamp(distance, _minDistance, _maxDistance))) / (_maxDistance - _minDistance);
                alignmentDir += flowFieldVector * weight;
            }
            else { excludeCount++; continue; }

        }
        alignmentDir = alignmentDir / (group.entityList.Count - excludeCount);
        //Debug.Log(alignmentDir);
        return alignmentDir.normalized;
    }

    public Vector3 GetSeparationDirection(Vector3 _curWorldPos, float _minDistance, float _maxDistance)
    {
        // Validity Check
        if (curFlowField == null) { return Vector3.zero; }
        if (isFinished == true) { return Vector3.zero; }

        Vector3 separationDir = new Vector3(0, 0, 0);

        int excludeCount = 0;
        for (int index = 0; index < group.entityList.Count; index++)
        {
            if (group.entityList[index] is IUnit)
            {
                if (group.actionList[index].isFinished == true) { excludeCount++; continue; }

                Vector3 curUnitPos = group.entityList[index].GetWorldPosition();
                Vector3 separationVector = _curWorldPos - curUnitPos;
                float weight = ((_maxDistance - Mathf.Clamp(separationVector.magnitude, _minDistance, _maxDistance))) / (_maxDistance - _minDistance);
                separationVector = weight * separationVector.normalized;
                separationDir += separationVector;
            }
            else { excludeCount++; continue; }

        }

        separationDir = separationDir / (group.entityList.Count - excludeCount);
        //Debug.Log(separationDir);
        return separationDir.normalized;
    }

    public void StopMoving(Rigidbody _rb)
    {
        if (_rb == null) { return; }
        _rb.velocity = Vector3.zero;
    }

    public void Cancel()
    {
        Stop();
    }

    public void Stop()
    {
        isFinished = true;
    }


    /// <summary>
    /// English: Initialize all necessary variables for Move Action
    /// 日本語：移動する行動の必要な変数を初期化する。
    /// </summary>
    /// <param name="_curPosition"></param>
    /// <param name="_moveMousePosition"></param>
    public void InitializeMoveTowards(Vector3 _curPosition, Vector3 _moveMousePosition)
    {
        // Check for entity at the position to initialize the approriate flowfield
        Vector2 dir = new Vector2(moveMousePosition.x - _curPosition.x, moveMousePosition.z - _curPosition.z).normalized;
        float distance = cellRadius * 2 * ((gridSize.x < gridSize.y) ? gridSize.y : gridSize.x);

        // Check for entity at the position to initialize the approriate flowfield
        RaycastHit hit;
        Physics.BoxCast(_curPosition, Vector3.one, new Vector3 (dir.x, 0, dir.y), out hit, Quaternion.identity, distance, LayerMask.GetMask(Tags.Obstacle), QueryTriggerInteraction.Ignore);

        if (hit.collider == null)
        {
            InitializeSelfFlowField(_curPosition, _moveMousePosition);
            Debug.Log("Updated current flowfield without obstacle");
        }
        else
        {
            InitializeSelfFlowField(_curPosition, _moveMousePosition, hit.collider.gameObject.GetComponent<IObstacle>());
            Debug.Log("Updated current flowfield with obstacle");
        }

        moveMousePosition = _moveMousePosition;

        AssignCurrentFlowField(selfFlowField);

        //GridDebug.SetCurFlowField(curFlowField);
        //GridDebug.DrawFlowField();
    }

    /// <summary>
    /// English: Initialize all necessary variables for Patrol Action
    /// 日本語：パトロールする行動の必要な変数を初期化する。
    /// </summary>
    /// <param name="_curPosition"></param>
    /// <param name="_moveWaypoints"></param>
    public void InitializePatrol(Vector3 _curPosition, Vector3[] _moveWaypoints)
    {
        // Check for entity at the position to initialize the approriate flowfield
        moveWaypoints = _moveWaypoints;
        IObstacle obstacle = Utilities.ReturnObstacleAtWorldPosition(moveWaypoints[0]);
        if (obstacle == null)
        {
            InitializeSelfFlowField(_curPosition, moveWaypoints[0]);
            Debug.Log("Updated current flowfield without obstacle");
        }
        else
        {
            InitializeSelfFlowField(_curPosition, moveWaypoints[0], obstacle);
            Debug.Log("Updated current flowfield with obstacle");
        }

        moveMousePosition = moveWaypoints[0];

        AssignCurrentFlowField(selfFlowField);

        curWaypointIndex = 0;
    }

    /// <summary>
    /// English: Initialize all necessary variables for Attack Target Action
    /// 日本語：ターゲットを追及しながら攻撃する行動の必要な変数を初期化する。
    /// </summary>
    /// <param name="_target"></param>
    public void InitializeAttackTarget(IEntity _target)
    {
        curTarget = _target;

    }

    /// <summary>
    /// English: Initialize all necessary variables for Attack Move Action
    /// 日本語：指定した場所へ移動しながら周りの敵を攻撃する行動の必要な変数を初期化する。
    /// </summary>
    /// <param name="_curPosition"></param>
    /// <param name="_attackMovePosition"></param>
    public void InitializeAttackMove(Vector3 _curPosition, Vector3 _attackMovePosition)
    {
        // Check for entity at the position to initialize the approriate flowfield
        InitializeSelfFlowField(_curPosition, _attackMovePosition);
        IObstacle obstacle = Utilities.ReturnObstacleAtWorldPosition(selfFlowField.destinationCell.worldPosition);
        if (obstacle == null)
        {
            Debug.Log("Updated current flowfield without obstacle");
        }
        else
        {
            InitializeSelfFlowField(_curPosition, _attackMovePosition, obstacle);
            Debug.Log("Updated current flowfield with obstacle");
        }

        attackMousePosition = _attackMovePosition;

        AssignCurrentFlowField(selfFlowField);
    }

    /// <summary>
    /// English: Creating a NEW self flowfield) with current position and the destination position in consideration
    /// 日本語：現在の位置と目的地で新しい独自のFlowFieldを作成する。
    /// </summary>
    /// <param name="_curPosition"></param>
    /// <param name="_destinationPosition"></param>
    public void InitializeSelfFlowField(Vector3 _curPosition, Vector3 _destinationPosition)
    {
        Vector2 vector = new Vector2(_destinationPosition.x - _curPosition.x, _destinationPosition.z - _curPosition.z);
        selfFlowField = new FlowField(_curPosition, vector, cellRadius, gridSize);
        selfFlowField.CreateGrid();
        selfFlowField.CreateCostField();
        Vector3 newDestinationPosition = _curPosition;
        for(int index = 0; index < vector.magnitude / (cellRadius * 2); index++)
        {
            if (selfFlowField.GetCellFromWorldPos(newDestinationPosition).cost == byte.MaxValue)
                break;
            newDestinationPosition += new Vector3(vector.x, 0, vector.y).normalized * cellRadius * 2;
        }
        selfFlowField.CreateIntegrationField(selfFlowField.GetCellFromWorldPos(newDestinationPosition));
        selfFlowField.CreateFlowField();

        GridDebug.SetCurFlowField(selfFlowField);
        //GridDebug.DrawFlowField();
    }

    /// <summary>
    /// English: Creating a NEW self flowfield) with current position, the destination position and obstacle in consideration
    /// 日本語：現在の位置と目的地と障害物で新しい独自のFlowFieldを作成する。
    /// </summary>
    /// <param name="_curPosition"></param>
    /// <param name="_obstacle"></param>
    public void InitializeSelfFlowField(Vector3 _curPosition, Vector3 _destinationPosition, IObstacle _obstacle)
    {
        Vector2 dir = new Vector2(_destinationPosition.x - _curPosition.x, _destinationPosition.z - _curPosition.z).normalized;
        

        selfFlowField = new FlowField(_obstacle.GetWorldPosition(), cellRadius, _obstacle.GetObstacleGridSize() + gridSize);
        selfFlowField.CreateGrid();
        selfFlowField.CreateCostField();
        Vector3 newDestinationPosition = _curPosition;
        if (selfFlowField.GetCellFromWorldPos(newDestinationPosition + new Vector3(dir.x, 0, dir.y).normalized * cellRadius * 2).cost == byte.MaxValue)
        {
            float distance = Mathf.Sqrt(_obstacle.GetObstacleGridSize().x * _obstacle.GetObstacleGridSize().x + _obstacle.GetObstacleGridSize().y * _obstacle.GetObstacleGridSize().y);
            newDestinationPosition += new Vector3(dir.x, 0, dir.y).normalized * distance * cellRadius * 2;
        }
        else
        {
            newDestinationPosition += new Vector3(dir.x, 0, dir.y).normalized * cellRadius * 2;
        }

        selfFlowField.CreateIntegrationField(selfFlowField.GetCellFromWorldPos(newDestinationPosition));
        selfFlowField.CreateFlowField();
        GridDebug.SetCurFlowField(selfFlowField);
        //GridDebug.DrawFlowField();
    }

    /// <summary>
    /// English: Initialize (Assign) the type of the Action
    /// 日本語：行動のタイプを初期化（割り当てる）。
    /// </summary>
    /// <param name="_type"></param>
    public void InitializeType(int _type) { type = (Type)_type; }

    /// <summary>
    /// English: Initialize (Assign) the group that the action (also the entity) is in
    /// 日本語：所属しているグループを初期化（割り当てる）。
    /// </summary>
    /// <param name="_group"></param>
    public void InitializeGroup(SelectGroup _group) { group = _group; }


    public void AssignCurrentFlowField(FlowField _flowField)
    {
        curFlowField = _flowField;
    }
    
    public void AssignSelfFlowField(FlowField _flowField)
    {
        selfFlowField = _flowField;
    }

    public void AssignMoveMousePosition(Vector3 _vector)
    {
        moveMousePosition = _vector;
    }

    public void AssignWaypointPosition(Vector3[] _vectors)
    {
        moveWaypoints = _vectors;
    }

    public void AssignTarget(IEntity _target)
    {
        curTarget = _target;
    }

    public void AssignSubActions(List<IAction> _actions)
    {
        if (_actions == null) { return; }
        subActions = _actions;
    }
    public List<IAction> GetSubActions() { return subActions; }

    public SelectGroup GetGroup() { return group; }
    public dynamic GetSelf() { return this; }

}

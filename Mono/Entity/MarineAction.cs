using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    public int curFlowFieldIndex { get; set; }

    

    public MarineAction()
    {
        Debug.Log("Marine Action Created");
        type = Type.None;
        curTarget = null;
        attackMousePosition = Vector3.zero;
        moveMousePosition = Vector3.zero;
        moveWaypoints = null;
        selfFlowField = null;
        curFlowField = null;

        isFinished = false;
        group = null;
    }


    public void AttackMove(Rigidbody _rb, float _movementSpeed, float _attackDamage, float _attackRange, float _visionRange, bool _canAttack)
    {
        float cellDistance = (group.groupFlowField.GetCellFromWorldPos(_rb.position).gridPosition - group.groupFlowField.GetCellFromWorldPos(attackMousePosition).gridPosition).magnitude;

        // Goal Check and Excution of other simpler action
        // 1st situation: if the destination is reached and there is no enemy nearby
        // 2nd situation: if there is enemy nearby
        // 3rd situation: if there is no target
        IEntity closestEnemy = ReturnNearbyEnemy(_rb, _visionRange);
        if (cellDistance < 1 && closestEnemy == null)
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

            if (selfFlowField.GetCellFromWorldPos(curTarget.GetWorldPosition()).gridPosition != selfFlowField.destinationCell.gridPosition)
            {
                InitializeSelfFlowField(_rb.position, curTarget.GetWorldPosition());
                AssignCurrentFlowField(selfFlowField);
            }

            // MOSTLY Attack Target Implementation
            // Move if outside range and return (no attacking)
            float distance = (curTarget.GetWorldPosition() - _rb.position).magnitude - curTarget.GetSelectedCircleRadius();
            if (distance > (float)_attackRange)
            {
                // Move with Rigidbody
                Vector3 flowFieldVector = GetFlowFieldDirection(_rb.position);
                Vector3 alignmentVector = GetAlignmentDirection(_rb.position, 1f, 3f);
                Vector3 separationVector = GetSeparationDirection(_rb.position, 1f, 3f);

                Vector3 moveDir = flowFieldVector + alignmentVector + separationVector;
                // rb.velocity cause other unit's transform to change slightly. Reason unknown
                // rb.velocity = moveDir.normalized * movementSpeed;
                if (moveDir == Vector3.zero) { return; }

                _rb.MovePosition(_rb.position + moveDir.normalized * _movementSpeed * Time.fixedDeltaTime);
                _rb.MoveRotation(Quaternion.LookRotation(moveDir.normalized));
                return;
            }

            if (curTarget.GetSelectionType() == IEntity.SelectionType.Selectable)
            {
                if (curTarget is IUnit curTargetUnit)
                {
                    if (curTarget == null || curTargetUnit.HealthIsZero())
                    {
                        return;
                    }

                    if (_canAttack)
                    {
                        curTargetUnit.MinusHealth(_attackDamage);
                        _rb.GetComponent<IUnit>().SetCurrentAttackCooldown(0);
                        //Debug.Log($"{_rb.gameObject.name} Attack {curTarget.name} for  {damageAmount}");
                    }
                }
                else if (curTarget is IStructure curTargetStructure)
                {
                    if (curTarget == null || curTargetStructure.HealthIsZero())
                    {
                        return;
                    }

                    if (_canAttack)
                    {
                        curTargetStructure.MinusHealth(_attackDamage);
                        _rb.GetComponent<IUnit>().SetCurrentAttackCooldown(0);
                        //Debug.Log($"{_rb.gameObject.name} Attack {curTarget.name} for  {damageAmount}");
                    }
                }
            }

            //Debug.Log($"Trying to Attack {curTarget.name}");
        }
        else
        {
            if (curFlowField.destinationCell.gridPosition != group.groupFlowField.destinationCell.gridPosition)
            {
                AssignCurrentFlowField(group.groupFlowField);
            }
            MoveTowards(_rb, _movementSpeed);
            //Debug.Log($"Move Towards destination: {curFlowField.destinationCell.gridPosition}");
        }
    }

    public void AttackTarget(Rigidbody _rb, float _movementSpeed, float _attackDamage, float _attackRange, float _visionRange, bool _canAttack)
    {
        if (curTarget == null)
        {
            Debug.Log("Action is finished");
            Debug.Log("Action Attack target is null");
            StopMoving(_rb);
            Stop();
            return;
        }
        if(curTarget == _rb.GetComponent<IEntity>())
        {
            Debug.Log("Action is finished");
            Debug.Log("Action Attack target is Self");
            StopMoving(_rb);
            Stop();
            return;
        }
        if (curTarget.GetSelectionType() == IEntity.SelectionType.UnSelectable)
        {
            Debug.Log("Action is finished");
            Debug.Log("Action Attack target is Unselectable");
            StopMoving(_rb);
            Stop();
            return;
        }

        // Move if outside range and return (no attacking)
        float distance = (curTarget.GetWorldPosition() - _rb.position).magnitude - curTarget.GetSelectedCircleRadius();
        if (distance > (float)_attackRange)
        {
            // Since group flow field is reinitialize every frame also
            AssignCurrentFlowField(group.groupFlowField);

            // Move with Rigidbody
            Vector3 flowFieldVector = GetFlowFieldDirection(_rb.position);
            Vector3 alignmentVector = GetAlignmentDirection(_rb.position, 1f, 3f);
            Vector3 separationVector = GetSeparationDirection(_rb.position, 1f, 3f);
            //Debug.Log(flowFieldVector);
            //Debug.Log(alignmentVector);
            //Debug.Log(separationVector);
            //Debug.Log(curFlowField.destinationCell.gridPosition);
            //Debug.Log(curFlowField.GetCellFromWorldPos(_rb.position).gridPosition);

            Vector3 moveDir = flowFieldVector + alignmentVector + separationVector;
            // rb.velocity cause other unit's transform to change slightly. Reason unknown
            // rb.velocity = moveDir.normalized * movementSpeed;
            if (moveDir == Vector3.zero) { return; }

            _rb.MovePosition(_rb.position + moveDir.normalized * _movementSpeed * Time.fixedDeltaTime);
            _rb.MoveRotation(Quaternion.LookRotation(moveDir.normalized));
            return;
        }


        if (curTarget is IUnit curTargetUnit)
        {
            if (curTarget == null || curTargetUnit.HealthIsZero())
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
            if (curTarget == null || curTargetStructure.HealthIsZero())
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
        // Goal Check
        float cellDistance = (curFlowField.GetCellFromWorldPos(_rb.position).gridPosition - curFlowField.destinationCell.gridPosition).magnitude;
        if (cellDistance < 1)
        {
            if (curFlowField == selfFlowField)
            {
                AssignCurrentFlowField(group.groupFlowField);
            }
            else
            {
                AssignCurrentFlowField(selfFlowField);
            }
        }

        // Move with Rigidbody
        Vector3 flowFieldVector = GetFlowFieldDirection(_rb.position);
        Vector3 alignmentVector = GetAlignmentDirection(_rb.position, 1f, 3f);
        Vector3 separationVector = GetSeparationDirection(_rb.position, 1f, 3f);


        Vector3 moveDir = flowFieldVector + alignmentVector + separationVector;
        // rb.velocity cause other unit's transform to change slightly. Reason unknown
        // rb.velocity = moveDir.normalized * movementSpeed;
        if (moveDir == Vector3.zero) { return; }

        _rb.MovePosition(_rb.position + moveDir.normalized * _speed * Time.fixedDeltaTime);
        _rb.MoveRotation(Quaternion.LookRotation(moveDir.normalized));
    }

    public void MoveTowards(Rigidbody _rb, float _speed)
    {
        float cellDistance = (curFlowField.GetCellFromWorldPos(_rb.position).gridPosition - curFlowField.destinationCell.gridPosition).magnitude;
        // Goal Check
        if (cellDistance < 1)
        {
            Debug.Log("Action is finished");
            StopMoving(_rb);
            Stop();
            return;
        }

        // Move with Rigidbody
        Vector3 flowFieldVector = GetFlowFieldDirection(_rb.position);
        Vector3 alignmentVector = GetAlignmentDirection(_rb.position, 1f, 3f);
        Vector3 separationVector = GetSeparationDirection(_rb.position, 1f, 3f);

        Vector3 moveDir = flowFieldVector + alignmentVector + separationVector;
        // rb.velocity cause other unit's transform to change slightly. Reason unknown
        // rb.velocity = moveDir.normalized * movementSpeed;
        if (moveDir == Vector3.zero) { return; }

        _rb.MovePosition(_rb.position + moveDir.normalized * _speed * Time.fixedDeltaTime);
        _rb.MoveRotation(Quaternion.LookRotation(moveDir.normalized));
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
    public Vector3 GetFlowFieldDirection(Vector3 _curWorldPos)
    {
        // Validity Check
        if (curFlowField == null) { return Vector3.zero; }
        if (isFinished == true) { return Vector3.zero; }

        if (curFlowField.GetCellFromWorldPos(_curWorldPos) == curFlowField.destinationCell) { return Vector3.zero; }
        //Debug.Log(curFlowField.GetCellFromWorldPos(_curWorldPos).bestDirection.Vector);
        // Calculate direction from flowfield
        int x = curFlowField.GetCellFromWorldPos(_curWorldPos).bestDirection.Vector.x;
        int z = curFlowField.GetCellFromWorldPos(_curWorldPos).bestDirection.Vector.y;
        Vector3 flowFieldDir = new Vector3(x, 0, z);
        //Debug.Log(flowFieldDir);
        return flowFieldDir.normalized;
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


    public void InitializeMoveTowards(Vector3 _moveMousePosition, FlowField _flowField)
    {
        moveMousePosition = _moveMousePosition;
        curFlowField = _flowField;
        curFlowField.CreateIntegrationField(curFlowField.GetCellFromWorldPos(_moveMousePosition));
        curFlowField.CreateFlowField();
    }
    public void InitializePatrol(Vector3 _moveMousePosition, Vector3 _curPosition, FlowField _flowField)
    {
        InitializeSelfFlowField(_curPosition, _moveMousePosition);

        curFlowField = _flowField;
        curFlowField.CreateIntegrationField(curFlowField.GetCellFromWorldPos(_moveMousePosition));
        curFlowField.CreateFlowField();
        curFlowFieldIndex = 0;
    }
    public void InitializeAttackTarget(IEntity _target, FlowField _flowField)
    {
        curTarget = _target;

        curFlowField = _flowField;
    }
    public void InitializeAttackMove(Vector3 _curPosition, Vector3 _attackMovePosition, FlowField _flowField)
    {
        attackMousePosition = _attackMovePosition;

        InitializeSelfFlowField(_curPosition, _attackMovePosition);

        curFlowField = _flowField;
        curFlowField.CreateIntegrationField(curFlowField.GetCellFromWorldPos(_attackMovePosition));
        curFlowField.CreateFlowField();
    }
    public void InitializeSelfFlowField(Vector3 _curPosition, Vector3 _destinationPosition)
    {
        Vector2 dir = new Vector2(_destinationPosition.x - _curPosition.x, _destinationPosition.z - _curPosition.z);
        selfFlowField = new FlowField(_curPosition, dir, GameObject.FindObjectOfType<GridController>().cellRadius, GameObject.FindObjectOfType<GridController>().gridSize);
        selfFlowField.CreateGrid();
        selfFlowField.CreateCostField();
        selfFlowField.CreateIntegrationField(selfFlowField.GetCellFromWorldPos(_destinationPosition));
        selfFlowField.CreateFlowField();
    }

    public void InitializeType(int _type) { type = (Type)_type; }
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

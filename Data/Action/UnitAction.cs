using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitAction
{
    /// <summary>
    /// DESCRIPTION: An Action is used by ActionController (component of a Unit gameObject) to determine what to do.
    /// An action is unique for each unit and for each declaration.
    /// It is run on FixedUpdate()
    /// </summary>
    
    //Different actions can be taken. Used in ActionController's [CommitCurrentAction] method as an argument in switch statement
    public enum ActionTypes
    {
        MoveTowards,
        Patrol,
        AttackTarget,
        AttackMove,
        StopAction,
        None
    }

    public ActionTypes type;

    //To determine if the action is finished (if move, then destination is reached).
    //Used in SelectGroup to see if all refering actions is finished to self-destruct.
    //Used in ActionController component to see if it should Dequeue the next action or not.
    public bool isFinished;

    public Vector3 mousePosition;

    //The select group (containing a flow field) to navigate with
    //For single destination movement
    public GameObject curSelectGroup;
    //For multiple destinations movement
    public FlowField curFlowField;
    public FlowField selfFlowField;
    public GameObject curTarget;

    private int curFlowFieldIndex;


    public UnitAction(ActionTypes _type)
    {
        type = _type;
        isFinished = false;
        mousePosition = Vector3.zero;
        curSelectGroup = null;
        curFlowField = null;
        curFlowFieldIndex = 0;
    }

    public void AttackMove(Unit _unit, float _movementSpeed, float _visionRange, float _damageAmount, float _attackRange, bool _canAttack)
    {
        Collider[] colliders = Physics.OverlapCapsule(_unit.GetRigidbody().position + Vector3.down * 50, _unit.GetRigidbody().position + Vector3.up * 50, _visionRange, LayerMask.GetMask(Tags.Selectable));
        List<GameObject> enemyList = new List<GameObject>();

        foreach (Collider collider in colliders)
        {
            if (collider.gameObject.GetComponent<IEntity>().GetSelectionType() == IEntity.SelectionType.Selectable)
            {
                if (collider.gameObject.GetComponent<IEntity>().GetRelationshipType() == IEntity.RelationshipType.Enemy)
                {
                    enemyList.Add(collider.gameObject);
                }
            }
        }

        float closestDistance = float.MaxValue;
        GameObject closestEnemy = null;

        foreach(GameObject curEnemy in enemyList)
        {
            float distance = Vector3.Distance(curEnemy.transform.position, curEnemy.transform.forward);
            if(distance < closestDistance)
            {
                closestDistance = distance;
                closestEnemy = curEnemy;
            }
        }

        float cellDistance = (curFlowField.GetCellFromWorldPos(_unit.GetRigidbody().position).gridPosition - curFlowField.GetCellFromWorldPos(mousePosition).gridPosition).magnitude;

        // Goal Check and Excution of other simpler action
        // 1st situation: if the destination is reached and there is no enemy nearby
        // 2nd situation: if there is enemy nearby
        // 3rd situation: if there is no target
        if (cellDistance < 1 && closestEnemy == null)
        {
            Debug.Log("Attack move reached destination and no enemy nearby");
            Debug.Log("Action is finished");
            StopMoving(_unit);
            FinishAction();
            return;
        }
        else if(closestEnemy != null)
        {
            if(curTarget != closestEnemy)
            {
                curTarget = closestEnemy;
            }

            if(selfFlowField.GetCellFromWorldPos(curTarget.GetComponent<Rigidbody>().position).gridPosition != selfFlowField.destinationCell.gridPosition)
            {
                InitializeSelfFlowField(curTarget.GetComponent<Rigidbody>().position);
                InitializeCurrentFlowField(selfFlowField);
            }

            // MOSTLY Attack Target Implementation
            // Move if outside range and return (no attacking)
            float distance = (curTarget.GetComponent<Rigidbody>().position - _unit.GetRigidbody().position).magnitude;
            if (distance > (float)_attackRange)
            {
                // Move with Rigidbody
                Vector3 flowFieldVector = GetFlowFieldVector(_unit.GetRigidbody().position) * GetFlowFieldWeight(_unit.GetRigidbody().position);
                Vector3 alignmentVector = GetAlignmentVector(_unit.GetRigidbody().position, 1f, 3f) * GetAlignmentWeight(_unit.GetRigidbody().position, 1f);
                Vector3 separationVector = GetSeparationVector(_unit.GetRigidbody().position, 1f, 3f) * GetSeparationWeight(_unit.GetRigidbody().position, 1f);

                Vector3 moveDir = flowFieldVector + alignmentVector + separationVector;
                // rb.velocity cause other unit's transform to change slightly. Reason unknown
                // rb.velocity = moveDir.normalized * movementSpeed;
                if (moveDir == Vector3.zero) { return; }

                _unit.GetRigidbody().MovePosition(_unit.GetRigidbody().position + moveDir.normalized * _movementSpeed * Time.deltaTime);
                _unit.GetRigidbody().MoveRotation(Quaternion.LookRotation(moveDir.normalized));
                return;
            }

            if(curTarget.GetComponent<IEntity>().GetSelectionType() == IEntity.SelectionType.Selectable)
            {
                if(curTarget.GetComponent<IEntity>().GetEntityType() == IEntity.EntityType.Unit)
                {
                    if (curTarget == null || curTarget.GetComponent<Unit>().HealthIsZero())
                    {
                        Debug.Log("Action is finished");
                        StopMoving(_unit);
                        FinishAction();
                        return;
                    }

                    if (_canAttack)
                    {
                        curTarget.GetComponent<Unit>().MinusHealth(_damageAmount);
                        _unit.SetCurrentAttackCooldown(0);
                        //Debug.Log($"Attack {curTarget.name} for  {damageAmount}");
                    }
                }
                else if(curTarget.GetComponent<IEntity>().GetEntityType() == IEntity.EntityType.Structure)
                {
                    if (curTarget == null || curTarget.GetComponent<Structure>().HealthIsZero())
                    {
                        Debug.Log("Action is finished");
                        StopMoving(_unit);
                        FinishAction();
                        return;
                    }

                    if (_canAttack)
                    {
                        curTarget.GetComponent<Structure>().MinusHealth(_damageAmount);
                        _unit.SetCurrentAttackCooldown(0);
                        //Debug.Log($"Attack {curTarget.name} for  {damageAmount}");
                    }
                }
            }

            //Debug.Log($"Trying to Attack {curTarget.name}");
        }
        else if(curTarget != null)
        {
            if (selfFlowField.GetCellFromWorldPos(curTarget.GetComponent<Rigidbody>().position).gridPosition != selfFlowField.destinationCell.gridPosition)
            {
                InitializeSelfFlowField(curTarget.GetComponent<Rigidbody>().position);
                InitializeCurrentFlowField(selfFlowField);
            }

            // MOSTLY Attack Target Implementation
            // Move if outside range and return (no attacking)
            float distance = (curTarget.GetComponent<Rigidbody>().position - _unit.GetRigidbody().position).magnitude;
            if (distance > (float)_attackRange)
            {
                // Move with Rigidbody
                Vector3 flowFieldVector = GetFlowFieldVector(_unit.GetRigidbody().position) * GetFlowFieldWeight(_unit.GetRigidbody().position);
                Vector3 alignmentVector = GetAlignmentVector(_unit.GetRigidbody().position, 1f, 3f) * GetAlignmentWeight(_unit.GetRigidbody().position, 1f);
                Vector3 separationVector = GetSeparationVector(_unit.GetRigidbody().position, 1f, 3f) * GetSeparationWeight(_unit.GetRigidbody().position, 1f);

                Vector3 moveDir = flowFieldVector + alignmentVector + separationVector;
                // rb.velocity cause other unit's transform to change slightly. Reason unknown
                // rb.velocity = moveDir.normalized * movementSpeed;
                if (moveDir == Vector3.zero) { return; }

                _unit.GetRigidbody().MovePosition(_unit.GetRigidbody().position + moveDir.normalized * _movementSpeed * Time.deltaTime);
                _unit.GetRigidbody().MoveRotation(Quaternion.LookRotation(moveDir.normalized));
                return;
            }

            if(curTarget.GetComponent<IEntity>().GetSelectionType() == IEntity.SelectionType.Selectable)
            {
                if(curTarget.GetComponent<IEntity>().GetEntityType() == IEntity.EntityType.Unit)
                {
                    if (curTarget == null || curTarget.GetComponent<Unit>().HealthIsZero())
                    {
                        Debug.Log("Action is finished");
                        StopMoving(_unit);
                        FinishAction();
                        return;
                    }

                    if (_canAttack)
                    {
                        curTarget.GetComponent<Unit>().MinusHealth(_damageAmount);
                        _unit.SetCurrentAttackCooldown(0);
                        Debug.Log($"Attack {curTarget.name} for  {_damageAmount}");
                    }
                }
                else if(curTarget.GetComponent<IEntity>().GetEntityType() == IEntity.EntityType.Structure)
                {
                    if (curTarget == null || curTarget.GetComponent<Structure>().HealthIsZero())
                    {
                        Debug.Log("Action is finished");
                        StopMoving(_unit);
                        FinishAction();
                        return;
                    }

                    if (_canAttack)
                    {
                        curTarget.GetComponent<Structure>().MinusHealth(_damageAmount);
                        _unit.SetCurrentAttackCooldown(0);
                        //Debug.Log($"Attack {curTarget.name} for  {damageAmount}");
                    }
                }
            }

            //Debug.Log($"Trying to Attack {curTarget.name}");
        }
        else
        {
            if(curFlowField.destinationCell.gridPosition != curSelectGroup.GetComponent<SelectGroup>().groupFlowField.destinationCell.gridPosition)
            {
                InitializeSelfFlowField(mousePosition);
                InitializeCurrentFlowField(selfFlowField);
            }
            MoveInFlowField(_unit, _movementSpeed, false);
            //Debug.Log($"Move Towards destination: {curFlowField.destinationCell.gridPosition}");
        }

    }

    public void Patrol(Unit _unit, float _movementSpeed)
    {
        // Goal Check
        float cellDistance = (curFlowField.GetCellFromWorldPos(_unit.GetRigidbody().position).gridPosition - curFlowField.destinationCell.gridPosition).magnitude;
        if (cellDistance < 1)
        {
            if(curFlowFieldIndex >= curSelectGroup.GetComponent<SelectGroup>().flowFieldList.Count)
            {
                InitializeCurrentFlowField(selfFlowField);
                curFlowFieldIndex = 0;
            }
            else
            {
                if(curFlowFieldIndex >= curSelectGroup.GetComponent<SelectGroup>().flowFieldList.Count - 1)
                    InitializeCurrentFlowField(curSelectGroup.GetComponent<SelectGroup>().flowFieldList[0]);
                else
                    InitializeCurrentFlowField(curSelectGroup.GetComponent<SelectGroup>().flowFieldList[curFlowFieldIndex + 1]);

                curFlowFieldIndex++;
            }
        }

        // Move with Rigidbody
        Vector3 flowFieldVector = GetFlowFieldVector(_unit.GetRigidbody().position) * GetFlowFieldWeight(_unit.GetRigidbody().position);
        Vector3 alignmentVector = GetAlignmentVector(_unit.GetRigidbody().position, 1f, 3f) * GetAlignmentWeight(_unit.GetRigidbody().position, 1f);
        Vector3 separationVector = GetSeparationVector(_unit.GetRigidbody().position, 1f, 3f) * GetSeparationWeight(_unit.GetRigidbody().position, 1f);


        Vector3 moveDir = flowFieldVector + alignmentVector + separationVector;
        // rb.velocity cause other unit's transform to change slightly. Reason unknown
        // rb.velocity = moveDir.normalized * movementSpeed;
        if (moveDir == Vector3.zero) { return; }

        _unit.GetRigidbody().MovePosition(_unit.GetRigidbody().position + moveDir.normalized * _movementSpeed * Time.deltaTime);
        _unit.GetRigidbody().MoveRotation(Quaternion.LookRotation(moveDir.normalized));
    }


    public void AttackTarget(Unit _unit, float _movementSpeed, float _damageAmount, float _attackRange, bool _canAttack, bool _finishConditionIsOn)
    {
        if(_finishConditionIsOn == true)
        {
            if (curTarget == null)
            {
                Debug.Log("Action is finished");
                Debug.Log("Action Attack target is null");
                StopMoving(_unit);
                FinishAction();
                return;
            }
            if (curTarget.GetComponent<IEntity>().GetSelectionType() == IEntity.SelectionType.UnSelectable)
            {
                Debug.Log("Action is finished");
                Debug.Log("Action Attack target is Unselectable");
                StopMoving(_unit);
                FinishAction();
                return;
            }
        }
        

        // Move if outside range and return (no attacking)
        float distance = (curTarget.GetComponent<Rigidbody>().position - _unit.GetRigidbody().position).magnitude;
        if ( distance > (float)_attackRange)
        {
            // Since group flow field is reinitialize every frame also
            InitializeCurrentFlowField(curSelectGroup.GetComponent<SelectGroup>().groupFlowField);

            // Move with Rigidbody
            Vector3 flowFieldVector = GetFlowFieldVector(_unit.GetRigidbody().position) * GetFlowFieldWeight(_unit.GetRigidbody().position);
            Vector3 alignmentVector = GetAlignmentVector(_unit.GetRigidbody().position, 1f, 3f) * GetAlignmentWeight(_unit.GetRigidbody().position, 1f);
            Vector3 separationVector = GetSeparationVector(_unit.GetRigidbody().position, 1f, 3f) * GetSeparationWeight(_unit.GetRigidbody().position, 1f);

            Vector3 moveDir = flowFieldVector + alignmentVector + separationVector;
            // rb.velocity cause other unit's transform to change slightly. Reason unknown
            // rb.velocity = moveDir.normalized * movementSpeed;
            if (moveDir == Vector3.zero) { return; }

            _unit.GetRigidbody().MovePosition(_unit.GetRigidbody().position + moveDir.normalized * _movementSpeed * Time.deltaTime);
            _unit.GetRigidbody().MoveRotation(Quaternion.LookRotation(moveDir.normalized));
            return;
        }


        if(curTarget.GetComponent<IEntity>().GetEntityType() == IEntity.EntityType.Unit)
        {
            if(curTarget == null || curTarget.GetComponent<Unit>().HealthIsZero())
            {
                Debug.Log("Action is finished");
                StopMoving(_unit);
                FinishAction();
                return;
            }

            if(_canAttack)
            {
                curTarget.GetComponent<Unit>().MinusHealth(_damageAmount);
                _unit.SetCurrentAttackCooldown(0);
                //Debug.Log($"Attack {curTarget.name} for  {amount}");
            }
        }
        else if(curTarget.GetComponent<IEntity>().GetEntityType() == IEntity.EntityType.Structure)
        {
            if (curTarget == null || curTarget.GetComponent<Structure>().HealthIsZero())
            {
                Debug.Log("Action is finished");
                StopMoving(_unit);
                FinishAction();
                return;
            }

            if(_canAttack)
            {
                curTarget.GetComponent<Structure>().MinusHealth(_damageAmount);
                _unit.SetCurrentAttackCooldown(0);
                //Debug.Log($"Attack {curTarget.name} for  {amount}");
            }
        }
    }

    

    /// <summary>
    /// MOVE Action: move according to the flowfield and flocking (alignment + separation) with others in SelectGroup by changing the rigidbody attached
    /// Executed by ActionController
    /// </summary>
    /// <param name="_rb"></param>
    /// <param name="flowField"></param>
    /// <param name="_movementSpeed"></param>
    public void MoveInFlowField(Unit _unit, float _movementSpeed, bool _finishConditionIsOn)
    {
        if(_finishConditionIsOn == true)
        {
            float cellDistance = (curFlowField.GetCellFromWorldPos(_unit.GetRigidbody().position).gridPosition - curFlowField.destinationCell.gridPosition).magnitude;
            // Goal Check
            if (cellDistance < 1)
            {
                Debug.Log("Action is finished");
                StopMoving(_unit);
                FinishAction();
                return;
            }
        }
        

        // Move with Rigidbody
        Vector3 flowFieldVector = GetFlowFieldVector(_unit.GetRigidbody().position) * GetFlowFieldWeight(_unit.GetRigidbody().position);
        Vector3 alignmentVector = GetAlignmentVector(_unit.GetRigidbody().position, 1f, 3f) * GetAlignmentWeight(_unit.GetRigidbody().position, 1f);
        Vector3 separationVector = GetSeparationVector(_unit.GetRigidbody().position, 1f, 3f) * GetSeparationWeight(_unit.GetRigidbody().position, 1f);

        Vector3 moveDir = flowFieldVector + alignmentVector + separationVector;
        // rb.velocity cause other unit's transform to change slightly. Reason unknown
        // rb.velocity = moveDir.normalized * movementSpeed;
        if(moveDir == Vector3.zero) { return; }

        _unit.GetRigidbody().MovePosition(_unit.GetRigidbody().position + moveDir.normalized * _movementSpeed * Time.deltaTime);
        _unit.GetRigidbody().MoveRotation(Quaternion.LookRotation(moveDir.normalized));

    }

    private Vector3 GetFlowFieldVector(Vector3 curWorldPos)
    {
        // Validity Check
        if (curFlowField == null) { return Vector3.zero; }
        if (isFinished == true) { return Vector3.zero; }

        if (curFlowField.GetCellFromWorldPos(curWorldPos) == curFlowField.destinationCell) { return Vector3.zero; }

        // Calculate direction from flowfield
        int x = curFlowField.GetCellFromWorldPos(curWorldPos).bestDirection.Vector.x;
        int z = curFlowField.GetCellFromWorldPos(curWorldPos).bestDirection.Vector.y;
        Vector3 flowFieldDir = new Vector3(x, 0, z);
        return flowFieldDir.normalized;
    }

    private float GetFlowFieldWeight(Vector3 _curWorldPos)
    {
        float weight = 1;
        weight = Mathf.Clamp01((curFlowField.destinationCell.worldPosition - _curWorldPos).magnitude);

        return weight;
    }

    private Vector3 GetAlignmentVector(Vector3 _curWorldPos, float _minDistance, float _maxDistance)
    {
        // Validity Check
        if (curFlowField == null) { return Vector3.zero; }
        if (isFinished == true) { return Vector3.zero; }

        Vector3 alignmentDir = new Vector3(0, 0, 0);

        int excludeCount = 0;
        for(int index = 0; index < curSelectGroup.GetComponent<SelectGroup>().unitList.Count; index++)
        {
            if(curSelectGroup.GetComponent<SelectGroup>().actionList[index].isFinished == true) { excludeCount++; continue; }

            Vector3 curUnitPos = curSelectGroup.GetComponent<SelectGroup>().unitList[index].gameObject.GetComponent<Rigidbody>().position;
            Vector3 flowFieldVector = GetFlowFieldVector(curUnitPos);
            float distance = Vector3.Distance(_curWorldPos, curUnitPos);
            float weight = ((_maxDistance - Mathf.Clamp(distance, _minDistance, _maxDistance))) / (_maxDistance - _minDistance);
            alignmentDir += flowFieldVector * weight;
        }
        alignmentDir = alignmentDir / (curSelectGroup.GetComponent<SelectGroup>().unitList.Count - excludeCount);

        return alignmentDir.normalized;
    }

    private float GetAlignmentWeight(Vector3 _curWorldPos, float _minDistance)
    {
        float weight = 1;
        for (int index = 0; index < curSelectGroup.GetComponent<SelectGroup>().unitList.Count; index++)
        {
            if(curSelectGroup.GetComponent<SelectGroup>().unitList[index] == null) { continue; }
            Vector3 curUnitPos = curSelectGroup.GetComponent<SelectGroup>().unitList[index].gameObject.GetComponent<Rigidbody>().position;
            if ((curUnitPos - _curWorldPos).magnitude < _minDistance)
                weight ++;
        }

        return weight;
    }

    private Vector3 GetSeparationVector(Vector3 _curWorldPos, float _minDistance, float _maxDistance)
    {
        // Validity Check
        if (curFlowField == null) { return Vector3.zero; }
        if (isFinished == true) { return Vector3.zero; }

        Vector3 separationDir = new Vector3(0, 0, 0);

        int excludeCount = 0;
        for (int index = 0; index < curSelectGroup.GetComponent<SelectGroup>().unitList.Count; index++)
        {
            if (curSelectGroup.GetComponent<SelectGroup>().actionList[index].isFinished == true) { excludeCount++; continue; }

            Vector3 curUnitPos = curSelectGroup.GetComponent<SelectGroup>().unitList[index].gameObject.GetComponent<Rigidbody>().position;
            Vector3 separationVector = _curWorldPos - curUnitPos;
            float weight = ((_maxDistance - Mathf.Clamp(separationVector.magnitude, _minDistance, _maxDistance))) / (_maxDistance - _minDistance);
            separationVector =  weight * separationVector.normalized;
            separationDir += separationVector;
        }

        separationDir = separationDir / (curSelectGroup.GetComponent<SelectGroup>().unitList.Count - excludeCount);

        return separationDir.normalized;
    }

    private float GetSeparationWeight(Vector3 _curWorldPos, float _minDistance)
    {
        float weight = 1;
        for (int index = 0; index < curSelectGroup.GetComponent<SelectGroup>().unitList.Count; index++)
        {
            if(curSelectGroup.GetComponent<SelectGroup>().unitList[index] == null) { continue; }
            Vector3 curUnitPos = curSelectGroup.GetComponent<SelectGroup>().unitList[index].gameObject.GetComponent<Rigidbody>().position;
            if ((curUnitPos - _curWorldPos).magnitude < _minDistance)
                weight ++;
        }

        return weight;
    }

    public void FinishAction() { isFinished = true; }

    public void StopMoving(Unit _unit) 
    {
        if (_unit == null) { return; }
        _unit.GetRigidbody().velocity = Vector3.zero;
    }

    // Initialize properties

    public void InitializeMousePosition(Vector3 _position) { mousePosition = _position; }

    public void InitializeSelectGroupObject(ref GameObject _selectGroup) 
    { 
        if(_selectGroup == null) { return; }
        curSelectGroup = _selectGroup; 
    }

    public void InitializeSelfFlowField(Vector3 _destinationPos)
    {
        selfFlowField = new FlowField(GameObject.FindObjectOfType<GridController>().cellRadius, GameObject.FindObjectOfType<GridController>().gridSize);
        selfFlowField.CreateGrid();
        selfFlowField.CreateCostField();
        selfFlowField.CreateIntegrationField(selfFlowField.GetCellFromWorldPos(_destinationPos));
        selfFlowField.CreateFlowField();
    }

    public void InitializeCurrentFlowField(FlowField _flowField)
    {
        if(_flowField == null) { return; }
        curFlowField = _flowField;
    }

    public void InitializeCurrentTarget(GameObject _target)
    {
        if(_target == null) { return; }
        curTarget = _target;
    }

}

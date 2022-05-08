using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackAction : IAttackMove
{
    public Vector3 attackPosition { get; set; }
    public Vector3 movePosition { get; set; }
    public IAction[] subActions { get; set; }
    public bool isFinished { get; set; }
    public SelectGroup group { get; set; }
    public IEntity target { get; set; }

    public FlowField selfFlowField { get; set; }
    public FlowField curFlowField { get; set; }

    private bool isAttackMove;
    private float cellRadius;
    private Vector2Int gridSize;

    private IEntity self;
    private AttackController attackController;
    private MovementController movementController;
    private Vision vision;
    private Rigidbody rb;

    public AttackAction(IEntity _self, SelectGroup _group, AttackController _controller, Vector3 _attackPosition)
    {
        target = null;
        attackPosition = _attackPosition;
        movePosition = _attackPosition;
        selfFlowField = null;
        curFlowField = null;

        isAttackMove = true;
        cellRadius = GameObject.FindObjectOfType<GridController>().cellRadius;
        gridSize = GameObject.FindObjectOfType<GridController>().gridSize;

        subActions = new IAction[15];
        isFinished = false;
        group = _group;

        self = _self;
        attackController = _controller;
        if (_controller.GetComponent<MovementController>() != null)
            movementController = _controller.GetComponent<MovementController>();
        if (_controller.GetComponent<Vision>() != null)
            vision = attackController.GetComponent<Vision>();
        if (_controller.GetComponent<Rigidbody>() != null)
            rb = _controller.GetComponent<Rigidbody>();
    }

    public AttackAction(IEntity _self, SelectGroup _group, AttackController _controller, IEntity _target)
    {
        target = _target;
        attackPosition = _target.GetWorldPosition();
        movePosition = Vector3.zero;
        selfFlowField = null;
        curFlowField = null;

        isAttackMove = false;
        cellRadius = GameObject.FindObjectOfType<GridController>().cellRadius;
        gridSize = GameObject.FindObjectOfType<GridController>().gridSize;

        subActions = new IAction[15];
        isFinished = false;
        group = _group;

        self = _self;
        attackController = _controller;
        if (_controller.GetComponent<MovementController>() != null)
            movementController = _controller.GetComponent<MovementController>();
        if (_controller.GetComponent<Vision>() != null)
            vision = attackController.GetComponent<Vision>();
        if (_controller.GetComponent<Rigidbody>() != null)
            rb = _controller.GetComponent<Rigidbody>();
    }

    public void Execute()
    {
        if(isAttackMove)
        {
            AttackMove(rb, movementController.GetMovementSpeed(), attackController.GetAttackDamage(), attackController.GetAttackRange(), vision.GetVisionRange());
        }
        else
        {
            AttackTarget(self.GetWorldPosition(), attackController.GetAttackDamage(), attackController.GetAttackRange(), vision.GetVisionRange());
        }
    }

    public void AttackMove(Rigidbody _rb, float _movementSpeed, float _attackDamage, float _attackRange, float _visionRange)
    {
        float destinationDistance = (_rb.position - attackPosition).magnitude;
        IEntity closestEnemy = ReturnNearbyEnemy(_rb.position, _visionRange);
        if (destinationDistance < cellRadius * 1.5 && closestEnemy == null && isAttackMove)
        {
            Debug.Log("Attack move reached destination and no enemy nearby");
            Debug.Log("Action is finished");
            StopMoving(_rb);
            Stop();
            return;
        }
        else if (closestEnemy != null)
        {
            if (target != closestEnemy) { target = closestEnemy; }

            AttackTarget(_rb.position, _attackDamage, _attackRange, _visionRange);
            Debug.Log($"Trying to Attack {target.GetTransform().gameObject.name}");

        }
        else if (movementController != null)
        {
            InitializeMoveTowards(_rb.position, attackPosition);
            MoveTowards(_rb, _movementSpeed);
            Debug.Log($"Move Towards attack position: {attackPosition}");
        }
    }

    public void AttackTarget(Vector3 _curPosition, float _attackDamage, float _attackRange, float _visionRange)
    {
        if (target.GetSelf() == null)
        {
            if (!isAttackMove)
            {
                Debug.Log("Target is null");
                Debug.Log("Action is finished");
                Stop();
            }
        }
        else
        {
            if (target.GetTransform().GetComponent<Health>() == null)
            {
                if (!isAttackMove)
                {
                    Debug.Log("Target is null");
                    Debug.Log("Action is finished");
                    Stop();
                }
            }
            else
            {
                if (target.GetTransform().GetComponent<Health>().HealthIsZero())
                {
                    if (!isAttackMove)
                    {
                        Debug.Log("Target is null");
                        Debug.Log("Action is finished");
                        Stop();
                    }
                }
            }
        }
        

        if(target.GetSelf() == null) { return; }
        float distance = (target.GetWorldPosition() - _curPosition).magnitude - target.GetSelectedCircleRadius();
        if (distance > _attackRange) 
        {
            if(movementController != null)
            {
                InitializeMoveTowards(rb.position, target.GetWorldPosition());
                MoveTowards(rb, movementController.GetMovementSpeed());
                Debug.Log($"Move Towards attack position: {attackPosition}");
            }
            return;
        }

        if(attackController.CanAttack())
        {
            target.GetTransform().GetComponent<Health>().MinusHealth(_attackDamage);
            attackController.SetCurrentAttackCooldown(0);
            Debug.Log($"{self.GetTransform().gameObject.name} Attack {target.GetTransform().gameObject.name} for  {_attackDamage}");
        }
            
    }


    public IEntity ReturnNearbyEnemy(Vector3 _curPosition, float _visionRange)
    {
        Collider[] colliders = Physics.OverlapCapsule(_curPosition + Vector3.down * 50, _curPosition + Vector3.up * 50, _visionRange, LayerMask.GetMask(Tags.Selectable));
        List<IEntity> enemyList = new List<IEntity>();
        foreach (Collider collider in colliders)
        {
            if (collider.gameObject.GetComponent<IEntity>().GetPlayerIndex() != self.GetPlayerIndex())
            {
                enemyList.Add(collider.gameObject.GetComponent<IEntity>());
                //Debug.Log($"Enemy List: {collider.gameObject.name}");
            }
        }

        float closestDistance = float.MaxValue;
        IEntity closestEnemy = null;

        foreach (IEntity curEnemy in enemyList)
        {
            Vector3 vector = curEnemy.GetWorldPosition() - _curPosition;
            float distance = new Vector3(vector.x, 0, vector.z).magnitude;
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestEnemy = curEnemy;
                //Debug.Log($"Closest Enemy: {closestEnemy.GetTransform().gameObject.name}");
            }
        }

        return closestEnemy;
    }
    
    public void MoveTowards(Rigidbody _rb, float _speed)
    {
        // Check if the entity has reached the destination or has reached the temporary position to update self flowfield
        float destinationDistance = new Vector3(_rb.position.x - movePosition.x, 0, _rb.position.z - movePosition.z).magnitude;
        float tempDistance = new Vector3(_rb.position.x - curFlowField.destinationCell.worldPosition.x, 0, _rb.position.z - curFlowField.destinationCell.worldPosition.z).magnitude;

        //Debug.Log(destinationDistance);
        if (tempDistance < cellRadius * 1.5)
        {
            Vector2 dir = new Vector2(movePosition.x - curFlowField.destinationCell.worldPosition.x, movePosition.z - curFlowField.destinationCell.worldPosition.z).normalized;
            float distance = cellRadius * 2 * ((gridSize.x < gridSize.y) ? gridSize.y : gridSize.x);
            if (destinationDistance < distance)
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

            if (hit.collider == null)
            {
                InitializeSelfFlowField(_rb.position, nextPosition);
                AssignCurrentFlowField(selfFlowField);
                //Debug.Log("Updated current flowfield without obstacle");
            }
            else
            {
                InitializeSelfFlowField(_rb.position, nextPosition, hit.collider.gameObject.GetComponent<IObstacle>());
                AssignCurrentFlowField(selfFlowField);
                //Debug.Log("Updated current flowfield with obstacle");
            }
        }

        // Move with Rigidbody
        Vector3 flowFieldVector = GetFlowFieldDirection(_rb.position);

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


    public void InitializeMoveTowards(Vector3 _curPosition, Vector3 _moveMousePosition)
    {
        // Check for entity at the position to initialize the approriate flowfield
        Vector2 dir = new Vector2(movePosition.x - _curPosition.x, movePosition.z - _curPosition.z).normalized;
        float distance = cellRadius * 2 * ((gridSize.x < gridSize.y) ? gridSize.y : gridSize.x);

        // Check for entity at the position to initialize the approriate flowfield
        RaycastHit hit;
        Physics.BoxCast(_curPosition, Vector3.one, new Vector3(dir.x, 0, dir.y), out hit, Quaternion.identity, distance, LayerMask.GetMask(Tags.Obstacle), QueryTriggerInteraction.Ignore);

        if (hit.collider == null)
        {
            InitializeSelfFlowField(_curPosition, _moveMousePosition);
            //Debug.Log("Updated current flowfield without obstacle");
        }
        else
        {
            InitializeSelfFlowField(_curPosition, _moveMousePosition, hit.collider.gameObject.GetComponent<IObstacle>());
            //Debug.Log("Updated current flowfield with obstacle");
        }

        movePosition = _moveMousePosition;

        AssignCurrentFlowField(selfFlowField);

        //GridDebug.SetCurFlowField(curFlowField);
        //GridDebug.DrawFlowField();
    }

    public void AssignCurrentFlowField(FlowField _flowField)
    {
        curFlowField = _flowField;
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
        for (int index = 0; index < vector.magnitude / (cellRadius * 2); index++)
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


    public void Cancel()
    {
        isFinished = true;
    }

    public SelectGroup GetGroup()
    {
        return group;
    }

    public Vector3 GetMovePosition()
    {
        return movePosition;
    }

    public dynamic GetSelf()
    {
        return this;
    }

    public IAction[] GetSubActions()
    {
        return subActions;
    }


    public void SetMovePosition(Vector3 _vector)
    {
        movePosition = _vector;
    }

    public void SetSubActions(IAction[] _actions)
    {
        subActions = _actions;
    }

    public void SetTarget(IEntity _target)
    {
        target = _target;
    }

    public void Stop()
    {
        isFinished = true;
    }

    public void StopMoving(Rigidbody _rb)
    {
        _rb.velocity = Vector3.zero;
    }
}

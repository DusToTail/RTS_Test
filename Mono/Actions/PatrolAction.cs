using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolAction : IPatrol
{
    public Vector3 movePosition { get; set; }
    public IAction[] subActions { get; set; }
    public bool isFinished { get; set; }
    public SelectGroup group { get; set; }

    public FlowField curFlowField { get; set; }
    public FlowField selfFlowField { get; set; }

    public Vector3[] moveWaypoints { get; set; }
    public int curWaypointIndex { get; set; }

    private float cellRadius;
    private Vector2Int gridSize;

    private IEntity self;
    private MovementController controller;

    /// <summary>
    /// Constructor.
    /// 日本語：コンストラクタ。
    /// </summary>
    public PatrolAction(IEntity _self, SelectGroup _group, MovementController _controller, Vector3 _curPosition, Vector3[] _moveWaypoints)
    {
        //Debug.Log("Marine Action Created");
        moveWaypoints = _moveWaypoints;
        curWaypointIndex = 0;
        movePosition = _moveWaypoints[curWaypointIndex];
        selfFlowField = null;
        curFlowField = null;

        cellRadius = GameObject.FindObjectOfType<GridController>().cellRadius;
        gridSize = GameObject.FindObjectOfType<GridController>().gridSize;

        subActions = new IAction[15];
        isFinished = false;
        group = _group;

        self = _self;
        controller = _controller;

        InitializePatrol(_curPosition, _moveWaypoints);
    }

    public void Execute()
    {
        Patrol(self.GetTransform().GetComponent<Rigidbody>(), controller.GetMovementSpeed());
    }

    public void Patrol(Rigidbody _rb, float _speed)
    {
        // Check if the entity has reached the destination to update current waypoint
        //Debug.Log(curWaypointIndex);
        float destinationDistance = new Vector3(_rb.position.x - moveWaypoints[curWaypointIndex].x, 0, _rb.position.z - moveWaypoints[curWaypointIndex].z).magnitude;
        //Debug.Log(destinationDistance);
        if (destinationDistance < cellRadius * 1.5)
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

        movePosition = moveWaypoints[0];

        AssignCurrentFlowField(selfFlowField);

        curWaypointIndex = 0;
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
            Debug.Log("Updated current flowfield without obstacle");
        }
        else
        {
            InitializeSelfFlowField(_curPosition, _moveMousePosition, hit.collider.gameObject.GetComponent<IObstacle>());
            Debug.Log("Updated current flowfield with obstacle");
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

    public void Stop()
    {
        isFinished = true;
    }

    public void StopMoving(Rigidbody _rb)
    {
        _rb.velocity = Vector3.zero;
    }


    public void Cancel()
    {
        isFinished = true;
    }

    public void SetMovePosition(Vector3 _vector)
    {
        movePosition = _vector;
    }

    public void SetSubActions(IAction[] _actions)
    {
        subActions = _actions;
    }

    public void SetWaypoints(Vector3[] _vectors)
    {
        moveWaypoints = _vectors;
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

}

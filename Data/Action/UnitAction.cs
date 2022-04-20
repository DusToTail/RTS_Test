using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitAction
{
    /// <summary>
    /// DESCRIPTION: An Action is used by ActionController (component of a Unit gameObject) to determine what to do.
    /// An action is unique for each unit and for each declaration.
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

    public void AttackMove(ref Rigidbody rb, float movementSpeed, float visionRange, float damageAmount, float attackRange, bool canAttack)
    {
        float cellDistance = (curFlowField.GetCellFromWorldPos(rb.position).gridPosition - curFlowField.destinationCell.gridPosition).magnitude;

        Collider[] colliders = Physics.OverlapCapsule(rb.position + Vector3.down * 50, rb.position + Vector3.up * 50, visionRange, LayerMask.GetMask(Tags.Selectable));
        List<GameObject> enemyList = new List<GameObject>();

        foreach (Collider collider in colliders)
        {
            if (collider.gameObject.GetComponent<EntityInterface>().GetEntityType() == EntityInterface.EntityTypes.SelectableUnit)
            {
                if (collider.gameObject.GetComponent<EntityInterface>().GetRelationshipType() == EntityInterface.RelationshipTypes.Enemy)
                {
                    enemyList.Add(collider.gameObject);
                }
            }
            else if (collider.gameObject.GetComponent<EntityInterface>().GetEntityType() == EntityInterface.EntityTypes.SelectableStructure)
            {
                if (collider.gameObject.GetComponent<EntityInterface>().GetRelationshipType() == EntityInterface.RelationshipTypes.Enemy)
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

        // Goal Check
        if (cellDistance < 1 && closestEnemy == null)
        {
            Debug.Log("Attack move reached destination and no enemy nearby");
            Debug.Log("Action is finished");
            StopMoving(ref rb);
            FinishAction();
            return;
        }
        else if(closestEnemy != null)
        {
            InitializeSelfFlowField(closestEnemy.GetComponent<Rigidbody>().position);
            InitializeCurrentFlowField(selfFlowField);
            curTarget = closestEnemy;
            AttackTarget(ref rb, movementSpeed, damageAmount, attackRange, canAttack, false);
        }
        else
        {
            InitializeSelfFlowField(mousePosition);
            InitializeCurrentFlowField(selfFlowField);
            MoveInFlowField(ref rb, movementSpeed, false);
        }

    }

    public void Patrol(ref Rigidbody rb, float movementSpeed)
    {
        // Goal Check
        float cellDistance = (curFlowField.GetCellFromWorldPos(rb.position).gridPosition - curFlowField.destinationCell.gridPosition).magnitude;
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
        Vector3 flowFieldVector = GetFlowFieldVector(rb.position) * GetFlowFieldWeight(rb.position);
        Vector3 alignmentVector = GetAlignmentVector(rb.position, 1f, 3f) * GetAlignmentWeight(rb.position, 1f);
        Vector3 separationVector = GetSeparationVector(rb.position, 1f, 3f) * GetSeparationWeight(rb.position, 1f);


        Vector3 moveDir = flowFieldVector + alignmentVector + separationVector;
        // rb.velocity cause other unit's transform to change slightly. Reason unknown
        // rb.velocity = moveDir.normalized * movementSpeed;
        if (moveDir == Vector3.zero) { return; }

        rb.MovePosition(rb.position + moveDir.normalized * movementSpeed * Time.deltaTime);
        rb.MoveRotation(Quaternion.LookRotation(moveDir.normalized));
    }


    public void AttackTarget(ref Rigidbody rb, float movementSpeed, float amount, float range, bool canAttack, bool finishConditionIsOn)
    {
        if(finishConditionIsOn == true)
        {
            if (curTarget == null)
            {
                Debug.Log("Action is finished");
                Debug.Log("Action Attack target is null");
                StopMoving(ref rb);
                FinishAction();
                return;
            }
            if (curTarget.GetComponent<EntityInterface>().GetEntityType() == EntityInterface.EntityTypes.UnselectableUnit || curTarget.GetComponent<EntityInterface>().GetEntityType() == EntityInterface.EntityTypes.UnselectableStructure)
            {
                Debug.Log("Action is finished");
                Debug.Log("Action Attack target is Unselectable");
                StopMoving(ref rb);
                FinishAction();
                return;
            }
        }
        

        // Move if outside range and return (no attacking)
        float distance = (curTarget.GetComponent<Rigidbody>().position - rb.position).magnitude;
        if ( distance > (float)range)
        {
            // Since group flow field is reinitialize every frame also
            InitializeCurrentFlowField(curSelectGroup.GetComponent<SelectGroup>().groupFlowField);

            // Move with Rigidbody
            Vector3 flowFieldVector = GetFlowFieldVector(rb.position) * GetFlowFieldWeight(rb.position);
            Vector3 alignmentVector = GetAlignmentVector(rb.position, 1f, 3f) * GetAlignmentWeight(rb.position, 1f);
            Vector3 separationVector = GetSeparationVector(rb.position, 1f, 3f) * GetSeparationWeight(rb.position, 1f);

            Vector3 moveDir = flowFieldVector + alignmentVector + separationVector;
            // rb.velocity cause other unit's transform to change slightly. Reason unknown
            // rb.velocity = moveDir.normalized * movementSpeed;
            if (moveDir == Vector3.zero) { return; }

            rb.MovePosition(rb.position + moveDir.normalized * movementSpeed * Time.deltaTime);
            rb.MoveRotation(Quaternion.LookRotation(moveDir.normalized));
            return;
        }


        if(curTarget.GetComponent<EntityInterface>().GetEntityType() == EntityInterface.EntityTypes.SelectableUnit)
        {
            if(curTarget.GetComponent<Unit>().GetCurHealth() == 0 || curTarget == null)
            {
                Debug.Log("Action is finished");
                StopMoving(ref rb);
                FinishAction();
                return;
            }

            if(canAttack)
            {
                curTarget.GetComponent<Unit>().MinusHealth(amount);
                rb.gameObject.GetComponent<Unit>().SetCurrentAttackCooldown(0);
                Debug.Log($"Attack {curTarget.name} for  {amount}");
            }
        }
        else if(curTarget.GetComponent<EntityInterface>().GetEntityType() == EntityInterface.EntityTypes.SelectableStructure)
        {
            if (curTarget.GetComponent<Structure>().GetCurHealth() == 0 || curTarget == null)
            {
                Debug.Log("Action is finished");
                StopMoving(ref rb);
                FinishAction();
                return;
            }

            if(canAttack)
            {
                curTarget.GetComponent<Structure>().HealthMinus(amount);
                rb.gameObject.GetComponent<Unit>().SetCurrentAttackCooldown(0);
                Debug.Log($"Attack {curTarget.name} for  {amount}");
            }
        }
    }

    

    /// <summary>
    /// MOVE Action: move according to the flowfield and flocking (alignment + separation) with others in SelectGroup by changing the rigidbody attached
    /// Executed by ActionController
    /// </summary>
    /// <param name="rb"></param>
    /// <param name="flowField"></param>
    /// <param name="movementSpeed"></param>
    public void MoveInFlowField(ref Rigidbody rb, float movementSpeed, bool finishConditionIsOn)
    {
        if(finishConditionIsOn == true)
        {
            float cellDistance = (curFlowField.GetCellFromWorldPos(rb.position).gridPosition - curFlowField.destinationCell.gridPosition).magnitude;
            // Goal Check
            if (cellDistance < 1)
            {
                Debug.Log("Action is finished");
                StopMoving(ref rb);
                FinishAction();
                return;
            }
        }
        

        // Move with Rigidbody
        Vector3 flowFieldVector = GetFlowFieldVector(rb.position) * GetFlowFieldWeight(rb.position);
        Vector3 alignmentVector = GetAlignmentVector(rb.position, 1f, 3f) * GetAlignmentWeight(rb.position, 1f);
        Vector3 separationVector = GetSeparationVector(rb.position, 1f, 3f) * GetSeparationWeight(rb.position, 1f);

        Vector3 moveDir = flowFieldVector + alignmentVector + separationVector;
        // rb.velocity cause other unit's transform to change slightly. Reason unknown
        // rb.velocity = moveDir.normalized * movementSpeed;
        if(moveDir == Vector3.zero) { return; }

        rb.MovePosition(rb.position + moveDir.normalized * movementSpeed * Time.deltaTime);
        rb.MoveRotation(Quaternion.LookRotation(moveDir.normalized));

    }

    private Vector3 GetFlowFieldVector(Vector3 curWorldPos)
    {
        // Validity Check
        if (curFlowField == null) { return Vector3.zero; }
        if (isFinished == true) { return Vector3.zero; }

        // Goal Check
        if (curFlowField.GetCellFromWorldPos(curWorldPos) ==
            curFlowField.destinationCell)
        {
            return Vector3.zero;
        }

        // Calculate direction from flowfield
        int x = curFlowField.GetCellFromWorldPos(curWorldPos).bestDirection.Vector.x;
        int z = curFlowField.GetCellFromWorldPos(curWorldPos).bestDirection.Vector.y;
        Vector3 flowFieldDir = new Vector3(x, 0, z);
        return flowFieldDir.normalized;
    }

    private float GetFlowFieldWeight(Vector3 curWorldPos)
    {
        float weight = 1;
        weight = Mathf.Clamp01((curFlowField.destinationCell.worldPosition - curWorldPos).magnitude);

        return weight;
    }

    private Vector3 GetAlignmentVector(Vector3 curWorldPos, float minDistance, float maxDistance)
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
            float distance = Vector3.Distance(curWorldPos, curUnitPos);
            float weight = ((maxDistance - Mathf.Clamp(distance, minDistance, maxDistance))) / (maxDistance - minDistance);
            alignmentDir += flowFieldVector * weight;
        }
        alignmentDir = alignmentDir / (curSelectGroup.GetComponent<SelectGroup>().unitList.Count - excludeCount);

        return alignmentDir.normalized;
    }

    private float GetAlignmentWeight(Vector3 curWorldPos, float minDistance)
    {
        float weight = 1;
        for (int index = 0; index < curSelectGroup.GetComponent<SelectGroup>().unitList.Count; index++)
        {
            Vector3 curUnitPos = curSelectGroup.GetComponent<SelectGroup>().unitList[index].gameObject.GetComponent<Rigidbody>().position;
            if ((curUnitPos - curWorldPos).magnitude < minDistance)
                weight ++;
        }

        return weight;
    }

    private Vector3 GetSeparationVector(Vector3 curWorldPos, float minDistance, float maxDistance)
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
            Vector3 separationVector = curWorldPos - curUnitPos;
            float weight = ((maxDistance - Mathf.Clamp(separationVector.magnitude, minDistance, maxDistance))) / (maxDistance - minDistance);
            separationVector =  weight * separationVector.normalized;
            separationDir += separationVector;
        }

        separationDir = separationDir / (curSelectGroup.GetComponent<SelectGroup>().unitList.Count - excludeCount);

        return separationDir.normalized;
    }

    private float GetSeparationWeight(Vector3 curWorldPos, float minDistance)
    {
        float weight = 1;
        for (int index = 0; index < curSelectGroup.GetComponent<SelectGroup>().unitList.Count; index++)
        {
            Vector3 curUnitPos = curSelectGroup.GetComponent<SelectGroup>().unitList[index].gameObject.GetComponent<Rigidbody>().position;
            if ((curUnitPos - curWorldPos).magnitude < minDistance)
                weight ++;
        }

        return weight;
    }

    public void FinishAction() { isFinished = true; }

    public void StopMoving(ref Rigidbody rb) 
    {
        if (rb == null) { return; }
        rb.velocity = Vector3.zero; 
    }

    public void InitializeMousePosition(Vector3 position) { mousePosition = position; }

    public void InitializeSelectGroupObject(ref GameObject _selectGroup) 
    { 
        if(_selectGroup == null) { return; }
        curSelectGroup = _selectGroup; 
    }

    public void InitializeSelfFlowField(Vector3 destinationPos)
    {
        selfFlowField = new FlowField(GameObject.FindObjectOfType<GridController>().cellRadius, GameObject.FindObjectOfType<GridController>().gridSize);
        selfFlowField.CreateGrid();
        selfFlowField.CreateCostField();
        selfFlowField.CreateIntegrationField(selfFlowField.GetCellFromWorldPos(destinationPos));
        selfFlowField.CreateFlowField();
    }

    public void InitializeCurrentFlowField(FlowField flowField)
    {
        if(flowField == null) { return; }
        curFlowField = flowField;
    }

    public void InitializeCurrentTarget(GameObject target)
    {
        if(target == null) { return; }
        curTarget = target;
    }

}

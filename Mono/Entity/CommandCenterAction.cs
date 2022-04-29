using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// English: A class that has different types of actions that a Command Center can take and those actions implementations.
/// 日本語：Command Centerの行動の種類とそれらの実装を持つクラス。
/// </summary>
public class CommandCenterAction : IAction, IProduce, IUpgrade
{
    public enum Type : int
    {
        Stop = 5,
        Cancel = 6,
        None = 7,

        Produce = 100,
        Upgrade = 101,
    }

    public Type type;
    public List<IAction> subActions { get; set; }
    public bool isFinished { get; set; }
    public SelectGroup group { get; set; }


    public GameObject prefabUnit { get; set; }
    public Upgrade prefabUpgrade { get; set; }
    public Vector3 spawnPosition { get; set; }
    public Vector3 rallyPosition { get; set; }
    public List<GameObject> productionList { get; set; }


    public float maxProgressTime { get; set; }
    public float curProgressTime { get; set; }

    /// <summary>
    /// Default Constructor. When used, need separate initialization for each type of Action.
    /// 日本語：デフォルトコンストラクタ. 使用するとき、行動の種類によって個別の初期化が必要。
    /// </summary>
    public CommandCenterAction()
    {
        type = Type.None;
        subActions = null;
        group = null;
        isFinished = false;

        prefabUnit = null;
        spawnPosition = Vector3.zero;
        rallyPosition = Vector3.zero;
        productionList = new List<GameObject>();
        maxProgressTime = 0;
        curProgressTime = 0;
    }

    
    public void SpawnUnit()
    {
        if (curProgressTime < maxProgressTime) { AddProgress(Time.deltaTime); };

        GameObject newUnit = GameObject.Instantiate(prefabUnit, spawnPosition, Quaternion.identity);
        var newAction = newUnit.GetComponent<IUnit>().ReturnNewAction();
        newAction.InitializeType(0);
        Vector2 dir = new Vector2(rallyPosition.x - spawnPosition.x, rallyPosition.z - spawnPosition.z);
        
        newAction.InitializeMoveTowards(spawnPosition, rallyPosition);
        newUnit.GetComponent<IUnit>().GetActionController().EnqueueAction(newAction, true);

        // Debug.Log($"Spawned {newUnit.name}");
        Stop();
    }

    
    public void Upgrade()
    {
        if (curProgressTime < maxProgressTime) { AddProgress(Time.deltaTime); };

        // Debug.Log($"Upgraded {prefabUpgrade.name}");
        Stop();
    }

    
    public void AddProgress(float _time)
    {
        curProgressTime += _time;
    }
    
    public bool IsRunning()
    {
        return curProgressTime < maxProgressTime;
    }

    public void Cancel()
    {
        Stop();
    }

    public void Stop() { isFinished = true; }

    /// <summary>
    /// ※　NOT USED FOR NOW　※
    /// </summary>
    /// <param name="_rb"></param>
    /// <param name="_movementSpeed"></param>
    /// <param name="_attackDamage"></param>
    /// <param name="_attackRange"></param>
    /// <param name="_visionRange"></param>
    /// <param name="_canAttack"></param>
    public void AttackMove(Rigidbody _rb, float _movementSpeed, float _attackDamage, float _attackRange, float _visionRange, bool _canAttack) { Stop(); return; }
    
    /// <summary>
    /// ※　NOT USED FOR NOW　※
    /// </summary>
    /// <param name="_rb"></param>
    /// <param name="_movementSpeed"></param>
    /// <param name="_attackDamage"></param>
    /// <param name="_attackRange"></param>
    /// <param name="_visionRange"></param>
    /// <param name="_canAttack"></param>
    public void AttackTarget(Rigidbody _rb, float _movementSpeed, float _attackDamage, float _attackRange, float _visionRange, bool _canAttack) { Stop(); return; }
   
    /// <summary>
    /// ※　NOT USED　FOR NOW　※
    /// </summary>
    /// <param name="_rb"></param>
    /// <param name="_visionRange"></param>
    /// <returns></returns>
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
    
    /// <summary>
    /// ※　NOT USED　FOR NOW　※
    /// </summary>
    /// <param name="_rb"></param>
    /// <param name="_speed"></param>
    public void Patrol(Rigidbody _rb, float _speed) { Stop(); return; }

    /// <summary>
    /// ※　NOT USED　FOR NOW　※
    /// </summary>
    /// <param name="_rb"></param>
    /// <param name="_speed"></param>
    public void MoveTowards(Rigidbody _rb, float _speed)
    {
        Stop();
        return ;
    }

    /// <summary>
    /// English: Initialization for Spawn Unit Action (Produce)
    /// 日本語：スポーン行動（生産）の初期化
    /// </summary>
    /// <param name="_prefab"></param>
    /// <param name="_spawnPosition"></param>
    /// <param name="_rallyPosition"></param>
    public void InitializeSpawnUnit(GameObject _prefab, Vector3 _spawnPosition, Vector3 _rallyPosition)
    {
        AssignUnit(_prefab);
        AssignSpawnPosition(_spawnPosition);
        AssignRallyPosition(_rallyPosition);
        AssignMaxProgressTime(GetUnitBuildTime());
        InitializeType(100);
        curProgressTime = 0;
    }
    
    /// <summary>
    /// English: Initialization for Upgrade Action.
    /// 日本語：アップグレード行動の初期化。
    /// </summary>
    /// <param name="_prefab"></param>
    public void InitializeUpgrade(GameObject _prefab)
    {
        AssignUnit(_prefab);
        AssignMaxProgressTime(GetUpgradeTime());
        InitializeType(101);
        curProgressTime = 0;
    }


    /// <summary>
    /// English: Initialize Type of Action
    /// 日本語：行動の種類を初期化
    /// </summary>
    /// <param name="_type"></param>
    public void InitializeType(int _type)
    {
        type = (Type)_type;
    }
    
    /// <summary>
    /// English: Initialization of the group that the entity is in.
    /// 日本語：所属しているグループの初期化
    /// </summary>
    /// <param name="_group"></param>
    public void InitializeGroup(SelectGroup _group)
    {
        if(_group == null) { return; }
        group = _group;
    }

    // NOT USED
    public void InitializeMoveTowards(Vector3 _curPosition, Vector3 _moveMousePosition) { return; }
    public void InitializePatrol(Vector3 _curPosition, Vector3[] _moveWaypoints) { return; }
    public void InitializeAttackTarget(IEntity _target) { return;}
    public void InitializeAttackMove(Vector3 _curPosition, Vector3 _attackMovePosition) { return; }
    public void InitializeSelfFlowField(Vector3 _curPosition, Vector3 _destinationPosition) { return; }

    
    public void AssignUnit(GameObject _prefab)
    {
        if(_prefab == null) { return; }
        prefabUnit = _prefab;
    }
    
    public void AssignUpgrade(Upgrade _prefab)
    {
        if(_prefab == null) { return; }
        prefabUpgrade = _prefab;
    }
    
    public void AssignSpawnPosition(Vector3 _spawnPosition)
    {
        spawnPosition = _spawnPosition;
    }
    
    public void AssignRallyPosition(Vector3 _rallyPosition)
    { 
        rallyPosition = _rallyPosition;
    }

    /// <summary>
    /// ※　NOT USED　※
    /// </summary>
    /// <param name="_vectors"></param>
    public void AssignWaypointPosition(Vector3[] _vectors) { return; }
    
    /// <summary>
    /// English: Assign max progress time of the action.
    /// 日本語：行動を完了するのに要する時間変数を割り当てる。
    /// </summary>
    /// <param name="_time"></param>
    public void AssignMaxProgressTime(float _time) { maxProgressTime = _time; }
    
    /// <summary>
    /// ※　NOT USED　※
    /// </summary>
    /// <param name="_actions"></param>
    public void AssignSubActions(List<IAction> _actions)
    {
        if (_actions == null) { return; }
        subActions = _actions;
    }
    
    public float GetUnitBuildTime()
    {
        return prefabUnit.GetComponent<IUnit>().GetSetBuildingTime();
    }
    public float GetUpgradeTime()
    {
        return 0;
    }
    public List<GameObject> GetProductionList()
    {
        return productionList;
    }
    public List<IAction> GetSubActions() { return subActions; }
    public SelectGroup GetGroup()
    {
        return group;
    }
    public virtual dynamic GetSelf()
    {
        return this;
    }

}

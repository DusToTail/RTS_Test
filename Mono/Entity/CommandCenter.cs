using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// English: A class for a Command Center, implementing IStructure interface. Contains the basic information and stats of the structure.
/// Also manages health, and information for production
/// 日本語：Command Center のクラス、IUniｔインターフェースを実装する。ビルディングの基本情報や数値をもつ。
/// その他、体力、生産の情報などを管理する
/// </summary>
public class CommandCenter : MonoBehaviour, IStructure
{
    [Header("Production / Upgrade")]
    public GameObject[] unitPrefabs;
    public GameObject[] upgradePrefabs;
    public Transform spawnPosition;

    [Header("Basic Info")]
    [SerializeField]
    protected IEntity.SelectionType selectionType;
    [SerializeField]
    protected IEntity.RelationshipType relationshipType;
    [SerializeField]
    protected Sprite portrait;

    [Header("Stats Info")]
    [SerializeField]
    protected float setHealth;
    [SerializeField]
    protected float setMovementSpeed;
    [SerializeField]
    protected float setVisionRange;
    [SerializeField]
    protected float setBuildingTime;

    [Header("Others")]
    [SerializeField]
    protected float selectedCircleRadius;
    [SerializeField]
    protected GameObject miniMapSelf;

    protected float curHealth;
    protected bool isOperational;

    protected GameObject selectedCircle;

    //　Components
    protected Collider col;
    protected Rigidbody rb;

    public virtual void Awake()
    {
        // Initialization
        col = gameObject.GetComponent<Collider>();
        rb = gameObject.GetComponent<Rigidbody>();

        setVisionRange *= GameObject.FindObjectOfType<GridController>().cellRadius * 2;

        curHealth = setHealth;

        selectedCircle = transform.GetChild(0).gameObject;
        Vector3[] vertices = new Vector3[4];
        vertices[0] = new Vector3(-selectedCircleRadius, 0, -selectedCircleRadius);
        vertices[1] = new Vector3(-selectedCircleRadius, 0, selectedCircleRadius);
        vertices[2] = new Vector3(selectedCircleRadius, 0, selectedCircleRadius);
        vertices[3] = new Vector3(selectedCircleRadius, 0, -selectedCircleRadius);

        selectedCircle.GetComponent<LineRenderer>().SetPositions(vertices);
        selectedCircleRadius = col.bounds.extents.magnitude;

        miniMapSelf.SetActive(true);

        Debug.Log($"Structure {gameObject.name} Constructed");
    }

    public virtual void Start()
    {

    }

    public virtual void Update()
    {
        if (HealthIsZero()) { Destroy(this.gameObject); }
    }

    


    // IMPLEMENTATION for IStructure
    public IEntity.SelectionType GetSelectionType() { return selectionType; }
    public IEntity.RelationshipType GetRelationshipType() { return relationshipType; }
    public Sprite GetPortrait() { return portrait; }

    /// <summary>
    /// English: Minus current health for the specified amount. Clamp it at 0
    /// 日本語：現在の体力を指定した量マイナスする。０以下になれば０にする。
    /// </summary>
    /// <param name="_amount"></param>
    public void MinusHealth(float _amount)
    {
        if (_amount < 0) { _amount = 0; }
        curHealth -= _amount;
        if (curHealth < 0) { curHealth = 0; }
    }

    /// <summary>
    /// English: Plus current health for the specified amount. Clamp it at max health
    /// 日本語：現在の体力を指定した量プラスする。体力の最大値以上になれば最大値にする。
    /// </summary>
    /// <param name="_amount"></param>
    public void PlusHealth(float _amount)
    {
        if (_amount < 0) { _amount = 0; }
        curHealth += _amount;
        if (_amount > setHealth) { _amount = setHealth; }
    }

    public bool HealthIsZero() { return curHealth <= 0; }

    public bool CanAttack() { return false; }

    public void RenderSelectedCircle(bool isOn)
    {
        if (selectedCircle == null) { return; }
        if (isOn == true)
            selectedCircle.SetActive(true);
        else
            selectedCircle.SetActive(false);
    }

    public void SetCurrentHealth(float _setAmount)
    {
        curHealth = _setAmount;
        if (curHealth < 0) { curHealth = 0; }
        if (curHealth > setHealth) { curHealth = setHealth; }
    }
    public void SetCurrentAttackCooldown(float _time) { return; }

    public float GetSetHealth() { return setHealth; }
    public float GetSetMovementSpeed() { return 0; }
    public float GetSetVisionRange() { return setVisionRange; }
    public float GetSetAttackDamage() { return 0; }
    public float GetSetAttackRange() { return 0; }
    public float GetSetAttackCooldown() { return 0; }
    public float GetSetBuildingTime() { return setBuildingTime; }

    public float GetCurrentHealth() { return curHealth; }
    public float GetCurrentAttackCooldown() { return 0; }

    public GameObject GetSelectedCircle() { return selectedCircle; }

    public Collider GetCollider() { return col; }

    public IEntity.EntityType GetEntityType() { return IEntity.EntityType.Structure; }
    public Transform GetTransform() { if (transform == null) return null; return transform; }
    public Vector3 GetWorldPosition()
    {
        if (rb == null) { return Vector3.zero; }
        return rb.position;
    }
    public Rigidbody GetRigidbody() { if (GetComponent<Rigidbody>() != null) return GetComponent<Rigidbody>(); return null; }

    public float GetSelectedCircleRadius() { return selectedCircleRadius; }

    public virtual dynamic GetSelf() { return this; }
    public virtual dynamic GetActionController() 
    { 
        if(GetComponent<StructureActionController>() == null)
            return null;
        return GetComponent<StructureActionController>(); 
    }
    

    public virtual dynamic ReturnSelfType() { return typeof(CommandCenter); }
    public virtual dynamic ReturnNewAction() { return new CommandCenterAction(); }
    public virtual dynamic ReturnActionType() { return typeof(CommandCenterAction); }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * 2);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, setVisionRange);

    }
}

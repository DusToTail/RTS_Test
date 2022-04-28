using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandCenter : MonoBehaviour, IStructure
{
    public GameObject[] unitPrefabs;
    public GameObject[] upgradePrefabs;
    public Transform spawnPosition;

    // DESCRIPTION:
    [SerializeField]
    protected IEntity.SelectionType selectionType;
    [SerializeField]
    protected IEntity.RelationshipType relationshipType;
    [SerializeField]
    protected Sprite portrait;

    [SerializeField]
    protected float setHealth;
    [SerializeField]
    protected float setMovementSpeed;
    [SerializeField]
    protected float setVisionRange;
    [SerializeField]
    protected float setBuildingTime;
    [SerializeField]
    protected float selectedCircleRadius;
    [SerializeField]
    protected GameObject miniMapSelf;

    protected float curHealth;
    protected bool isOperational;

    protected GameObject selectedCircle;

    //Components
    protected Collider col;
    protected Rigidbody rb;

    //Components

    public virtual void Awake()
    {
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

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * 2);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, setVisionRange);

    }


    // IMPLEMENTATION for IStructure
    public IEntity.SelectionType GetSelectionType() { return selectionType; }
    public IEntity.RelationshipType GetRelationshipType() { return relationshipType; }
    public Sprite GetPortrait() { return portrait; }


    public void MinusHealth(float _amount)
    {
        if (_amount < 0) { _amount = 0; }
        curHealth -= _amount;
        if (curHealth < 0) { curHealth = 0; }
    }

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

}

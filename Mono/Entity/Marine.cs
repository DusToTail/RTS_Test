using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class Marine : MonoBehaviour, IUnit
{
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
    protected float setAttackDamage;
    [SerializeField]
    protected float setAttackRange;
    [SerializeField]
    protected float setAttackCooldown;
    [SerializeField]
    protected float setVisionRange;
    [SerializeField]
    protected float setBuildingTime;
    [SerializeField]
    protected float selectedCircleRadius;
    [SerializeField]
    protected GameObject miniMapSelf;

    protected float curHealth;
    protected float curAttackCooldown;

    protected GameObject selectedCircle;

    //Components
    protected Rigidbody rb;
    protected Collider col;

    public virtual void Awake()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        col = gameObject.GetComponent<Collider>();

        setAttackRange *= GameObject.FindObjectOfType<GridController>().cellRadius * 2;
        setVisionRange *= GameObject.FindObjectOfType<GridController>().cellRadius * 2;

        curHealth = setHealth;
        curAttackCooldown = setAttackCooldown;

        selectedCircle = transform.GetChild(0).gameObject;
        Vector3[] vertices = new Vector3[4];
        vertices[0] = new Vector3(-selectedCircleRadius, 0, -selectedCircleRadius);
        vertices[1] = new Vector3(-selectedCircleRadius, 0, selectedCircleRadius);
        vertices[2] = new Vector3(selectedCircleRadius, 0, selectedCircleRadius);
        vertices[3] = new Vector3(selectedCircleRadius, 0, -selectedCircleRadius);

        selectedCircle.GetComponent<LineRenderer>().SetPositions(vertices);
        selectedCircleRadius = col.bounds.extents.magnitude;

        rb.velocity = Vector3.zero;

        miniMapSelf.SetActive(true);

        Debug.Log($"Unit {gameObject.name} Constructed");
    }

    public virtual void Start()
    {

    }

    public virtual void Update()
    {
        AutoSeparationFromNearbyUnit(0.5f, 3f);
        if (CanAttack() == false)
            SetCurrentAttackCooldown(curAttackCooldown + Time.deltaTime);

        if (HealthIsZero()) { Destroy(this.gameObject); }

    }

    protected void AutoSeparationFromNearbyUnit(float _pushForce, float _radius)
    {
        Collider[] colliders = Physics.OverlapSphere(rb.position, _radius, LayerMask.GetMask(Tags.Selectable), QueryTriggerInteraction.Ignore);
        if (colliders.Length == 0) { return; }
        Vector3 moveVector = Vector3.zero;
        foreach (Collider collider in colliders)
        {
            if (collider.gameObject == this.gameObject) { continue; }

            Vector3 unitPos = collider.gameObject.GetComponent<Rigidbody>().position;
            Vector3 separateVector = rb.position - unitPos;
            if (separateVector.magnitude < _radius)
            {
                moveVector += separateVector.normalized * (1 - Mathf.Clamp01(separateVector.magnitude / _radius));
                //Debug.Log(gameObject.name + " is avoiding with " + force);
            }
        }
        rb.AddForce(moveVector.normalized * _pushForce);
    }

    public virtual void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * 2);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, setVisionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, setAttackRange);

    }


    // IMPLEMENTATION for IUnit
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

    public void SetCurrentAttackCooldown(float _setTime)
    {
        curAttackCooldown = _setTime;
        if (curAttackCooldown < 0) { curAttackCooldown = 0; }
    }

    public bool CanAttack() { return curAttackCooldown > setAttackCooldown; }

    public float GetSetHealth() { return setHealth; }
    public float GetSetMovementSpeed() { return setMovementSpeed; }
    public float GetSetVisionRange() { return setVisionRange; }
    public float GetSetAttackDamage() { return setAttackDamage; }
    public float GetSetAttackRange() { return setAttackRange; }
    public float GetSetAttackCooldown() { return setAttackCooldown; }
    public float GetSetBuildingTime() { return setBuildingTime; }

    public float GetCurrentHealth() { return curHealth; }
    public float GetCurrentAttackCooldown() { return curAttackCooldown; }

    public GameObject GetSelectedCircle() { return selectedCircle; }

    public Rigidbody GetRigidbody() { return rb; }
    public Collider GetCollider() { return col; }

    public IEntity.EntityType GetEntityType() { return IEntity.EntityType.Unit; }
    public Transform GetTransform() { if (transform == null) return null; return transform; }
    public Vector3 GetWorldPosition()
    {
        if (rb == null) { return Vector3.zero; }

        return rb.position;
    }

    public float GetSelectedCircleRadius() { return selectedCircleRadius; }


    public virtual dynamic GetSelf() { return this; }
    public virtual dynamic GetActionController() 
    { 
        if(GetComponent<UnitActionController>() == null)
            return null;
        return GetComponent<UnitActionController>(); 
    }
    public virtual dynamic ReturnSelfType() { return typeof(Marine); }
    public virtual dynamic ReturnNewAction() { return new MarineAction(); }
    public virtual dynamic ReturnActionType() { return typeof(MarineAction); }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class Structure : MonoBehaviour, IStructure
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
    protected float setVisionRange;
    [SerializeField]
    protected float setBuildingTime;

    protected float curHealth;

    protected GameObject selectedCircle;

    //Components
    protected Rigidbody rb;
    protected Collider col;

    //Components

    public virtual void Awake()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        col = gameObject.GetComponent<Collider>();

        setVisionRange *= FindObjectOfType<GridController>().cellRadius * 2;

        curHealth = setHealth;

        selectedCircle = transform.GetChild(0).gameObject;

        rb.velocity = Vector3.zero;
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

    public float GetSetHealth() { return setHealth; }
    public float GetSetVisionRange() { return setVisionRange; }
    public float GetSetBuildingTime() { return setBuildingTime; }

    public float GetCurrentHealth() { return curHealth; }

    public GameObject GetSelectedCircle() { return selectedCircle; }

    public Rigidbody GetRigidbody() { return rb; }
    public Collider GetCollider() { return col; }

    public IEntity.EntityType GetEntityType() { return IEntity.EntityType.Structure; }
    public Transform GetTransform() { return transform; }
    public Vector3 GetWorldPosition()
    {
        if (rb == null) { return Vector3.zero; }

        return rb.position;
    }

    public virtual dynamic GetSelf() { return this; }

}

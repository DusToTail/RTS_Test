using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class Structure : MonoBehaviour, EntityInterface
{
    [SerializeField]
    public EntityInterface.EntityTypes entityType;
    [SerializeField]
    public EntityInterface.RelationshipTypes relationshipType;
    [SerializeField]
    public Transform footPosition;
    [SerializeField]
    public Vector3 size3D;

    [SerializeField]
    private float setHealth = 50;
    [SerializeField]
    private float setVisionRange = 10;
    [SerializeField]
    private float setBuildingTime = 0;

    private float curHealth;

    private GameObject selectedCircle;

    //Components
    private Rigidbody rb;
    private Collider col;

    private void Awake()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        col = gameObject.GetComponent<Collider>();

        footPosition = transform;
        size3D = col.bounds.size;

        setVisionRange *= FindObjectOfType<GridController>().cellRadius * 2;

        curHealth = setHealth;

        selectedCircle = transform.GetChild(0).gameObject;

        rb.velocity = Vector3.zero;
    }

    private void Update()
    {
        if (HasNoHealth()) { Destroy(this.gameObject); }
    }

    public void HealthMinus(float amount)
    {
        if (amount < 0) { amount = 0; }

        curHealth -= amount;
        if (curHealth < 0) { curHealth = 0; }
    }

    public float GetMaxHealth() { return setHealth; }

    public float GetCurHealth() { return curHealth; }

    public float GetVisionRange() { return setVisionRange; }

    public float GetBuildingTime() { return setBuildingTime; }

    public void RenderSelectedCircle(bool isOn)
    {
        if (isOn == true)
            selectedCircle.SetActive(true);
        else
            selectedCircle.SetActive(false);
    }

    private bool HasNoHealth() { return curHealth <= 0; }

    // IMPLEMENTATION for EntityInterface
    public void DisplayPosition()
    {
        Debug.Log(this.gameObject.name + " is currently at position " + transform.position);
    }
    public void DisplayFootPosition()
    {
        Debug.Log(this.gameObject.name + " is currently at position " + footPosition.position);
    }
    public void DisplaySize()
    {
        Debug.Log(this.gameObject.name + " is currently at position " + size3D);
    }
    public EntityInterface.EntityTypes GetEntityType()
    {
        return entityType;
    }
    public EntityInterface.RelationshipTypes GetRelationshipType()
    {
        return relationshipType;
    }
    public GameObject GetGameObject()
    {
        return this.gameObject;
    }



}

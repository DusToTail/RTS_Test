using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class Unit : MonoBehaviour, EntityInterface
{
    // DESCRIPTION: class that contains the basic information (such as health and speed) and methods of all units.
    [SerializeField]
    public EntityInterface.EntityTypes entityType;
    [SerializeField]
    public EntityInterface.RelationshipTypes relationshipType;
    [SerializeField]
    public Transform footPosition;
    [SerializeField]
    public Vector3 size3D;


    [SerializeField]
    private int setHealth = 50;
    [SerializeField]
    private float setMovementSpeed = 0;
    [SerializeField]
    private int setAttackDamage = 5;
    [SerializeField]
    private int setAttackRange = 6;
    [SerializeField]
    private int setVisionRange = 10;
    [SerializeField]
    private float setBuildingTime = 0;

    private int curHealth;

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

        curHealth = setHealth;

        selectedCircle = transform.GetChild(0).gameObject;
        rb.velocity = Vector3.zero;

    }

    private void Start()
    {

    }

    private void Update()
    {
        AutoSeparationFromNearbyUnit(0.5f, 3f);
    }

    private void AutoSeparationFromNearbyUnit(float pushForce, float radius)
    {
        Collider[] colliders = Physics.OverlapSphere(rb.position, radius, LayerMask.GetMask(Tags.Selectable), QueryTriggerInteraction.Ignore);
        if(colliders.Length == 0) { return; }

        foreach(Collider collider in colliders)
        {
            if(collider.gameObject == this.gameObject) { continue; }

            Vector3 unitPos = collider.gameObject.GetComponent<Rigidbody>().position;
            Vector3 separateVector = rb.position - unitPos;
            if(separateVector.magnitude < radius)
            {
                Vector3 moveVector = separateVector.normalized * pushForce * (1 - Mathf.Clamp01(separateVector.magnitude / radius));
                rb.MovePosition(rb.position + moveVector * Time.deltaTime);
                //Debug.Log(gameObject.name + " is avoiding with " + force);
            }
        }
    }

    public void HealthMinus(int amount)
    {
        if(amount < 0) { amount = 0; }

        curHealth -= amount;
        if(curHealth < 0) { curHealth = 0; }
    }

    public int GetMaxHealth() { return setHealth; }

    public int GetCurHealth() { return curHealth; }

    public float GetMovementSpeed() { return setMovementSpeed; }

    public int GetAttackDamage() { return setAttackDamage; }

    public int GetAttackRange() { return setAttackRange; }

    public int GetVisionRange() { return setVisionRange; }

    public float GetBuildingTime() { return setBuildingTime; }

    public void RenderSelectedCircle(bool isOn)
    {
        if (isOn == true)
            selectedCircle.SetActive(true);
        else
            selectedCircle.SetActive(false);
    }

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

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * 2);
    }

}

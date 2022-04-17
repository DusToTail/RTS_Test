using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class Unit : MonoBehaviour, EntityInterface
{
    // DESCRIPTION: class that contains the basic information (such as health and speed) and methods of all units.

    public EntityInterface.EntityTypes entityType;
    public Transform footPosition;
    public Vector3 size3D;

    [SerializeField]
    private float setMovementSpeed = 0;

    [SerializeField]
    private int setVisionRange = 10;

    private GameObject selectedCircle;

    //Components
    private Rigidbody rb;
    private Collider col;

    private void Awake()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        col = gameObject.GetComponent<Collider>();

        entityType = EntityInterface.EntityTypes.SelectableUnit;
        footPosition = transform;
        size3D = col.bounds.size;

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

    public float GetMovementSpeed() { return setMovementSpeed; }

    public int GetVisionRange() { return setVisionRange; }

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

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * 2);
    }

}

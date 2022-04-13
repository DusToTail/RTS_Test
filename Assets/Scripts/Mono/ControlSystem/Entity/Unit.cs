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

    }

    private void Start()
    {

    }

    private void Update()
    {

    }

    public float GetMovementSpeed()
    {
        return setMovementSpeed;
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


}

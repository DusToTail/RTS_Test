using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementController : MonoBehaviour
{
    [SerializeField]
    private float movementSpeed;

    public Rigidbody rb { get; private set; }
    private Vision vision;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        vision = GetComponent<Vision>();

    }

    public float GetMovementSpeed() { return movementSpeed; }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TechStructure : Structure
{
    private float curProductionTime;

    public override void Awake()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        col = gameObject.GetComponent<Collider>();

        setVisionRange *= FindObjectOfType<GridController>().cellRadius * 2;

        curHealth = setHealth;

        selectedCircle = transform.GetChild(0).gameObject;

        rb.velocity = Vector3.zero;
        Debug.Log($"Tech Structure {gameObject.name} Constructed");

    }

    public override void Start()
    {

    }


    public override void Update()
    {
        if (HealthIsZero()) { Destroy(this.gameObject); }
    }



    public float GetCurrentProductionTime() { return curProductionTime; }


}

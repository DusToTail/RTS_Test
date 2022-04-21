using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProductionStructure : Structure
{
    [SerializeField]
    private GameObject[] unitPrefabs;

    private float curProductionTime;
    
    public override void Awake()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        col = gameObject.GetComponent<Collider>();

        setVisionRange *= FindObjectOfType<GridController>().cellRadius * 2;

        curHealth = setHealth;

        selectedCircle = transform.GetChild(0).gameObject;

        rb.velocity = Vector3.zero;
        Debug.Log($"Production Structure {gameObject.name} Constructed");

    }

    // Start is called before the first frame update
    public override void Start()
    {

    }


    // Update is called once per frame
    public override void Update()
    {
        if (HealthIsZero()) { Destroy(this.gameObject); }
    }



    public GameObject[] GetUnitPrefabs() { return unitPrefabs; }
    public float GetCurrentProductionTime() { return curProductionTime; }


}

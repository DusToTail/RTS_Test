using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityInfo : MonoBehaviour, IEntity
{
    [Header("Basic Info")]
    [SerializeField]
    private string entityName;
    [SerializeField]
    private string entityDescription;
    [SerializeField]
    private IEntity.Type entityType;
    [SerializeField]
    private Sprite portrait;

    [Header("Others")]
    [SerializeField]
    private int playerIndex;
    [SerializeField]
    private bool isSelectable;
    [SerializeField]
    private GameObject selectedCircle;
    [SerializeField]
    private float selectedCircleRadius;
    [SerializeField]
    private GameObject miniMapSelf;

    //Å@Components
    private Rigidbody rb;
    private Collider col;

    private void Start()
    {
        // Initialization
        rb = gameObject.GetComponent<Rigidbody>();
        col = gameObject.GetComponent<Collider>();

        selectedCircle = transform.GetChild(0).gameObject;
        Vector3[] vertices = new Vector3[4];
        vertices[0] = new Vector3(-selectedCircleRadius, 0, -selectedCircleRadius);
        vertices[1] = new Vector3(-selectedCircleRadius, 0, selectedCircleRadius);
        vertices[2] = new Vector3(selectedCircleRadius, 0, selectedCircleRadius);
        vertices[3] = new Vector3(selectedCircleRadius, 0, -selectedCircleRadius);

        selectedCircle.GetComponent<LineRenderer>().SetPositions(vertices);
        selectedCircleRadius = Mathf.Sqrt(col.bounds.extents.x * col.bounds.extents.x + col.bounds.extents.z * col.bounds.extents.z);

        miniMapSelf.SetActive(true);

    }

    public bool IsSelectable() { return isSelectable; }

    public string GetName() { return entityName; }
    public string GetDescription() { return entityDescription; }
    public IEntity.Type GetEntityType() { return entityType; }
    public int GetPlayerIndex() { return playerIndex; }
    public Sprite GetPortrait() { return portrait; }
    public GameObject GetSelectedCircle() { return selectedCircle; }
    public float GetSelectedCircleRadius() { return selectedCircleRadius; }
    public Transform GetTransform() { return transform; }
    public Vector3 GetWorldPosition() 
    {
        if(transform == null)
            return Vector3.zero;
        if(rb == null)
            return transform.position;
        return rb.position; 
    }

    public EntityInfo GetSelf() { return this; }

    public void RenderSelectedCircle(bool _isOn)
    {
        if (selectedCircle == null) { return; }
        selectedCircle.SetActive(_isOn);
    }
}

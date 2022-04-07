using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectSystem : MonoBehaviour
{
    public List<GameObject> unitList { get; private set; }

    [SerializeField]
    private RectTransform selectFieldPosition;
    [SerializeField]
    private CanvasScaler UI;
    [SerializeField]
    private Transform groundWorldPosition;

    [SerializeField]
    private bool displayGizmos;

    private Vector3 leftClickMousePosition_Click;
    private Vector3 leftClickMousePosition_Release;
    private Vector3 rightClickMousePosition_Click;
    private Vector3 rightClickMousePosition_Release;


    void Start()
    {
        unitList = new List<GameObject>();
        leftClickMousePosition_Click = Vector3.zero;
        leftClickMousePosition_Release = Vector3.zero;
        rightClickMousePosition_Click = Vector3.zero;
        rightClickMousePosition_Release = Vector3.zero;
    }

    void Update()
    {
        //DEBUG
        if (Input.GetKeyDown(KeyCode.Space))
            DisplayUnitList();
        UpdateSelectFieldTransform();



        if(Input.GetMouseButtonDown(0))
        {
            unitList.Clear();
            Vector3 worldMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            leftClickMousePosition_Click = new Vector3(worldMousePos.x, groundWorldPosition.position.y, worldMousePos.z);
            Debug.Log("Left Clicked at " + leftClickMousePosition_Click);
        }

        if(Input.GetMouseButtonUp(0))
        {
            Vector3 worldMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            leftClickMousePosition_Release = new Vector3(worldMousePos.x, groundWorldPosition.position.y, worldMousePos.z);
            AddUnitsToList();
            Debug.Log("Left Released at " + leftClickMousePosition_Release);
        }

        if (Input.GetMouseButtonDown(1))
        {
            Vector3 worldMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            rightClickMousePosition_Click = new Vector3(worldMousePos.x, groundWorldPosition.position.y, worldMousePos.z);
            Debug.Log("Right Released at " + rightClickMousePosition_Click);
        }

        if (Input.GetMouseButtonUp(1))
        {
            Vector3 worldMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            rightClickMousePosition_Release = new Vector3(worldMousePos.x, groundWorldPosition.position.y, worldMousePos.z);
            Debug.Log("Right Released at " + rightClickMousePosition_Release);
        }
    }


    private void AddUnitsToList()
    {
        Vector3 checkPosition = (leftClickMousePosition_Click + leftClickMousePosition_Release) / 2;
        int selectHitBoxSize = Mathf.FloorToInt(Vector3.Distance(leftClickMousePosition_Click, leftClickMousePosition_Release) / Mathf.Sqrt(2));
        Vector3 selectHitBox = Vector3.one * selectHitBoxSize + Vector3.up * 1000;
        Debug.Log(checkPosition);
        Debug.Log(selectHitBoxSize);
        Debug.Log(selectHitBox);
        Collider[] colliders = Physics.OverlapBox(checkPosition, selectHitBox, Quaternion.identity, LayerMask.NameToLayer(Tags.Selectable));

        Debug.Log(colliders.Length);
        foreach(Collider collider in colliders)
        {
            if (collider.gameObject.GetComponent<Unit>().entityType == EntityInterface.EntityTypes.SelectableUnit)
            {
                unitList.Add(collider.gameObject);
                Debug.Log("Added " + collider.gameObject.name);
            }
        }
    }

    private void UpdateSelectFieldTransform()
    {
        Vector3 mousePos = Input.mousePosition;
        float percentX = Mathf.Clamp01(mousePos.x / Camera.main.scaledPixelWidth);
        float percentY = Mathf.Clamp01(mousePos.y / Camera.main.scaledPixelHeight);

        selectFieldPosition.anchoredPosition = new Vector2(percentX * UI.referenceResolution.x, percentY * UI.referenceResolution.y);
    }

    private void OnDrawGizmos()
    {
        if(!displayGizmos) { return; }
        Gizmos.color = Color.red;
        Vector3 checkPosition = (leftClickMousePosition_Click + leftClickMousePosition_Release) / 2;
        int selectHitBoxSize = Mathf.FloorToInt(Vector3.Distance(leftClickMousePosition_Click, leftClickMousePosition_Release) / Mathf.Sqrt(2));
        Vector3 selectHitBox = Vector3.one * selectHitBoxSize + Vector3.up * 100;
        Gizmos.DrawWireCube(checkPosition, selectHitBox);
    }

    private void DisplayUnitList()
    {
        if(unitList.Count == 0) { Debug.Log("No object in Unit List"); return; }
        for (int i = 0; i < unitList.Count; i++)
            Debug.Log("Object No." + i + " in [Unit List in Select System] is: " + unitList[i].name);
    }
}

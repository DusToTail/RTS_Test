using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectSystem : MonoBehaviour
{
    /// <summary>
    /// DESCRIPTION: A system that allows user to select and/or command entities via clicking and dragging mouse
    /// Contains the following features:
    /// 1. Select units
    /// 2. Command units
    /// 3. UI interface (select field and cursor display)
    /// 
    /// Works with Canvas-related components,
    /// </summary>
    public List<Unit> unitList { get; private set; }

    // UI reference
    [SerializeField]
    private Transform groundWorldPosition; // In World Space, refer to the ground, used for readjusting y-axis coordinate when detecting colliders
    [SerializeField]
    private RectTransform cursorPosition; // In Screen Space, in pixels
    [SerializeField]
    private CanvasScaler canvasScaler; // To get screen size in pixels
    [SerializeField]
    private LineRenderer selectFieldBox; // An object under Canvas, with 4 vertices (coord in pixels) to create a box for select field
    [SerializeField]
    private float selectFieldWidth; // width of the select field lines

    [SerializeField]
    private bool displayGizmos;

    // Mouse click-release position in World Space
    // Å¶Note: the game is set on xz plane, thus y-axis would indicate height.
    private Vector3 leftClickMousePosition_Click;
    private Vector3 leftClickMousePosition_Release;
    private Vector3 rightClickMousePosition_Click;
    private Vector3 rightClickMousePosition_Release;


    void Start()
    {
        //Initialization
        unitList = new List<Unit>();
        ResetClickReleasePosition();
    }

    void Update()
    {
        //DEBUG (not working!)
        if (Input.GetKeyDown(KeyCode.Space))
            DisplayUnitList();

        //Move an image in replacement of the default cursor on the screen
        UpdateCursorTransform();

        // LEFT CLICK DOWN
        if (Input.GetMouseButtonDown(0))
        {
            // Calculate and initially synchronize both left click and release position
            Vector3 worldMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            leftClickMousePosition_Click = new Vector3(worldMousePos.x, groundWorldPosition.position.y, worldMousePos.z);
            leftClickMousePosition_Release = leftClickMousePosition_Click;

            // Reset unit list
            unitList.Clear();
            unitList.TrimExcess();

            //Debug.Log("Left Clicked at " + leftClickMousePosition_Click);
        }

        // LEFT CLICK HELD
        // Render Select Field
        if (Input.GetMouseButton(0))
        {
            selectFieldBox.enabled = true;
            RenderSelectField();
        }
        else { selectFieldBox.enabled = false; }

        // LEFT CLICK UP
        if (Input.GetMouseButtonUp(0))
        {
            // Calculate left click release position
            Vector3 worldMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            leftClickMousePosition_Release = new Vector3(worldMousePos.x, groundWorldPosition.position.y, worldMousePos.z);

            //Add units detected in a box collider (with limits determined by left click-release position) to the list
            AddUnitsToList();

            //Debug.Log("Left Released at " + leftClickMousePosition_Release);
        }

        // RIGHT CLICK DOWN
        if (Input.GetMouseButtonDown(1))
        {
            // Move Command
            // Determine to Queue command OR Overwrite past commands by holding SHIFT or not
            bool isInstant = true;
            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
                isInstant = false;

            // Calculate right click position
            Vector3 worldMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            rightClickMousePosition_Click = new Vector3(worldMousePos.x, groundWorldPosition.position.y, worldMousePos.z);

            //Create a Select Group object, containing ALL actions to be created and ONE flow field
            GameObject selectGroup = new GameObject("SelectGroup " + transform.childCount);
            selectGroup.transform.SetParent(transform);
            selectGroup.AddComponent<SelectGroup>();
            selectGroup.GetComponent<SelectGroup>().InitializeFlowField(worldMousePos);
            
            if (unitList.Count == 0) { return; }
            foreach (Unit curUnit in unitList)
            {
                // Create a unique action to enqueue for each unit
                // Initialize select group for future reference (flow field for navigation)
                Action curMoveAction = new Action(Action.ActionTypes.MoveTowards);
                curMoveAction.InitializeMousePosition(rightClickMousePosition_Click);
                curMoveAction.InitializeSelectGroupObject(ref selectGroup);

                // Enqueue action
                curUnit.gameObject.GetComponent<ActionController>().EnqueueAction(ref curMoveAction, isInstant);

                // Add action to SelectGroup for future reference (self-destruct when no longer used by any action)
                selectGroup.GetComponent<SelectGroup>().AddToActionList(ref curMoveAction);
            }


            //Debug.Log("Right Released at " + rightClickMousePosition_Click);

            //DEBUG: draw the created flowfield with icons for each cell, indicating path and destination
            GridDebug.SetCurFlowField(selectGroup.GetComponent<SelectGroup>().groupFlowField);
            GridDebug.DrawFlowField();
        }

        //RIGHT CLICK UP
        if (Input.GetMouseButtonUp(1))
        {
            // Calculate right click release position
            Vector3 worldMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            rightClickMousePosition_Release = new Vector3(worldMousePos.x, groundWorldPosition.position.y, worldMousePos.z);

            //Debug.Log("Right Released at " + rightClickMousePosition_Release);
        }
    }

    // Add units detected by Physics.OverlapBox() at the center position of click and release, with offset towards the ground.
    private void AddUnitsToList()
    {
        Vector3 sum = leftClickMousePosition_Click + leftClickMousePosition_Release;
        Vector3 subtract = leftClickMousePosition_Click - leftClickMousePosition_Release;
        Vector3 checkPosition = sum / 2;
        Vector3 checkBoxSize2D = new Vector3(Mathf.Abs(subtract.x), 0, Mathf.Abs(subtract.z));
        Vector3 selectHitBox =   checkBoxSize2D + Vector3.up * 100;

        Collider[] colliders = Physics.OverlapBox(checkPosition, selectHitBox / 2, Quaternion.identity, LayerMask.GetMask(Tags.Selectable));

        foreach(Collider collider in colliders)
        {
            if (collider.gameObject.GetComponent<Unit>().entityType == EntityInterface.EntityTypes.SelectableUnit)
            {
                unitList.Add(collider.gameObject.GetComponent<Unit>());
            }
        }
    }

    // Update the position of the cursor basing on the mouse position (all in pixels, in screen space).
    // Anchored Point is bottom left of the map
    private void UpdateCursorTransform()
    {
        Vector3 mousePos = Input.mousePosition;
        float percentX = Mathf.Clamp01(mousePos.x / Camera.main.scaledPixelWidth);
        float percentY = Mathf.Clamp01(mousePos.y / Camera.main.scaledPixelHeight);

        cursorPosition.anchoredPosition = new Vector2(percentX * canvasScaler.referenceResolution.x, percentY * canvasScaler.referenceResolution.y);
    }

    // Render Select Field as an empty rectangle, determined by 4 vertices (in pixel coord, in screen space), connected by a Line Renderer
    // Anchored Point is bottom left of the map
    private void RenderSelectField()
    {
        selectFieldBox.startWidth = selectFieldWidth;
        selectFieldBox.endWidth = selectFieldWidth;
        

        Vector3 leftClickPos_Click = Camera.main.WorldToScreenPoint(leftClickMousePosition_Click);
        float percentX_Click = Mathf.Clamp01(leftClickPos_Click.x / Camera.main.scaledPixelWidth);
        float percentY_Click = Mathf.Clamp01(leftClickPos_Click.y / Camera.main.scaledPixelHeight);

        Vector3 mousePos_Cur = Input.mousePosition;
        float percentX_Cur = Mathf.Clamp01(mousePos_Cur.x / Camera.main.scaledPixelWidth);
        float percentY_Cur = Mathf.Clamp01(mousePos_Cur.y / Camera.main.scaledPixelHeight);


        Vector3[] vertexArray = new Vector3[4];
        vertexArray[0] = new Vector3(percentX_Click * canvasScaler.referenceResolution.x, percentY_Click * canvasScaler.referenceResolution.y, 0);
        vertexArray[1] = new Vector3(percentX_Cur * canvasScaler.referenceResolution.x, percentY_Click * canvasScaler.referenceResolution.y, 0);
        vertexArray[2] = new Vector3(percentX_Cur * canvasScaler.referenceResolution.x, percentY_Cur * canvasScaler.referenceResolution.y, 0);
        vertexArray[3] = new Vector3(percentX_Click * canvasScaler.referenceResolution.x, percentY_Cur * canvasScaler.referenceResolution.y, 0);


        selectFieldBox.SetPositions(vertexArray);
    }

    private void ResetClickReleasePosition()
    {
        leftClickMousePosition_Click = Vector3.zero;
        leftClickMousePosition_Release = Vector3.zero;
        rightClickMousePosition_Click = Vector3.zero;
        rightClickMousePosition_Release = Vector3.zero;
    }

    // DEBUG
    private void OnDrawGizmos()
    {
        if(!displayGizmos) { return; }
        Gizmos.color = Color.red;
        Vector3 sum = leftClickMousePosition_Click + leftClickMousePosition_Release;
        Vector3 subtract = leftClickMousePosition_Click - leftClickMousePosition_Release;
        Vector3 checkPosition = sum / 2;
        Vector3 checkBoxSize2D = new Vector3(Mathf.Abs(subtract.x), 0, Mathf.Abs(subtract.z));
        Vector3 selectHitBox = checkBoxSize2D + Vector3.up * 100;

        Gizmos.DrawWireCube(checkPosition, selectHitBox);

        DisplayUnitList();

    }

    private void DisplayUnitList()
    {
        if (Application.isEditor) { return; }
        if(unitList.Count == 0) { return; }
        for (int i = 0; i < unitList.Count; i++)
        {
            Debug.Log("Drawing");
            Gizmos.color = Color.blue;
            Gizmos.DrawIcon(unitList[i].gameObject.transform.position + Vector3.up * 10, unitList[i].gameObject.name);
        }
    }
}

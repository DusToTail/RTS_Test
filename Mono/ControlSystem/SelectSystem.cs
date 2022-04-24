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
    public List<IUnit> unitList { get; private set; }
    public List<IStructure> structureList { get; private set; }
    public List<IEntity> selectableList { get; private set; }
    public List<SelectGroup> selectGroupList { get; private set; }
    public SelectGroup curSelectGroup { get; private set; }
    
    public enum InputMode
    {
        MoveButton,
        PatrolButton,
        AttackButton,
        None
    }
    public InputMode inputMode { get; private set; }

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
    public Vector3 leftClickMousePosition_Click { get; private set; }
    public Vector3 leftClickMousePosition_Release {get; private set;}
    public Vector3 rightClickMousePosition_Click {get; private set;}
    public Vector3 rightClickMousePosition_Release {get; private set;}

    
    private void Awake()
    {
        //Initialization
        unitList = new List<IUnit>();
        structureList = new List<IStructure>();
        selectableList = new List<IEntity>();

        selectGroupList = new List<SelectGroup>();
        selectGroupList.Capacity = 10;
        curSelectGroup = null;

        ResetClickReleasePosition();
        inputMode = InputMode.None;
    }

    private void Start()
    {
        
    }

    private void Update()
    {
        //Move an image in replacement of the default cursor on the screen
        UpdateCursorTransform();

        // INPUT MODE
        if(Input.GetKeyDown(KeyCode.A))
        {
            inputMode = InputMode.AttackButton;
        }
        else if(Input.GetKeyDown(KeyCode.M))
        {
            inputMode = InputMode.MoveButton; 
        }
        else if(Input.GetKeyDown(KeyCode.P))
        {
            inputMode = InputMode.PatrolButton;
        }

        // SELECT GROUP INDEXING
        if(selectableList.Count > 0)
        {
            if(Input.GetKeyDown(KeyCode.Keypad1))
            {
                AddToSelectGroupList(0);
                AssignCurrentSelectGroup(0);
            }
            if (Input.GetKeyDown(KeyCode.Keypad2))
            {
                AddToSelectGroupList(1);
                AssignCurrentSelectGroup(1);
            }
            if (Input.GetKeyDown(KeyCode.Keypad3))
            {
                AddToSelectGroupList(2);
                AssignCurrentSelectGroup(2);
            }
            if (Input.GetKeyDown(KeyCode.Keypad4))
            {
                AddToSelectGroupList(3);
                AssignCurrentSelectGroup(3);
            }
            if (Input.GetKeyDown(KeyCode.Keypad5))
            {
                AddToSelectGroupList(4);
                AssignCurrentSelectGroup(4);
            }
            if (Input.GetKeyDown(KeyCode.Keypad6))
            {
                AddToSelectGroupList(5);
                AssignCurrentSelectGroup(5);
            }
            if (Input.GetKeyDown(KeyCode.Keypad7))
            {
                AddToSelectGroupList(6);
                AssignCurrentSelectGroup(6);
            }
            if (Input.GetKeyDown(KeyCode.Keypad8))
            {
                AddToSelectGroupList(7);
                AssignCurrentSelectGroup(7);
            }
            if (Input.GetKeyDown(KeyCode.Keypad9))
            {
                AddToSelectGroupList(8);
                AssignCurrentSelectGroup(8);
            }
            if (Input.GetKeyDown(KeyCode.Keypad0))
            {
                AddToSelectGroupList(9);
                AssignCurrentSelectGroup(9);
            }
        }

        // MOUSE INPUT
        // LEFT CLICK DOWN
        if (Input.GetMouseButtonDown(0))
        {
            // Calculate and initially synchronize both left click and release position
            Vector3 worldMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            leftClickMousePosition_Click = new Vector3(worldMousePos.x, groundWorldPosition.position.y, worldMousePos.z);
            leftClickMousePosition_Release = leftClickMousePosition_Click;

            // Turn selected circle of past units off
            RenderSelectedCircles(false);

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

            if (inputMode == InputMode.None)
            {
                // Reset selectables list
                ClearLists();

                // Add units detected in a box collider (with limits determined by left click-release position) to the list
                AddSelectablesToList();

                // Turn selected circle of past units off
                RenderSelectedCircles(true);
            }


            SetInputMode(InputMode.None);

            //Debug.Log("Left Released at " + leftClickMousePosition_Release);
        }

        // RIGHT CLICK DOWN
        if (Input.GetMouseButtonDown(1))
        {
            // Calculate right click position
            Vector3 worldMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            rightClickMousePosition_Click = new Vector3(worldMousePos.x, groundWorldPosition.position.y, worldMousePos.z);
            rightClickMousePosition_Release = rightClickMousePosition_Click;
            //Debug.Log("Right Released at " + rightClickMousePosition_Click);

        }

        // RIGHT CLICK HELD
        if (Input.GetMouseButton(1))
        {

        }

        //RIGHT CLICK UP
        if (Input.GetMouseButtonUp(1))
        {
            // Calculate right click release position
            Vector3 worldMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            rightClickMousePosition_Release = new Vector3(worldMousePos.x, groundWorldPosition.position.y, worldMousePos.z);

            SetInputMode(InputMode.None);

            //Debug.Log("Right Released at " + rightClickMousePosition_Release);
        }
    }

    public void SetInputMode(InputMode mode)
    {
        inputMode = mode;
    }


    // Add units detected by Physics.OverlapBox() at the center position of click and release, with offset towards the ground.
    private void AddSelectablesToList()
    {
        Vector3 sum = leftClickMousePosition_Click + leftClickMousePosition_Release;
        Vector3 subtract = leftClickMousePosition_Click - leftClickMousePosition_Release;
        Vector3 checkPosition = sum / 2;
        Vector3 checkBoxSize2D = new Vector3(Mathf.Abs(subtract.x), 0, Mathf.Abs(subtract.z));
        Vector3 selectHitBox =   checkBoxSize2D + Vector3.up * 100;

        Collider[] colliders = Physics.OverlapBox(checkPosition, selectHitBox / 2, Quaternion.identity, LayerMask.GetMask(Tags.Selectable));

        foreach(Collider collider in colliders)
        {
            if(collider.gameObject.GetComponent<IEntity>().GetRelationshipType() == IEntity.RelationshipType.Enemy) { continue; }

            if (collider.gameObject.GetComponent<IEntity>().GetSelectionType() == IEntity.SelectionType.Selectable)
            {
                selectableList.Add(collider.gameObject.GetComponent<IEntity>());
                if (collider.gameObject.GetComponent<IEntity>() is IUnit)
                {
                    unitList.Add(collider.gameObject.GetComponent<Unit>());
                    //Debug.Log($"Added {collider.gameObject.name}");
                }
                else if (collider.gameObject.GetComponent<IEntity>() is IStructure)
                {
                    structureList.Add(collider.gameObject.GetComponent<Structure>());
                    //Debug.Log($"Added {collider.gameObject.name}");
                }
            }
        }
    }

    private void AddToSelectGroupList(int _groupIndex)
    {
        GameObject selectGroup = new GameObject($"Static SelectGroup No.{_groupIndex}");
        selectGroup.transform.SetParent(transform);
        selectGroup.AddComponent<SelectGroup>();

        for (int index = 0; index < structureList.Count; index++)
        {
            selectGroup.GetComponent<SelectGroup>().entityList.Add(structureList[index]);
        }
        for(int index = 0; index < unitList.Count; index++)
        {
            selectGroup.GetComponent<SelectGroup>().entityList.Add(unitList[index]);
        }

        selectGroupList.Insert(_groupIndex, selectGroup.GetComponent<SelectGroup>());
    }
    private void AssignCurrentSelectGroup(int _groupIndex)
    {
        curSelectGroup = selectGroupList[_groupIndex];
        ClearLists();
        foreach(IEntity curEntity in curSelectGroup.entityList)
        {
            selectableList.Add(curEntity);
            if(curEntity is IUnit unit)
            {
                unitList.Add(unit);
            }
            else if(curEntity is IStructure structure)
            {
                structureList.Add(structure);
            }
        }
    }

    private void ClearLists()
    {
        unitList.Clear();
        unitList.TrimExcess();
        structureList.Clear();
        structureList.TrimExcess();
        selectableList.Clear();
        selectableList.TrimExcess();
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

    // Render Selected Circles of All Selectables
    private void RenderSelectedCircles(bool isOn)
    {
        if (unitList.Count > 0)
        {
            foreach (Unit curUnit in unitList)
            {
                curUnit.RenderSelectedCircle(isOn);
            }
        }

        if (structureList.Count > 0)
        {
            foreach (Structure curStructure in structureList)
            {
                curStructure.RenderSelectedCircle(isOn);
            }
        }
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

    }

}

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
    public List<IUnit> unitList { get; set; }
    public List<IStructure> structureList { get; set; }
    public List<IEntity> selectableList { get; set; }
    public List<SelectGroup> selectGroupList { get; set; }
    public SelectGroup curSelectGroup { get; set; }
    
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
    private RectTransform cursorPosition; // In Screen Space, in pixels
    [SerializeField]
    private LineRenderer selectFieldBox; // An object under Canvas, with 4 vertices (coord in pixels) to create a box for select field
    [SerializeField]
    private float selectFieldWidth; // width of the select field lines

    [SerializeField]
    private bool displayGizmos;

    // Mouse click-release position in World Space
    // ��Note: the game is set on xz plane, thus y-axis would indicate height.
    public Vector3 leftClickMouseWorldPosition_Click { get; private set; }
    public Vector3 leftClickMouseWorldPosition_Release {get; private set;}
    public Vector3 rightClickMouseWorldPosition_Click {get; private set;}
    public Vector3 rightClickMouseWorldPosition_Release {get; private set;}

    
    private void Awake()
    {
        //Initialization
        unitList = new List<IUnit>();
        structureList = new List<IStructure>();
        selectableList = new List<IEntity>();

        selectGroupList = new List<SelectGroup>();
        selectGroupList.Capacity = 10;
        curSelectGroup = transform.Find("Temp SelectGroup").GetComponent<SelectGroup>();

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
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
                    AddToSelectGroupList(0);
                AssignCurrentSelectGroup(0);
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
                    AddToSelectGroupList(1);
                AssignCurrentSelectGroup(1);
            }
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
                    AddToSelectGroupList(2);
                AssignCurrentSelectGroup(2);
            }
            if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
                    AddToSelectGroupList(3);
                AssignCurrentSelectGroup(3);
            }
            if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
                    AddToSelectGroupList(4);
                AssignCurrentSelectGroup(4);
            }
            if (Input.GetKeyDown(KeyCode.Alpha6))
            {
                if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
                    AddToSelectGroupList(5);
                AssignCurrentSelectGroup(5);
            }
            if (Input.GetKeyDown(KeyCode.Alpha7))
            {
                if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
                    AddToSelectGroupList(6);
                AssignCurrentSelectGroup(6);
            }
            if (Input.GetKeyDown(KeyCode.Alpha8))
            {
                if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
                    AddToSelectGroupList(7);
                AssignCurrentSelectGroup(7);
            }
            if (Input.GetKeyDown(KeyCode.Alpha9))
            {
                if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
                    AddToSelectGroupList(8);
                AssignCurrentSelectGroup(8);
            }
            if (Input.GetKeyDown(KeyCode.Alpha0))
            {
                if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
                    AddToSelectGroupList(9);
                AssignCurrentSelectGroup(9);
            }
        }


        // MOUSE INPUT
        // LEFT CLICK DOWN
        if (Input.GetMouseButtonDown(0))
        {
            // Calculate and initially synchronize both left click and release position
            leftClickMouseWorldPosition_Click = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            leftClickMouseWorldPosition_Release = leftClickMouseWorldPosition_Click;
            RaycastHit hit;
            Physics.Raycast(leftClickMouseWorldPosition_Click, Camera.main.transform.forward, out hit, 1000, LayerMask.GetMask(Tags.Ground));

            if (curSelectGroup.entityList.Count > 0)
            {
                if (inputMode == InputMode.AttackButton)
                {
                    curSelectGroup.ResetSelectGroup();
                    bool isQueued = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
                    CommandSystem.AttackCommand(hit.point, curSelectGroup, !isQueued);
                }
                else if (inputMode == InputMode.MoveButton)
                {
                    curSelectGroup.ResetSelectGroup();
                    bool isQueued = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
                    CommandSystem.MoveCommand(hit.point, curSelectGroup, !isQueued);
                }
                else if (inputMode == InputMode.PatrolButton)
                {
                    curSelectGroup.ResetSelectGroup();
                    bool isQueued = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
                    CommandSystem.PatrolCommand(hit.point, curSelectGroup, !isQueued);
                }
                else if (inputMode == InputMode.None)
                {

                }
            }

            //Debug.Log("Left Clicked at " + leftClickMousePosition_Click);
        }

        // LEFT CLICK HELD
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
            leftClickMouseWorldPosition_Release = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            if (inputMode == InputMode.None)
            {
                // Turn selected circle of past units off
                RenderSelectedCircles(false);

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
            rightClickMouseWorldPosition_Click = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            rightClickMouseWorldPosition_Release = rightClickMouseWorldPosition_Click;
            //Debug.Log("Right Released at " + rightClickMousePosition_Click);
            RaycastHit hit;
            Physics.Raycast(rightClickMouseWorldPosition_Click, Camera.main.transform.forward, out hit, 1000, LayerMask.GetMask(Tags.Ground));

            if (curSelectGroup.entityList.Count > 0)
            {
                curSelectGroup.ResetSelectGroup();
                IEntity enemyAtMouse = CommandSystem.ReturnEntityAtMouse(IEntity.RelationshipType.Enemy, hit.point);
                if ( enemyAtMouse == null)
                {
                    bool isQueued = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
                    CommandSystem.MoveCommand(hit.point, curSelectGroup, !isQueued);
                }
                else
                {
                    bool isQueued = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
                    CommandSystem.AttackTargetCommand(enemyAtMouse, curSelectGroup, !isQueued);
                }
            }

        }

        // RIGHT CLICK HELD
        if (Input.GetMouseButton(1))
        {

        }

        //RIGHT CLICK UP
        if (Input.GetMouseButtonUp(1))
        {
            // Calculate right click release position
            rightClickMouseWorldPosition_Release = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            SetInputMode(InputMode.None);

            //Debug.Log("Right Released at " + rightClickMousePosition_Release);
        }
    }

    public void SetInputMode(InputMode mode)
    {
        inputMode = mode;
    }


    // Add units detected by Physics.OverlapBox() at the center position of click and release, with offset towards the ground.
    public void AddSelectablesToList()
    {
        Vector3 point1 = leftClickMouseWorldPosition_Click;
        Vector3 point2 = leftClickMouseWorldPosition_Release;

        Vector3 point3ViewPort = new Vector3(Camera.main.WorldToViewportPoint(point1).x, Camera.main.WorldToViewportPoint(point2).y, 0);
        Vector3 point3 = Camera.main.ViewportToWorldPoint(point3ViewPort);
        //Vector3 point4ViewPort = new Vector3(Camera.main.WorldToViewportPoint(point2).x, Camera.main.WorldToViewportPoint(point1).y, 0);
        //Vector3 point4 = Camera.main.ViewportToWorldPoint(point4ViewPort);
        Vector3 sum = point1 + point2;
        Vector3 checkPosition = sum / 2 + Camera.main.transform.forward * 100;
        Vector3 checkBoxSize = new Vector3((point2 - point3).magnitude, (point1 - point3).magnitude, 1000);

        Collider[] colliders = Physics.OverlapBox(checkPosition, checkBoxSize / 2, Camera.main.transform.rotation, LayerMask.GetMask(Tags.Selectable));
        if (colliders.Length == 1)
        {
            if (colliders[0].gameObject.GetComponent<IEntity>().GetSelectionType() == IEntity.SelectionType.Selectable)
            {
                selectableList.Add(colliders[0].gameObject.GetComponent<IEntity>());
                if (colliders[0].gameObject.GetComponent<IEntity>() is IUnit unit)
                {
                    unitList.Add(unit);
                    Debug.Log($"Added {colliders[0].gameObject.name}");
                }
                else if (colliders[0].gameObject.GetComponent<IEntity>() is IStructure structure)
                {
                    structureList.Add(structure);
                    Debug.Log($"Added {colliders[0].gameObject.name}");
                }
            }
        }
        else
        {
            foreach (Collider collider in colliders)
            {
                if (collider.gameObject.GetComponent<IEntity>().GetRelationshipType() == IEntity.RelationshipType.Enemy) { continue; }

                if (collider.gameObject.GetComponent<IEntity>().GetSelectionType() == IEntity.SelectionType.Selectable)
                {
                    selectableList.Add(collider.gameObject.GetComponent<IEntity>());
                    if (collider.gameObject.GetComponent<IEntity>() is IUnit unit)
                    {
                        unitList.Add(unit);
                        Debug.Log($"Added {collider.gameObject.name}");
                    }
                    else if (collider.gameObject.GetComponent<IEntity>() is IStructure structure)
                    {
                        structureList.Add(structure);
                        Debug.Log($"Added {collider.gameObject.name}");
                    }
                }
            }
        }

        if (selectableList.Count <= 0) { return; }

        GameObject selectGroup = new GameObject("Temp SelectGroup");
        selectGroup.transform.SetParent(transform);
        selectGroup.AddComponent<SelectGroup>();

        for (int index = 0; index < structureList.Count; index++)
        {
            selectGroup.GetComponent<SelectGroup>().entityList.Add(structureList[index]);
        }
        for (int index = 0; index < unitList.Count; index++)
        {
            selectGroup.GetComponent<SelectGroup>().entityList.Add(unitList[index]);
        }

        curSelectGroup = selectGroup.GetComponent<SelectGroup>();
        
    }

    public void AddToSelectGroupList(int _groupIndex)
    {
        if(transform.Find($"Static SelectGroup No.{_groupIndex}") == null)
        {
            GameObject selectGroup = new GameObject($"Static SelectGroup No.{_groupIndex}");
            selectGroup.transform.SetParent(transform);
            selectGroup.AddComponent<SelectGroup>();

            for (int index = 0; index < structureList.Count; index++)
            {
                selectGroup.GetComponent<SelectGroup>().entityList.Add(structureList[index]);
            }
            for (int index = 0; index < unitList.Count; index++)
            {
                selectGroup.GetComponent<SelectGroup>().entityList.Add(unitList[index]);
            }
            selectGroupList.Insert(_groupIndex, selectGroup.GetComponent<SelectGroup>());
        }
        else
        {
            selectGroupList[_groupIndex].entityList.Clear();
            selectGroupList[_groupIndex].entityList.TrimExcess();
            for (int index = 0; index < structureList.Count; index++)
            {
                selectGroupList[_groupIndex].entityList.Add(structureList[index]);
            }
            for (int index = 0; index < unitList.Count; index++)
            {
                selectGroupList[_groupIndex].entityList.Add(unitList[index]);
            }
        }



        Debug.Log($"Added Static SelectGroup No.{_groupIndex}");
    }

    public void AssignCurrentSelectGroup(int _groupIndex)
    {
        if(_groupIndex > selectGroupList.Count - 1) { return; }
        if(selectGroupList[_groupIndex] == null) { Debug.Log($"Static SelectGroup No.{_groupIndex} is NULL"); return; }

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
        RenderSelectedCircles(true);
        Debug.Log($"Assign Current SelectGroup to SelectGroup No.{_groupIndex}");
    }

    // Render Selected Circles of All Selectables
    public void RenderSelectedCircles(bool isOn)
    {
        if (unitList.Count > 0)
        {
            foreach (IUnit curUnit in unitList)
            {
                curUnit.RenderSelectedCircle(isOn);
            }
        }

        if (structureList.Count > 0)
        {
            foreach (IStructure curStructure in structureList)
            {
                curStructure.RenderSelectedCircle(isOn);
            }
        }
    }

    public void ClearLists()
    {
        unitList.Clear();
        unitList.TrimExcess();
        structureList.Clear();
        structureList.TrimExcess();
        selectableList.Clear();
        selectableList.TrimExcess();
        Debug.Log("Clear All List");
    }

    // Update the position of the cursor basing on the mouse position (all in pixels, in screen space).
    // Anchored Point is bottom left of the map
    private void UpdateCursorTransform()
    {
        Vector3 mousePos = Input.mousePosition;
        float percentX = Mathf.Clamp01(mousePos.x / Camera.main.scaledPixelWidth);
        float percentY = Mathf.Clamp01(mousePos.y / Camera.main.scaledPixelHeight);

        cursorPosition.anchoredPosition = new Vector2(percentX * Screen.currentResolution.width, percentY * Screen.currentResolution.height);
    }

    // Render Select Field as an empty rectangle, determined by 4 vertices (in pixel coord, in screen space), connected by a Line Renderer
    // Anchored Point is bottom left of the map
    private void RenderSelectField()
    {
        selectFieldBox.startWidth = selectFieldWidth;
        selectFieldBox.endWidth = selectFieldWidth;
        

        Vector3 leftClickPos_Click = Camera.main.WorldToScreenPoint(leftClickMouseWorldPosition_Click);
        float percentX_Click = Mathf.Clamp01(leftClickPos_Click.x / Camera.main.scaledPixelWidth);
        float percentY_Click = Mathf.Clamp01(leftClickPos_Click.y / Camera.main.scaledPixelHeight);

        Vector3 mousePos_Cur = Input.mousePosition;
        float percentX_Cur = Mathf.Clamp01(mousePos_Cur.x / Camera.main.scaledPixelWidth);
        float percentY_Cur = Mathf.Clamp01(mousePos_Cur.y / Camera.main.scaledPixelHeight);


        Vector3[] vertexArray = new Vector3[4];
        vertexArray[0] = new Vector3(percentX_Click * Screen.currentResolution.width, percentY_Click * Screen.currentResolution.height, 0);
        vertexArray[1] = new Vector3(percentX_Cur * Screen.currentResolution.width, percentY_Click * Screen.currentResolution.height, 0);
        vertexArray[2] = new Vector3(percentX_Cur * Screen.currentResolution.width, percentY_Cur * Screen.currentResolution.height, 0);
        vertexArray[3] = new Vector3(percentX_Click * Screen.currentResolution.width, percentY_Cur * Screen.currentResolution.height, 0);


        selectFieldBox.SetPositions(vertexArray);
    }

    

    private void ResetClickReleasePosition()
    {
        leftClickMouseWorldPosition_Click = Vector3.zero;
        leftClickMouseWorldPosition_Release = Vector3.zero;
        rightClickMouseWorldPosition_Click = Vector3.zero;
        rightClickMouseWorldPosition_Release = Vector3.zero;
    }

    
    // DEBUG
    private void OnDrawGizmos()
    {
        if(!displayGizmos) { return; }
        Gizmos.color = Color.red;

        Vector3 point1 = leftClickMouseWorldPosition_Click;
        Vector3 point2 = leftClickMouseWorldPosition_Release;

        Vector3 point3ViewPort = new Vector3(Camera.main.WorldToViewportPoint(point1).x, Camera.main.WorldToViewportPoint(point2).y, 0);
        Vector3 point3 = Camera.main.ViewportToWorldPoint(point3ViewPort);
        Vector3 point4ViewPort = new Vector3(Camera.main.WorldToViewportPoint(point2).x, Camera.main.WorldToViewportPoint(point1).y, 0);
        Vector3 point4 = Camera.main.ViewportToWorldPoint(point4ViewPort);
        Gizmos.DrawLine(point1, point3);
        Gizmos.DrawLine(point3, point2);
        Gizmos.DrawLine(point2, point4);
        Gizmos.DrawLine(point4, point1);

        Gizmos.DrawLine(point1, point1 + Camera.main.transform.forward * 1000);
        Gizmos.DrawLine(point2, point2 + Camera.main.transform.forward * 1000);
        Gizmos.DrawLine(point3, point3 + Camera.main.transform.forward * 1000);
        Gizmos.DrawLine(point4, point4 + Camera.main.transform.forward * 1000);


    }

}

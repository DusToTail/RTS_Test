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
    public List<Structure> structureList { get; private set; }

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

    private enum InputMode
    {
        MoveButton,
        PatrolButton,
        AttackButton,
        None
    }
    private InputMode inputMode;

    void Start()
    {
        //Initialization
        unitList = new List<Unit>();
        structureList = new List<Structure>();
        ResetClickReleasePosition();
        inputMode = InputMode.None;
    }

    void Update()
    {
        //DEBUG (not working!)
        if (Input.GetKeyDown(KeyCode.Space))
            DisplayUnitList();

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

        // LEFT CLICK DOWN
        if (Input.GetMouseButtonDown(0))
        {
            // Calculate and initially synchronize both left click and release position
            Vector3 worldMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            leftClickMousePosition_Click = new Vector3(worldMousePos.x, groundWorldPosition.position.y, worldMousePos.z);
            leftClickMousePosition_Release = leftClickMousePosition_Click;

            // Turn selected circle of past units off
            RenderSelectedCircles(false);

            if(inputMode == InputMode.None)
            {
                // Reset selectables list
                unitList.Clear();
                unitList.TrimExcess();
                structureList.Clear();
                structureList.TrimExcess();
            }
            else if(inputMode == InputMode.PatrolButton)
            {
                PatrolCommand();
            }
            else if(inputMode == InputMode.AttackButton)
            {
                // Check for enemy at mouse when right click
                bool hasSpecificTarget = false;
                GameObject enemy = null;

                Collider[] colliders = Physics.OverlapBox(leftClickMousePosition_Click, Vector3.one + Vector3.up * 100, Quaternion.identity, LayerMask.GetMask(Tags.Selectable));
                foreach (Collider collider in colliders)
                {
                    if (collider.gameObject.GetComponent<EntityInterface>().GetEntityType() == EntityInterface.EntityTypes.SelectableUnit)
                    {
                        if (collider.gameObject.GetComponent<EntityInterface>().GetRelationshipType() == EntityInterface.RelationshipTypes.Enemy)
                        {
                            hasSpecificTarget = true;
                            enemy = collider.gameObject;
                            break;
                        }
                    }
                    else if (collider.gameObject.GetComponent<EntityInterface>().GetEntityType() == EntityInterface.EntityTypes.SelectableStructure)
                    {
                        if (collider.gameObject.GetComponent<EntityInterface>().GetRelationshipType() == EntityInterface.RelationshipTypes.Enemy)
                        {
                            hasSpecificTarget = true;
                            enemy = collider.gameObject;
                            break;
                        }
                    }
                }

                if(hasSpecificTarget == true)
                {
                    // Attack Command with target
                    AttackCommand(enemy);

                    Debug.Log($"Attack {enemy.name}");
                }
                else
                {
                    // Attack Command without target
                    AttackCommand(leftClickMousePosition_Click);

                    Debug.Log($"Attack move towards {leftClickMousePosition_Click}");
                }
            }
            


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
            
            if(inputMode == InputMode.None)
            {
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


            // Check for enemy at mouse when right click
            bool attackEnemy = false;
            GameObject enemy = null;

            Collider[] colliders = Physics.OverlapBox(rightClickMousePosition_Click, Vector3.one + Vector3.up * 100, Quaternion.identity, LayerMask.GetMask(Tags.Selectable));
            foreach (Collider collider in colliders)
            {
                if (collider.gameObject.GetComponent<EntityInterface>().GetEntityType() == EntityInterface.EntityTypes.SelectableUnit)
                {
                    if(collider.gameObject.GetComponent<EntityInterface>().GetRelationshipType() == EntityInterface.RelationshipTypes.Enemy)
                    {
                        attackEnemy = true;
                        enemy = collider.gameObject;
                        break;
                    }
                }
                else if (collider.gameObject.GetComponent<EntityInterface>().GetEntityType() == EntityInterface.EntityTypes.SelectableStructure)
                {
                    if (collider.gameObject.GetComponent<EntityInterface>().GetRelationshipType() == EntityInterface.RelationshipTypes.Enemy)
                    {
                        attackEnemy = true;
                        enemy = collider.gameObject;
                        break;
                    }
                }
            }

            if (attackEnemy)
            {
                // Attack Command
                AttackCommand(enemy);
            }
            else
            {
                // Move Command
                MoveCommand();
            }
            

            //Debug.Log("Right Released at " + rightClickMousePosition_Click);

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

    private void AttackCommand(GameObject _target)
    {
        // Determine to Queue command OR Overwrite past commands by holding SHIFT or not
        bool isInstant = true;
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
            isInstant = false;

        if (_target == null) { return; }

        if (unitList.Count == 0) { return; }

        Vector3 targetPosition = new Vector3(_target.GetComponent<Rigidbody>().position.x, groundWorldPosition.position.y, _target.GetComponent<Rigidbody>().position.z);

        // Create a Select Group object, containing ALL actions to be created and ONE flow field
        GameObject selectGroup = new GameObject("SelectGroup " + transform.childCount);
        selectGroup.transform.SetParent(transform);
        selectGroup.AddComponent<SelectGroup>();
        selectGroup.GetComponent<SelectGroup>().InitializeCurrentCostField();

        // Check if the clicked position is traversible
        if (_target != null)
        {
            Debug.Log($"Attack Enemy: {_target.name}");
            // Initialize the flowfield of select group before using
            selectGroup.GetComponent<SelectGroup>().InitializeCurrentIntegrationField(targetPosition);
            selectGroup.GetComponent<SelectGroup>().InitializeCurrentFlowField();

            // Adding units and actions to refer from
            foreach (Unit curUnit in unitList)
            {
                // Create a unique action to enqueue for each unit
                // Initialize select group for future reference (flow field for navigation)
                UnitAction curMoveAction = new UnitAction(UnitAction.ActionTypes.AttackTarget);
                curMoveAction.InitializeMousePosition(targetPosition);
                curMoveAction.InitializeSelectGroupObject(ref selectGroup);
                curMoveAction.InitializeCurrentFlowField(selectGroup.GetComponent<SelectGroup>().groupFlowField);
                curMoveAction.InitializeCurrentTarget(_target);

                // Enqueue action
                curUnit.gameObject.GetComponent<UnitActionController>().EnqueueAction(ref curMoveAction, isInstant);

                // Add action and unit to SelectGroup for future reference (self-destruct when no longer used by any action and unit)
                selectGroup.GetComponent<SelectGroup>().AddToActionList(curMoveAction);
                selectGroup.GetComponent<SelectGroup>().AddToUnitList(curUnit);
                if(selectGroup.GetComponent<SelectGroup>().groupTarget == null) { selectGroup.GetComponent<SelectGroup>().InitializeGroupTarget(_target); }
            }
        }
        else
        {
            Debug.Log("Cant Attack Target There!");
            Destroy(selectGroup);
        }

    }

    private void AttackCommand(Vector3 _destination)
    {
        // Determine to Queue command OR Overwrite past commands by holding SHIFT or not
        bool isInstant = true;
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
            isInstant = false;


        if (unitList.Count == 0) { return; }

        // Create a Select Group object, containing ALL actions to be created and ONE flow field
        GameObject selectGroup = new GameObject("SelectGroup " + transform.childCount);
        selectGroup.transform.SetParent(transform);
        selectGroup.AddComponent<SelectGroup>();
        selectGroup.GetComponent<SelectGroup>().InitializeCurrentCostField();

        // Check if the clicked position is traversible
        if (selectGroup.GetComponent<SelectGroup>().groupFlowField.GetCellFromWorldPos(_destination).cost < byte.MaxValue)
        {
            Debug.Log($"Attack Move Towards: {_destination}");
            // Initialize the flowfield of select group before using
            selectGroup.GetComponent<SelectGroup>().InitializeCurrentIntegrationField(_destination);
            selectGroup.GetComponent<SelectGroup>().InitializeCurrentFlowField();

            // Adding units and actions to refer from
            foreach (Unit curUnit in unitList)
            {
                // Create a unique action to enqueue for each unit
                // Initialize select group for future reference (flow field for navigation)
                UnitAction curMoveAction = new UnitAction(UnitAction.ActionTypes.AttackMove);
                curMoveAction.InitializeMousePosition(_destination);
                curMoveAction.InitializeSelectGroupObject(ref selectGroup);
                curMoveAction.InitializeSelfFlowField(_destination);
                curMoveAction.InitializeCurrentFlowField(curMoveAction.selfFlowField);

                // Enqueue action
                curUnit.gameObject.GetComponent<UnitActionController>().EnqueueAction(ref curMoveAction, isInstant);

                // Add action and unit to SelectGroup for future reference (self-destruct when no longer used by any action and unit)
                selectGroup.GetComponent<SelectGroup>().AddToActionList(curMoveAction);
                selectGroup.GetComponent<SelectGroup>().AddToUnitList(curUnit);
            }
        }
        else
        {
            Debug.Log("Cant Attack Target There!");
            Destroy(selectGroup);
        }
    }

    private void PatrolCommand()
    {
        // Determine to Queue command OR Overwrite past commands by holding SHIFT or not
        bool isInstant = true;
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
            isInstant = false;


        // Calculate right click position. Also will be the starting point if used in a chain command
        Vector3 worldMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        rightClickMousePosition_Click = new Vector3(worldMousePos.x, groundWorldPosition.position.y, worldMousePos.z);

        if (unitList.Count == 0) { return; }

        // Create a Select Group object, containing ALL actions to be created and ONE flow field
        GameObject selectGroup = new GameObject("SelectGroup " + transform.childCount);
        selectGroup.transform.SetParent(transform);
        selectGroup.AddComponent<SelectGroup>();
        selectGroup.GetComponent<SelectGroup>().InitializeCurrentCostField();

        // Check if the clicked position is traversible
        if (selectGroup.GetComponent<SelectGroup>().groupFlowField.GetCellFromWorldPos(rightClickMousePosition_Click).cost < byte.MaxValue)
        {
            // Initialize the flowfield of select group before using
            selectGroup.GetComponent<SelectGroup>().InitializeCurrentIntegrationField(rightClickMousePosition_Click);
            selectGroup.GetComponent<SelectGroup>().InitializeCurrentFlowField();

            selectGroup.GetComponent<SelectGroup>().AddNewFlowFieldToList(rightClickMousePosition_Click);

            // Adding units and actions to refer from
            foreach (Unit curUnit in unitList)
            {
                Vector3 curUnitPos = curUnit.GetComponent<Rigidbody>().position;
                curUnitPos = new Vector3(curUnitPos.x, groundWorldPosition.position.y, curUnitPos.z);
                // Create a unique action to enqueue for each unit
                // Initialize select group for future reference (flow field for navigation)
                UnitAction curMoveAction = new UnitAction(UnitAction.ActionTypes.Patrol);
                curMoveAction.InitializeMousePosition(rightClickMousePosition_Click);
                curMoveAction.InitializeSelectGroupObject(ref selectGroup);
                curMoveAction.InitializeSelfFlowField(curUnitPos);
                curMoveAction.InitializeCurrentFlowField(selectGroup.GetComponent<SelectGroup>().flowFieldList[0]);


                // Enqueue action
                curUnit.gameObject.GetComponent<UnitActionController>().EnqueueAction(ref curMoveAction, isInstant);

                // Add action and unit to SelectGroup for future reference (self-destruct when no longer used by any action and unit)
                selectGroup.GetComponent<SelectGroup>().AddToActionList(curMoveAction);
                selectGroup.GetComponent<SelectGroup>().AddToUnitList(curUnit);
            }
        }
        else
        {
            Debug.Log("Cant Move There!");
            Destroy(selectGroup);
        }
    }

    /// <summary>
    /// Command the group of units to move towards a destination specified by mouse position
    /// </summary>
    private void MoveCommand()
    {
        // Determine to Queue command OR Overwrite past commands by holding SHIFT or not
        bool isInstant = true;
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
            isInstant = false;


        // Calculate right click position
        Vector3 worldMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        rightClickMousePosition_Click = new Vector3(worldMousePos.x, groundWorldPosition.position.y, worldMousePos.z);

        if (unitList.Count == 0) { return; }

        // Create a Select Group object, containing ALL actions to be created and ONE flow field
        GameObject selectGroup = new GameObject("SelectGroup " + transform.childCount);
        selectGroup.transform.SetParent(transform);
        selectGroup.AddComponent<SelectGroup>();
        selectGroup.GetComponent<SelectGroup>().InitializeCurrentCostField();

        // Check if the clicked position is traversible
        if (selectGroup.GetComponent<SelectGroup>().groupFlowField.GetCellFromWorldPos(rightClickMousePosition_Click).cost < byte.MaxValue)
        {
            // Initialize the flowfield of select group before using
            selectGroup.GetComponent<SelectGroup>().InitializeCurrentIntegrationField(rightClickMousePosition_Click);
            selectGroup.GetComponent<SelectGroup>().InitializeCurrentFlowField();

            // Adding units and actions to refer from
            foreach (Unit curUnit in unitList)
            {
                if(curUnit == null) { continue; }
                // Create a unique action to enqueue for each unit
                // Initialize select group for future reference (flow field for navigation)
                UnitAction curMoveAction = new UnitAction(UnitAction.ActionTypes.MoveTowards);
                curMoveAction.InitializeMousePosition(rightClickMousePosition_Click);
                curMoveAction.InitializeSelectGroupObject(ref selectGroup);
                curMoveAction.InitializeCurrentFlowField(selectGroup.GetComponent<SelectGroup>().groupFlowField);

                // Enqueue action
                curUnit.gameObject.GetComponent<UnitActionController>().EnqueueAction(ref curMoveAction, isInstant);

                // Add action and unit to SelectGroup for future reference (self-destruct when no longer used by any action and unit)
                selectGroup.GetComponent<SelectGroup>().AddToActionList(curMoveAction);
                selectGroup.GetComponent<SelectGroup>().AddToUnitList(curUnit);
            }
        }
        else
        {
            Debug.Log("Cant Move There!");
            Destroy(selectGroup);
        }

        //DEBUG: draw the created flowfield with icons for each cell, indicating path and destination
        GridDebug.SetCurFlowField(selectGroup.GetComponent<SelectGroup>().groupFlowField);
        if (displayGizmos == true)
        {
            GridDebug.DrawFlowField();
        }
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
            if (collider.gameObject.GetComponent<EntityInterface>().GetEntityType() == EntityInterface.EntityTypes.SelectableUnit)
            {
                unitList.Add(collider.gameObject.GetComponent<Unit>());
                //Debug.Log($"Added {collider.gameObject.name}");
            }
            else if(collider.gameObject.GetComponent<EntityInterface>().GetEntityType() == EntityInterface.EntityTypes.SelectableStructure)
            {
                structureList.Add(collider.gameObject.GetComponent<Structure>());
                //Debug.Log($"Added {collider.gameObject.name}");
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

    private void SetInputMode(InputMode mode)
    {
        inputMode = mode;
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
            Debug.Log("Drawing " + unitList[i].gameObject.name);
            Gizmos.color = Color.blue;
            Gizmos.DrawIcon(unitList[i].gameObject.transform.position + Vector3.up * 10, "user icon.png", true);
        }
    }
}

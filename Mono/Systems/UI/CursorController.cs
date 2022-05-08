using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CursorController : MonoBehaviour
{
    [SerializeField]
    private Sprite[] borderSprites;
    [SerializeField]
    private int playerIndex;

    private bool isAtBorder;

    private Image image;
    private Animator animator;


    private void Start()
    {
        image = GetComponent<Image>();
        animator = GetComponent<Animator>();
        Cursor.visible = false;
    }

    private void LateUpdate()
    {
        int x = 0;
        int y = 0;
        if (Input.mousePosition.x > Screen.width - 10)
            x = 1;
        else if (Input.mousePosition.x < 1)
            x = -1;
        else
            x = 0;

        if (Input.mousePosition.y > Screen.height - 10)
            y = 1;
        else if (Input.mousePosition.y <= 1)
            y = -1;
        else
            y = 0;

        if (x == -1 && y == 0)
            SetCursorBorder(0);
        else if(x == -1 && y == 1)
            SetCursorBorder(1);
        else if (x == 0 && y == 1)
            SetCursorBorder(2);
        else if (x == 1 && y == 1)
            SetCursorBorder(3);
        else if (x == 1 && y == 0)
            SetCursorBorder(4);
        else if (x == 1 && y == -1)
            SetCursorBorder(5);
        else if (x == 0 && y == -1)
            SetCursorBorder(6);
        else if (x == -1 && y == -1)
            SetCursorBorder(7);
        else
            isAtBorder = false;

        if(isAtBorder == false)
        {
            RaycastHit hit;
            Physics.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Camera.main.transform.forward, out hit, 1000, LayerMask.GetMask(Tags.Ground));
            IEntity entity = Utilities.ReturnSelectableEntityAtWorldPosition(hit.point);
            if(entity != null)
            {
                if(entity.GetPlayerIndex() == playerIndex)
                {
                    SetCursorMode(1);
                }
                else if(entity.GetPlayerIndex() != playerIndex)
                {
                    SetCursorMode(3);
                }
                else
                {
                    SetCursorMode(2);
                }
            }
            else
            {
                SetCursorMode(0);
            }
        }

    }


    public void SetCursorBorder(int index)
    {
        image.sprite = borderSprites[index];
        isAtBorder = true;
    }

    public void SetCursorMode(int index)
    {
        animator.SetInteger("mode", index);
    }

    public void SetCursorTrigger(string _triggerName)
    {
        animator.SetTrigger(_triggerName);
    }
}

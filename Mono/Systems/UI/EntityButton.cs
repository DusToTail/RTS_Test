using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// English: A class that allows the selection of an individual entity stored as a reference here and the shifting of the camera for focusing when clicked.
/// ���{��F�����ɕۑ����ꂽEntity�̑I���Ƃ���Entity�܂ł̃J�����ړ����ł���{�^�������N���X�B
/// </summary>
public class EntityButton : MonoBehaviour, IButton
{
    public SelectManager selectManager;
    public CommandManager commandManager;
    public IEntity entity { get; private set; }

    private void Awake()
    {
        entity = null;
        GetComponent<Button>().onClick.AddListener(() => OnClick());
    }

    /// <summary>
    /// English: Assign a new entity to the button
    /// ���{��F�V����Entity�����蓖�Ă�B
    /// </summary>
    /// <param name="_entity"></param>
    public void AssignEntity(IEntity _entity)
    {
        entity = _entity;
        GetComponent<Image>().sprite = entity.GetPortrait();

    }

    /// <summary>
    /// English: Clear the current SelectSystem class's current group's lists, add this entity for selection, and shift the camera to its position.
    /// ���{��F���݂�SelectSystem�N���X�̌��ݎg�p���Ă���O���[�v�̃��X�g���N���A���A����Entity�������i�I���j�A�J����������Entity�̈ʒu�܂ňړ�������B
    /// </summary>
    public void OnClick()
    {
        if(entity == null) { return; }
        if(selectManager == null) { return; }

        // Select the entity, move the camera to the entity position
        selectManager.RenderSelectedCircles(false);
        selectManager.ClearLists();
        selectManager.selectableList.Add(entity);

        GameObject selectGroup = new GameObject("Temp SelectGroup");
        selectGroup.transform.SetParent(selectManager.transform);
        selectGroup.AddComponent<SelectGroup>();

        for (int index = 0; index < selectManager.structureList.Count; index++)
        {
            selectGroup.GetComponent<SelectGroup>().entityList.Add(selectManager.structureList[index]);
        }
        for (int index = 0; index < selectManager.unitList.Count; index++)
        {
            selectGroup.GetComponent<SelectGroup>().entityList.Add(selectManager.unitList[index]);
        }

        selectManager.curSelectGroup = selectGroup.GetComponent<SelectGroup>();
        commandManager.SetCommandMode(CommandManager.CommandInt.None);

        selectManager.RenderSelectedCircles(true);

        FindObjectOfType<CameraController>().MoveCameraTo(entity.GetWorldPosition());
    }

}

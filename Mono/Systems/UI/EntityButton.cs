using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// English: A class that allows the selection of an individual entity stored as a reference here and the shifting of the camera for focusing when clicked.
/// ���{��F�����ɕۑ����ꂽEntity�̑I���Ƃ���Entity�܂ł̃J�����ړ����ł���{�^�������N���X�B
/// </summary>
public class EntityButton : MonoBehaviour, IButton
{
    public IEntity entity;
    public SelectSystem selectSystem;
    public CameraControl cameraControl;
    private Sprite image;
    
    /// <summary>
    /// English: Clear the current SelectSystem class's current group's lists, add this entity for selection, and shift the camera to its position.
    /// ���{��F���݂�SelectSystem�N���X�̌��ݎg�p���Ă���O���[�v�̃��X�g���N���A���A����Entity�������i�I���j�A�J����������Entity�̈ʒu�܂ňړ�������B
    /// </summary>
    public void OnClick()
    {
        if(entity == null) { return; }
        if(selectSystem == null) { return; }

        // Select the entity, move the camera to the entity position
        selectSystem.RenderSelectedCircles(false);
        selectSystem.ClearLists();
        selectSystem.selectableList.Add(entity);

        GameObject selectGroup = new GameObject("Temp SelectGroup");
        selectGroup.transform.SetParent(selectSystem.transform);
        selectGroup.AddComponent<SelectGroup>();

        for (int index = 0; index < selectSystem.structureList.Count; index++)
        {
            selectGroup.GetComponent<SelectGroup>().entityList.Add(selectSystem.structureList[index]);
        }
        for (int index = 0; index < selectSystem.unitList.Count; index++)
        {
            selectGroup.GetComponent<SelectGroup>().entityList.Add(selectSystem.unitList[index]);
        }

        selectSystem.curSelectGroup = selectGroup.GetComponent<SelectGroup>();
        selectSystem.SetInputMode(SelectSystem.InputMode.None);

        selectSystem.RenderSelectedCircles(true);

        if (cameraControl == null) { return; }
        cameraControl.MoveCameraTo(entity.GetWorldPosition());
    }

    /// <summary>
    /// English: Return the sprite associated with the entity to be used in UI.
    /// ���{��FUI�Ŏg�p�����Entity�̃|�[�g���[�g��Sprite��Ԃ��B
    /// </summary>
    /// <returns></returns>
    public Sprite GetImage()
    {
        if(entity == null) { return null; }
        return entity.GetPortrait();
    }
}

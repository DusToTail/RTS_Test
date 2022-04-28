using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityButton : MonoBehaviour, IButton
{
    public IEntity entity;
    public SelectSystem selectSystem;
    public CameraControl cameraControl;
    private Sprite image;
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

    public Sprite GetImage()
    {
        if(entity == null) { return null; }
        return entity.GetPortrait();
    }
}

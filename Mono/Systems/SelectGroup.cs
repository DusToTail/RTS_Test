using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// English: A class that groups entities and their actions. Is created, saved, referred by mainly but not limited to SelectSystem class.
/// ���{��F������Entity�ƍs���𓝊�����N���X�B���SelectSystem�N���X�ɍ쐬�A�ۗ��A�Q�Ƃ����B
/// </summary>
/// 
public class SelectGroup : MonoBehaviour
{
    // Potential future changes: Instead of 2 lists, use Map<IEntity, IAction>
    public List<IAction> actionList;
    public List<IEntity> entityList;

    private bool groupUpdate;

    private void Awake()
    {
        actionList = new List<IAction>();
        entityList = new List<IEntity>();

        groupUpdate = false;
    }

    private void Start()
    {
        
    }

    private void Update()
    {
        // Check if all actions is finished to self destroy and save performance.
        // Only apply if it is not saved, and thus named "Temp SelectGroup" and is not the current SelectGroup in SelectSystem
        if(this != FindObjectOfType<SelectManager>().curSelectGroup) 
        {
            if(this.gameObject.name == "Temp SelectGroup")
            {
                if (CheckIfAllActionsIsFinished() == true) { Destroy(gameObject); }
            }
        }

        if (groupUpdate == false) { return; }
        if(entityList.Count == 0) { return; }
        if(actionList.Count == 0) { return; }

        // Check if all actions is finished to self destroy and save performance.
        // For groups that is not current SelectGroup in SelectSystem
        if (CheckIfAllActionsIsFinished() == true) 
        { 
            if(this != FindObjectOfType<SelectManager>().curSelectGroup)
                Destroy(gameObject); 
            else
                ResetSelectGroup();
        }

    }

    /// <summary>
    /// English: Add an action to its action List.
    /// ���{��F�s�����X�g�ɍs����������B
    /// </summary>
    /// <param name="_action"></param>
    public void AddToActionList(IAction _action)
    {
        if(_action == null) { return; }
        actionList.Add(_action);
    }

    /// <summary>
    /// English: Add an entity to its entity List.
    /// ���{��FEntity���X�g��Entity��������B
    /// </summary>
    /// <param name="_entity"></param>
    public void AddToEntityList(IEntity _entity)
    {
        if(_entity == null) { return;}
        entityList.Add(_entity);
    }

    /// <summary>
    /// English: Allow the Update() function to run or not.
    /// ���{��FUpdate�i�j��ʂ����邩�ǂ����B
    /// </summary>
    /// <param name="isTrue"></param>
    public void StartUpdateGroup(bool isTrue) { groupUpdate = isTrue; }

    /// <summary>
    /// English: Return group's action list and Update() function state to default.
    /// ���{��F�O���[�v�̍s�����X�g��Update�i�j�֐��̏�Ԃ�����������B
    /// </summary>
    public void ResetSelectGroup()
    {
        actionList.Clear();
        actionList.TrimExcess();

        groupUpdate = false;
        Debug.Log($"{this.gameObject.name} is Reset");
    }


    /// <summary>
    /// English: false if AT LEAST one action is not finished. else true
    /// ���{��F���Ȃ��Ƃ���̍s���͊�������Ă��Ȃ��ꍇ�Ffalse�B�����ł͂Ȃ��ꍇ�Ftrue
    /// </summary>
    /// <returns></returns>
    private bool CheckIfAllActionsIsFinished()
    {
        for (int index = 0; index < actionList.Count; index++)
        {
            //Debug.Log("Action index " + index + " isFinished: " + actionList[index].isFinished);
            if (actionList[index].isFinished == false)
            {
                return false;
            }
        }
        return true;
    }


    /// <summary>
    /// ���@NOT USED�@��
    /// English: Return true if the front of the unit has a terrain with different cost value compared to its own cost field (meaning changes in the environment). Else false.
    /// ���{��F���j�b�g�̑O�ɂ���n�`�ɂ��̃��j�b�g�̒m���Ɣ�ׂăR�X�g���Ⴄ�ꍇ�Atrue��Ԃ��B�����ł͂Ȃ��ꍇ�Afalse
    /// </summary>
    /// <param name="_unit"></param>
    /// <returns></returns>
    private bool CheckIfUnitCollidesWithObstacle(IUnit _unit, int _unitIndex)
    {
        if (_unit == null) { return false; }
        if(_unit.HealthIsZero()) { return false; }

        Vector3 forwardPosition = _unit.GetWorldPosition() + _unit.GetTransform().forward * 2;
        LayerMask terrainMask = LayerMask.GetMask(Tags.Impassible_Terrain, Tags.Rough_Terrain, Tags.Selectable);
        Vector3 cellHalfExtents = Vector3.one * GameObject.FindObjectOfType<GridController>().cellRadius;

        Cell cell = new Cell(actionList[_unitIndex].GetSelf().curFlowField.GetCellFromWorldPos(forwardPosition).worldPosition, actionList[_unitIndex].GetSelf().curFlowField.GetCellFromWorldPos(forwardPosition).gridPosition);

        Collider[] hits = Physics.OverlapBox(cell.worldPosition, cellHalfExtents, Quaternion.identity, terrainMask);
        foreach (Collider hit in hits)
        {
            if (hit.gameObject.layer == LayerMask.NameToLayer(Tags.Impassible_Terrain))
            {
                cell.IncreaseCost(255);
                break;
            }
            else if (hit.gameObject.layer == LayerMask.NameToLayer(Tags.Rough_Terrain))
            {
                cell.IncreaseCost(3);
                break;
            }
            else if (hit.gameObject.layer == LayerMask.NameToLayer(Tags.Selectable))
            {
                if (hit.gameObject.GetComponent<IEntity>() is IStructure)
                {
                    cell.IncreaseCost(255);
                    break;
                }
            }
        }

        if (cell.cost != actionList[_unitIndex].GetSelf().curFlowField.grid[cell.gridPosition.x, cell.gridPosition.y].cost &&
            cell.gridPosition != actionList[_unitIndex].GetSelf().curFlowField.destinationCell.gridPosition)
        {
            Debug.DrawLine(cell.worldPosition, cell.worldPosition + Vector3.up * 100, Color.yellow);
            return true;
        }

        return false;
    }











}

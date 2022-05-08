using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// English: A class that groups entities and their actions. Is created, saved, referred by mainly but not limited to SelectSystem class.
/// 日本語：複数のEntityと行動を統括するクラス。主にSelectSystemクラスに作成、保留、参照される。
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
    /// 日本語：行動リストに行動を加える。
    /// </summary>
    /// <param name="_action"></param>
    public void AddToActionList(IAction _action)
    {
        if(_action == null) { return; }
        actionList.Add(_action);
    }

    /// <summary>
    /// English: Add an entity to its entity List.
    /// 日本語：EntityリストにEntityを加える。
    /// </summary>
    /// <param name="_entity"></param>
    public void AddToEntityList(IEntity _entity)
    {
        if(_entity == null) { return;}
        entityList.Add(_entity);
    }

    /// <summary>
    /// English: Allow the Update() function to run or not.
    /// 日本語：Update（）を通させるかどうか。
    /// </summary>
    /// <param name="isTrue"></param>
    public void StartUpdateGroup(bool isTrue) { groupUpdate = isTrue; }

    /// <summary>
    /// English: Return group's action list and Update() function state to default.
    /// 日本語：グループの行動リストとUpdate（）関数の状態を初期化する。
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
    /// 日本語：少なくとも一つの行動は完了されていない場合：false。そうではない場合：true
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



}

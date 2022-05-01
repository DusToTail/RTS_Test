using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// English: A class for handling events and inputs from the player alongside with SelectSystem
/// 日本語：イベントやインプットなどをSelectSystemクラスと共に管理するクラス
/// </summary>
public class EventManager : MonoBehaviour
{

    public delegate void SelectGroupAdded(int index);
    public static event SelectGroupAdded OnSelectGroupAdded;

    public delegate void CurrentGroupSelected(List<IEntity> entityList);
    public static event CurrentGroupSelected OnCurrentGroupSelected;
    public delegate void CurrentGroupCleared();
    public static event CurrentGroupCleared OnCurrentGroupCleared;


    public void CallSelectGroupAdded(int index)
    {
        if(OnSelectGroupAdded == null) { return; }
        Debug.Log("Event SelectGroupAdded called");
        OnSelectGroupAdded(index);
    }

    public void CallCurrentGroupSelected(List<IEntity> entityList)
    {
        if(OnCurrentGroupSelected == null) { return; }
        Debug.Log("Event CurrentGroupSelected called");
        OnCurrentGroupSelected(entityList);
    }

    public void CallCurrentGroupCleared()
    {
        if(OnCurrentGroupCleared == null) { return; }
        Debug.Log("Event CurrentGroupCleared called");
        OnCurrentGroupCleared();
    }

}

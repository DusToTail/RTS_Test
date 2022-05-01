using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HoverInfo : MonoBehaviour, IHover
{
    public enum HoverType
    {
        AttackInfo,
        DefenseInfo
    }
    public IEntity entity { get; private set; }

    [SerializeField]
    private HoverType type;

    private void Awake()
    {
        entity = null;
    }

    private void Update()
    {
        
    }

    /// <summary>
    /// English: Assign a new entity to the button
    /// 日本語：新しいEntityを割り当てる。
    /// </summary>
    /// <param name="_entity"></param>
    public void AssignEntity(IEntity _entity)
    {
        entity = _entity;
        // Temporary no image for attack and defense
        GetComponent<Image>().sprite = entity.GetPortrait();
    }

    public void OnHover()
    {
        if(type == HoverType.AttackInfo)
        {

        }
        else if(type == HoverType.DefenseInfo)
        {

        }

    }

}

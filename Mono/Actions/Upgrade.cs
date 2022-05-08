using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// English: a base class for upgrades. Will use events to inform all relevant entities (listeners) to update their stats or unlock abilities.
/// 日本語：アップグレードのベースクラス。イベントで関係のあるEntity（リスナー）が数値を更新したりアビリティを解除したりできるよう発信する。
/// </summary>
public class Upgrade
{
    public string name;
    public string description;
    public Sprite image;


    public enum Type
    {
        Stats,
        Unlock
    }
    public Type type;


}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// English: Interface for hovering information
/// 日本語：情報を表示する用のインターフェース
/// </summary>
public interface IHover
{
    /// <summary>
    /// English: Do something when cursor is hovering over it
    /// 日本語：マウスが上に浮かんでいるとき、何かする
    /// </summary>
    public void OnHover();
}

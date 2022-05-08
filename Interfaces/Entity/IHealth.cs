using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///  English: Interface for health
/// 日本語：体力用のインターフェース
/// </summary>
public interface IHealth
{
    /// <summary>
    /// English: Minus current health for the specified amount. Clamp it at 0
    /// 日本語：現在の体力を指定した量マイナスする。０以下になれば０にする。
    /// </summary>
    /// <param name="_amount"></param>
    public void MinusHealth(float _amount);
    /// <summary>
    /// English: Plus current health for the specified amount. Clamp it at max health
    /// 日本語：現在の体力を指定した量プラスする。体力の最大値以上になれば最大値にする。
    /// </summary>
    /// <param name="_amount"></param>
    public void PlusHealth(float _amount);

    /// <summary>
    /// English: Set _amount as the max health
    /// 日本語：_amountを体力の最大値を設定する
    /// </summary>
    /// <param name="_amount"></param>
    public void SetMaxHealth(float _amount);
    /// <summary>
    /// English: Set _amount as the current health
    /// 日本語：_amountを現在の体力を設定する
    /// </summary>
    /// <param name="_amount"></param>
    public void SetCurrentHealth(float _amount);

    /// <summary>
    /// English: Return max health
    /// 日本語：体力の最大値を返す。
    /// </summary>
    /// <returns></returns>
    public float GetMaxHealth();
    /// <summary>
    /// English: Return current health
    /// 日本語：現在の体力を返す。
    /// </summary>
    /// <returns></returns>
    public float GetCurrentHealth();


    /// <summary>
    /// English: Return true if current health is equal to or below 0. Else return false
    /// 日本語：現在の体力が０以下の場合：trueを返す。そうではない場合、falseを返す
    /// </summary>
    /// <returns></returns>
    public bool HealthIsZero();
}

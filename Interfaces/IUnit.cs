using UnityEngine;

/// <summary>
/// English: Interface for any entity considered as a UNIT
/// 日本語：ユニットとして認知されるEnitity用のインターフェース
/// </summary>
public interface IUnit : IEntity
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
    /// English: Return true if current health is equal to or below 0. Else return false
    /// 日本語：現在の体力が０以下の場合：trueを返す。そうではない場合、falseを返す
    /// </summary>
    /// <returns></returns>
    public bool HealthIsZero();

    /// <summary>
    /// English: Turn on/off the selected circle when selected
    /// 日本語：選ばれた時に、円をオン・オフにする
    /// </summary>
    /// <param name="isOn"></param>
    public void RenderSelectedCircle(bool _isOn);

    /// <summary>
    /// English: Set current health to a specific amount. Clamp at 0 and its max health
    /// 日本語：現在の体力を指定した量にする。０と最大値の間にClampする
    /// </summary>
    /// <param name="_setAmount"></param>
    public void SetCurrentHealth(float _setAmount);
    /// <summary>
    /// English: Set current attack cooldown to the specified time. used in the unit attack action
    /// 日本語：現在の攻撃クールダウンを指定した値にする。攻撃の行動で使用される
    /// </summary>
    /// <param name="_setTime"></param>
    public void SetCurrentAttackCooldown(float _setTime);

    /// <summary>
    /// English: Return true if current attack cooldown is greater than its set cooldown (set in inspector). Else, false
    /// 日本語：現在の攻撃のクールダウンが定めたクールダウン（Inspectorで定める）より大きい場合、trueを返す。そうではない、falseを返す。
    /// </summary>
    /// <returns></returns>
    public bool CanAttack();

    /// <summary>
    /// English: Return max health of the unit
    /// 日本語：ユニットの体力の最大値を返す。
    /// </summary>
    /// <returns></returns>
    public float GetSetHealth();
    /// <summary>
    /// English: Return the movement speed of the unit
    /// 日本語：ユニットの速度を返す。
    /// </summary>
    /// <returns></returns>
    public float GetSetMovementSpeed();
    /// <summary>
    /// English: Return the vision range of the unit
    /// 日本語：ユニットのビジョンの距離を返す
    /// </summary>
    /// <returns></returns>
    public float GetSetVisionRange();
    /// <summary>
    /// English: Return the attack damage of the unit
    /// 日本語：ユニットの攻撃のダメージを返す
    /// </summary>
    /// <returns></returns>
    public float GetSetAttackDamage();
    /// <summary>
    /// English: Return the attack range of the unit
    /// 日本語：ユニットの攻撃の距離を返す
    /// </summary>
    /// <returns></returns>
    public float GetSetAttackRange();
    /// <summary>
    /// English: Return the attack cooldown (attack speed) of the unit
    /// 日本語：ユニットの攻撃のクールダウン（攻撃スピード）を返す
    /// </summary>
    /// <returns></returns>
    public float GetSetAttackCooldown();
    /// <summary>
    /// English: Return the build time of the unit
    /// 日本語：ユニットの生産時間を返す
    /// </summary>
    /// <returns></returns>
    public float GetSetBuildingTime();
    /// <summary>
    /// English: Return the current health of the unit
    /// 日本語：ユニットの現在の体力を返す
    /// </summary>
    /// <returns></returns>
    public float GetCurrentHealth();
    /// <summary>
    /// English: Return the current attack cooldown of the unit
    /// 日本語：ユニットの現在の攻撃のクールダウンを返す
    /// </summary>
    /// <returns></returns>
    public float GetCurrentAttackCooldown();
    /// <summary>
    /// English: Return the selected circle of the unit
    /// 日本語：ユニットの選ばれた時の円の返す
    /// </summary>
    /// <returns></returns>
    public GameObject GetSelectedCircle();
    /// <summary>
    /// English: Return the Rigidbody of the unit
    /// 日本語：ユニットのRigidbodyを返す
    /// </summary>
    /// <returns></returns>
    public Rigidbody GetRigidbody();
    /// <summary>
    /// English: Return the Collider of the unit
    /// 日本語：ユニットのコライダーを返す
    /// </summary>
    /// <returns></returns>
    public Collider GetCollider();

}

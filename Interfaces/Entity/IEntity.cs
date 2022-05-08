using UnityEngine;

/// <summary>
/// English: Base Interface for all entities
/// 日本語：あらゆるEntity用のベースインターフェース
/// </summary>
public interface IEntity
{
    public enum Type
    {
        Structure,
        Unit
    }

    /// <summary>
    /// English: Return true if selectable by mouse. else false
    /// 日本語：マウスで選べられるとtrueを返す。そうでないと、falseを返す
    /// </summary>
    /// <returns></returns>
    public bool IsSelectable();

    /// <summary>
    /// English: Turn on/off the selected circle when selected
    /// 日本語：選ばれた時に、円をオン・オフにする
    /// </summary>
    /// <param name="isOn"></param>
    public void RenderSelectedCircle(bool _isOn);

    /// <summary>
    /// English: Return name
    /// 日本語：名前を返す
    /// </summary>
    /// <returns></returns>
    public string GetName();
    /// <summary>
    /// English: Return description
    /// 日本語：ディスクリプションを返す
    /// </summary>
    /// <returns></returns>
    public string GetDescription();
    /// <summary>
    /// English: Return the sprite for the portrait
    /// 日本語：ポートレートのSpriteを返す
    /// </summary>
    /// <returns></returns>
    public Sprite GetPortrait();
    /// <summary>
    /// English: Return the index of the player this entity belongs to
    /// 日本語：所属するプレイヤーのインデックスを返す。
    /// </summary>
    /// <returns></returns>
    public int GetPlayerIndex();
    /// <summary>
    /// English: Return the type of this entity
    /// 日本語：Entityのタイプを返す。
    /// </summary>
    /// <returns></returns>
    public Type GetEntityType();
    /// <summary>
    /// English: Return the selected circle
    /// 日本語：選ばれた時の円の返す
    /// </summary>
    /// <returns></returns>
    public GameObject GetSelectedCircle();
    /// <summary>
    /// English: Return the selected circle radius
    /// 日本語：選ばれた時の円の半径を返す
    /// </summary>
    /// <returns></returns>
    public float GetSelectedCircleRadius();

    /// <summary>
    /// English: Return the Transform
    /// 日本語：Transformを返す
    /// </summary>
    /// <returns></returns>
    public Transform GetTransform();
    /// <summary>
    /// English: Return the current world position
    /// 日本語：現在のWorld位置を返す
    /// </summary>
    /// <returns></returns>
    public Vector3 GetWorldPosition();

    /// <summary>
    /// English: Return the class implementing this interface
    /// 日本語：このインターフェースを実装するクラスを返す
    /// </summary>
    /// <returns></returns>
    public EntityInfo GetSelf();

}

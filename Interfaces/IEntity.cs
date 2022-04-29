using UnityEngine;

/// <summary>
/// English: Base Interface for all entities
/// 日本語：あらゆるEntity用のベースインターフェース
/// </summary>
public interface IEntity
{
    /// <summary>
    /// English: Either Selectable or UnSelectable
    /// 日本語：選択できるかどうか
    /// </summary>
    public enum SelectionType
    {
        Selectable,
        UnSelectable
    }
    /// <summary>
    /// English: Relationship with other entities in the game: Ally, Enemy, Neutral
    /// 日本語：他のEntityとの関係性：味方、敵、中立
    /// </summary>
    public enum RelationshipType
    {
        Ally,
        Enemy,
        Neutral
    }
    /// <summary>
    /// English：Either Unit or Structure
    /// 日本語：ユニットかビルディングか
    /// </summary>
    public enum EntityType
    {
        Unit,
        Structure
    }

    /// <summary>
    /// English: Return the selection type
    /// 日本語：選択可能のタイプを返す
    /// </summary>
    /// <returns></returns>
    public SelectionType GetSelectionType();
    /// <summary>
    /// English: Return the relationship type
    /// 日本語：関係性を返す
    /// </summary>
    /// <returns></returns>
    public RelationshipType GetRelationshipType();
    /// <summary>
    /// English: Return the entity type
    /// 日本語：Entityのタイプを返す
    /// </summary>
    /// <returns></returns>
    public EntityType GetEntityType();

    /// <summary>
    /// English: Return the sprite for the portrait
    /// 日本語：ポートレートのSpriteを返す
    /// </summary>
    /// <returns></returns>
    public Sprite GetPortrait();
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
    /// English: Return self (run-time type)
    /// 日本語：自分を返す(run-timeタイプ）
    /// </summary>
    /// <returns></returns>
    public dynamic GetSelf();
    /// <summary>
    /// English: Return its own (Unit/Structure)ActionController component (run-time type)
    /// 日本語：(Unit/Structure)の行動管理コンポーネントを返す（run-time タイプ）
    /// </summary>
    /// <returns></returns>
    public dynamic GetActionController();
    /// <summary>
    /// English: Return the type of itself (run-time type)
    /// 日本語：自分のタイプを返す（run-time タイプ）
    /// </summary>
    /// <returns></returns>
    public dynamic ReturnSelfType();
    /// <summary>
    /// English: Return a new empty action instance of depending on its info class (run-time type)
    /// 日本語：自分のクラスに応じる新しく、空の行動のインスタンスを返す（run-time タイプ）
    /// </summary>
    /// <returns></returns>
    public dynamic ReturnNewAction();
    /// <summary>
    /// English: Return the type of its action class depending on its info class (run-time type)
    /// 日本語：自分のクラスに応じる行動のクラスのタイプを返す（run-time タイプ）
    /// </summary>
    /// <returns></returns>
    public dynamic ReturnActionType();

}

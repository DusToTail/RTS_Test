using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// English; A class that handles universal and helpful static functions
/// 日本語：汎用的で便利で静的な関数をもつクラス
/// </summary>
public class Utilities
{

    /// <summary>
    /// English: Return an entity with specified relationship type at the world position.
    /// 日本語：ワールド位置にあり、指定した関係性を持つEntityを返す。
    /// </summary>
    /// <param name="_relationshipType"></param>
    /// <param name="_worldPosition"></param>
    /// <returns></returns>
    public static IEntity ReturnEntityAtWorldPosition(IEntity.RelationshipType _relationshipType, Vector3 _worldPosition)
    {
        IEntity enemy = null;

        Collider[] colliders = Physics.OverlapBox(_worldPosition, Vector3.one + Vector3.up * 100, Quaternion.identity, LayerMask.GetMask(Tags.Selectable));
        foreach (Collider collider in colliders)
        {
            if (collider.gameObject.GetComponent<IEntity>().GetSelectionType() == IEntity.SelectionType.Selectable)
            {
                if (collider.gameObject.GetComponent<IEntity>().GetRelationshipType() == _relationshipType)
                {
                    enemy = collider.gameObject.GetComponent<IEntity>();
                    break;
                }
            }
        }

        return enemy;
    }

    /// <summary>
    /// English: Return an entity with specified entity type at the world position.
    /// 日本語：ワールド位置にあり、指定したentityタイプを持つEntityを返す。
    /// </summary>
    /// <param name="_entityType"></param>
    /// <param name="_worldPosition"></param>
    /// <returns></returns>
    public static IEntity ReturnEntityAtWorldPosition(IEntity.EntityType _entityType, Vector3 _worldPosition)
    {
        IEntity entity = null;
        Collider[] colliders = Physics.OverlapBox(_worldPosition, Vector3.one + Vector3.up * 100, Quaternion.identity, LayerMask.GetMask(Tags.Selectable));
        foreach (Collider collider in colliders)
        {
            if (collider.gameObject.GetComponent<IEntity>().GetSelectionType() == IEntity.SelectionType.Selectable)
            {
                Debug.Log("Structure found");
                if (collider.gameObject.GetComponent<IEntity>().GetEntityType() == _entityType)
                {
                    entity = collider.gameObject.GetComponent<IEntity>();
                    break;
                }
            }
        }

        return entity;
    }

    /// <summary>
    /// English: Return an entity at the world position.
    /// 日本語：マウスの位置にあるEntityを返す。
    /// </summary>
    /// <param name="_worldPosition"></param>
    /// <returns></returns>
    public static IEntity ReturnEntityAtWorldPosition(Vector3 _worldPosition)
    {
        IEntity entity = null;
        Collider[] colliders = Physics.OverlapBox(_worldPosition, Vector3.one + Vector3.up * 100, Quaternion.identity, LayerMask.GetMask(Tags.Selectable));
        foreach (Collider collider in colliders)
        {
            if (collider.gameObject.GetComponent<IEntity>().GetSelectionType() == IEntity.SelectionType.Selectable)
            {
                entity = collider.gameObject.GetComponent<IEntity>();
                break;
            }
        }

        return entity;
    }

    /// <summary>
    /// English: Return an obstacle at world position.
    /// 日本語：ワールド位置にある障害物を返す。
    /// </summary>
    /// <param name="_clickMousePosition"></param>
    /// <returns></returns>
    public static IObstacle ReturnObstacleAtWorldPosition(Vector3 _worldPosition)
    {
        IObstacle obstacle = null;
        Collider[] colliders = Physics.OverlapBox(_worldPosition, Vector3.one + Vector3.up * 100, Quaternion.identity, LayerMask.GetMask(Tags.Obstacle));
        foreach (Collider collider in colliders)
        {
            obstacle = collider.gameObject.GetComponent<IObstacle>();
            Debug.Log(collider.gameObject.name);
            break;
        }

        return obstacle;
    }

}

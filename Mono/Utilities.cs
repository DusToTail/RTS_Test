using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// English; A class that handles universal and helpful static functions
/// ���{��F�ėp�I�ŕ֗��ŐÓI�Ȋ֐������N���X
/// </summary>
public class Utilities
{
    /// <summary>
    /// English: Return an entity at the world position.
    /// ���{��F�}�E�X�̈ʒu�ɂ���Entity��Ԃ��B
    /// </summary>
    /// <param name="_worldPosition"></param>
    /// <returns></returns>
    public static IEntity ReturnSelectableEntityAtWorldPosition(Vector3 _worldPosition)
    {
        IEntity entity = null;
        Collider[] colliders = Physics.OverlapBox(_worldPosition, Vector3.one + Vector3.up * 100, Quaternion.identity, LayerMask.GetMask(Tags.Selectable));
        foreach (Collider collider in colliders)
        {
            entity = collider.gameObject.GetComponent<IEntity>();
            break;
        }

        return entity;
    }

    /// <summary>
    /// English: Return an obstacle at world position.
    /// ���{��F���[���h�ʒu�ɂ����Q����Ԃ��B
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

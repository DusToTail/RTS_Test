using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// English: Interface for obstacles
/// ���{��F��Q���p�̃C���^�[�t�F�[�X
/// </summary>
public interface IObstacle
{
    /// <summary>
    /// English: Return a Vector2Int that represents the size of a x-y grid, covered by the obstacle
    /// ���{��F��Q������߂�O���b�h�̃T�C�Y��\��Vector2Int�̖߂�l��Ԃ��B
    /// </summary>
    /// <returns></returns>
    public Vector2Int GetObstacleGridSize();
    /// <summary>
    /// English; Return world position
    /// ���{��F���[���h�ʒu��Ԃ�
    /// </summary>
    /// <returns></returns>
    public Vector3 GetWorldPosition();
}

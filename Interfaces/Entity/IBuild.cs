using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// English: Interface for building units actions
/// ���{��F���Y�s���p�̃C���^�[�t�F�[�X
/// </summary>
public interface IBuild : IProgress
{
    public GameObject unit { get; set; }

    public Vector3 spawnPosition { get; set; }
    public Vector3 rallyPosition { get; set; }

    /// <summary>
    /// English: Set a prefab unit to the action.
    /// ���{��F�s���̃��j�b�g�v���n�u�ϐ������蓖�Ă�B
    /// </summary>
    /// <param name="_unit"></param>
    public void SetUnitPrefab(GameObject _unit);
    /// <summary>
    /// English: Set spawn position to the action.
    /// ���{��F�s���̃X�|�[���n�_�ϐ������蓖�Ă�B
    /// </summary>
    /// <param name="_spawnPosition"></param>public void AssignSpawnPosition(Vector3 _spawnPosition);
    public void SetSpawnPosition(Vector3 _spawnPosition);
    /// <summary>
    /// English: Set rally position to the action.
    /// ���{��F�s���̃����[�ʒu�ϐ������蓖�Ă�B
    /// </summary>
    /// <param name="_rallyPosition"></param>
    public void SetRallyPosition(Vector3 _rallyPosition);
}

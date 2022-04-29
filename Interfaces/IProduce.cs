using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// English: Interface for producing actions
/// ���{��F���Y�s���p�̃C���^�[�t�F�[�X
/// </summary>
public interface IProduce : IOperating
{
    public GameObject prefabUnit { get; set; }

    public Vector3 spawnPosition { get; set; }
    public Vector3 rallyPosition { get; set; }
    public List<GameObject> productionList { get; set; }
    /// <summary>
    /// English: Update the current progress time. Spawn a prefab unit at the spawn position and command it to move towards the rally point when finished.
    /// ���{��F�i���̎��Ԃ��X�V����B����������A�X�|�[���n�_�Ƀv���n�u�̃C���X�^���X���쐬���A���̃C���X�^���X�������[�ꏊ�ւ̈ړ��R�}���h�𖽂���B
    /// </summary>
    public void SpawnUnit();
    /// <summary>
    /// English: Return current unit build time
    /// ���{��F���ݐ��Y���Ă��郆�j�b�g�̐��Y���Ԃ�Ԃ�
    /// </summary>
    /// <returns></returns>
    public float GetUnitBuildTime();
    /// <summary>
    /// English: Return the current list of unit/upgrade being built/upgraded
    /// ���{��F���ݐ��Y����Ă��郆�j�b�g��A�b�v�O���[�h����Ă���A�b�v�O���[�h�̃��X�g��Ԃ�
    /// </summary>
    /// <returns></returns>
    public List<GameObject> GetProductionList();
    /// <summary>
    /// English: Assign a prefab unit to the action.
    /// ���{��F�s���̃��j�b�g�v���n�u�ϐ������蓖�Ă�B
    /// </summary>
    /// <param name="_prefab"></param>
    public void AssignUnit(GameObject _unit);
    /// <summary>
    /// English: Assign spawn position to the action.
    /// ���{��F�s���̃X�|�[���n�_�ϐ������蓖�Ă�B
    /// </summary>
    /// <param name="_spawnPosition"></param>public void AssignSpawnPosition(Vector3 _spawnPosition);
    public void AssignSpawnPosition(Vector3 _spawnPosition);
    /// <summary>
    /// English: Assign rally position to the action.
    /// ���{��F�s���̃����[�ʒu�ϐ������蓖�Ă�B
    /// </summary>
    /// <param name="_rallyPosition"></param>
    public void AssignRallyPosition(Vector3 _rallyPosition);
}

using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// English: Interface for building units actions
/// 日本語：生産行動用のインターフェース
/// </summary>
public interface IBuild : IProgress
{
    public GameObject unit { get; set; }

    public Vector3 spawnPosition { get; set; }
    public Vector3 rallyPosition { get; set; }

    /// <summary>
    /// English: Set a prefab unit to the action.
    /// 日本語：行動のユニットプリハブ変数を割り当てる。
    /// </summary>
    /// <param name="_unit"></param>
    public void SetUnitPrefab(GameObject _unit);
    /// <summary>
    /// English: Set spawn position to the action.
    /// 日本語：行動のスポーン地点変数を割り当てる。
    /// </summary>
    /// <param name="_spawnPosition"></param>public void AssignSpawnPosition(Vector3 _spawnPosition);
    public void SetSpawnPosition(Vector3 _spawnPosition);
    /// <summary>
    /// English: Set rally position to the action.
    /// 日本語：行動のラリー位置変数を割り当てる。
    /// </summary>
    /// <param name="_rallyPosition"></param>
    public void SetRallyPosition(Vector3 _rallyPosition);
}

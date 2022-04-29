using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// English: Interface for producing actions
/// 日本語：生産行動用のインターフェース
/// </summary>
public interface IProduce : IOperating
{
    public GameObject prefabUnit { get; set; }

    public Vector3 spawnPosition { get; set; }
    public Vector3 rallyPosition { get; set; }
    public List<GameObject> productionList { get; set; }
    /// <summary>
    /// English: Update the current progress time. Spawn a prefab unit at the spawn position and command it to move towards the rally point when finished.
    /// 日本語：進捗の時間を更新する。完了したら、スポーン地点にプリハブのインスタンスを作成し、そのインスタンスをラリー場所への移動コマンドを命じる。
    /// </summary>
    public void SpawnUnit();
    /// <summary>
    /// English: Return current unit build time
    /// 日本語：現在生産しているユニットの生産時間を返す
    /// </summary>
    /// <returns></returns>
    public float GetUnitBuildTime();
    /// <summary>
    /// English: Return the current list of unit/upgrade being built/upgraded
    /// 日本語：現在生産されているユニットやアップグレードされているアップグレードのリストを返す
    /// </summary>
    /// <returns></returns>
    public List<GameObject> GetProductionList();
    /// <summary>
    /// English: Assign a prefab unit to the action.
    /// 日本語：行動のユニットプリハブ変数を割り当てる。
    /// </summary>
    /// <param name="_prefab"></param>
    public void AssignUnit(GameObject _unit);
    /// <summary>
    /// English: Assign spawn position to the action.
    /// 日本語：行動のスポーン地点変数を割り当てる。
    /// </summary>
    /// <param name="_spawnPosition"></param>public void AssignSpawnPosition(Vector3 _spawnPosition);
    public void AssignSpawnPosition(Vector3 _spawnPosition);
    /// <summary>
    /// English: Assign rally position to the action.
    /// 日本語：行動のラリー位置変数を割り当てる。
    /// </summary>
    /// <param name="_rallyPosition"></param>
    public void AssignRallyPosition(Vector3 _rallyPosition);
}

using UnityEngine;

/// <summary>
/// Interface for upgrading action
/// </summary>
public interface IUpgrade : IOperating
{
    public Upgrade prefabUpgrade { get; set; }
    /// <summary>
    /// English: Update the current progress time. Send an event to all listener about the upgrade when finished.
    /// 日本語：進捗の時間を更新する。完了したら、イベントを発信する。
    /// </summary>
    public void Upgrade();

    public float GetUpgradeTime();
    /// <summary>
    /// English: Assign an upgrade to the action.
    /// 日本語：行動のアップグレード変数を割り当てる。
    /// </summary>
    /// <param name="_prefab"></param>
    public void AssignUpgrade(Upgrade _upgrade);
}

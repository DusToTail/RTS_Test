using UnityEngine;

/// <summary>
/// English: Interface for upgrading action
/// 日本語：アップグレード行動のインターフェース
/// </summary>
public interface IUpgrade : IProgress
{
    public Upgrade upgrade { get; set; }
    /// <summary>
    /// English: Update the current progress time. Send an event to all listener about the upgrade when finished.
    /// 日本語：進捗の時間を更新する。完了したら、イベントを発信する。
    /// </summary>
    public void UpdateProgress();

    /// <summary>
    /// English: Set a prefab upgrade to the action.
    /// 日本語：行動のアップグレードの変数を割り当てる。
    /// </summary>
    /// <param name="_upgrade"></param>
    public void SetUpgrade(Upgrade _upgrade);
}

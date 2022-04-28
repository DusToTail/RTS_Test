using UnityEngine;

public interface IUpgrade : IOperating
{
    public Upgrade prefabUpgrade { get; set; }

    public void Upgrade();

    public float GetUpgradeTime();

    public void AssignUpgrade(Upgrade _upgrade);
}

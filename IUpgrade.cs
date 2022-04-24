using UnityEngine;

public interface IUpgrade : IOperating
{
    public GameObject prefabUpgrade { get; set; }

    public void Upgrade();

    public float GetUpgradeTime();

    public void AssignUpgrade(GameObject _upgrade);
}

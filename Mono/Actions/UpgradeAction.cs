using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeAction : IUpgrade
{
    public Upgrade upgrade { get; set; }
    public float maxProgressTime { get; set; }
    public float curProgressTime { get; set; }
    public bool isFinished { get; set; }

    private SelectGroup group;

    public UpgradeAction(SelectGroup _group, Upgrade _upgrade, float _maxProgressTime)
    {
        upgrade = _upgrade;
        maxProgressTime = _maxProgressTime;

        group = _group;

        curProgressTime = 0;
        isFinished = false;
    }

    public void Execute()
    {
    }

    public void UpdateProgress()
    {
    }

    public void AddProgress(float _amount)
    {
        curProgressTime += _amount;
    }

    public void SetUpgrade(Upgrade _upgrade)
    {
        upgrade = _upgrade;
    }

    public void Stop()
    {
        isFinished = true;
    }

    
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildAction : IBuild
{
    public GameObject unit { get; set; }
    public Vector3 spawnPosition { get; set; }
    public Vector3 rallyPosition { get; set; }
    public float maxProgressTime { get; set; }
    public float curProgressTime { get; set; }
    public bool isFinished { get; set; }

    private SelectGroup group;

    public BuildAction(SelectGroup _group, GameObject _prefab, Vector3 _spawnPosition, Vector3 _rallyPosition, float _maxProgressTime)
    {
        unit = _prefab;
        spawnPosition = _spawnPosition;
        rallyPosition = _rallyPosition;
        maxProgressTime = _maxProgressTime;

        group = _group;

        curProgressTime = 0;
        isFinished = false;
    }

    public void Execute()
    {
    }

    public void AddProgress(float _amount)
    {
        curProgressTime += _amount;
    }

    public void SetRallyPosition(Vector3 _rallyPosition)
    {
        rallyPosition = _rallyPosition;
    }

    public void SetSpawnPosition(Vector3 _spawnPosition)
    {
        spawnPosition = _spawnPosition;
    }

    public void SetUnitPrefab(GameObject _unit)
    {
        unit = _unit;
    }

    public void Stop()
    {
        isFinished = true;
    }
}

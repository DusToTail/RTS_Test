using System.Collections.Generic;
using UnityEngine;

public interface IProduce : IOperating
{
    public GameObject prefabUnit { get; set; }

    public Vector3 spawnPosition { get; set; }
    public Vector3 rallyPosition { get; set; }
    public List<GameObject> productionList { get; set; }

    public void SpawnUnit();

    public float GetUnitBuildTime();
    public List<GameObject> GetProductionList();

    public void AssignUnit(GameObject _unit);
    public void AssignSpawnPosition(Vector3 _spawnPosition);
    public void AssignRallyPosition(Vector3 _rallyPosition);
}

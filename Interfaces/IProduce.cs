using UnityEngine;

public interface IProduce : IOperating
{
    public GameObject prefabUnit { get; set; }

    public Vector3 spawnPosition { get; set; }
    public Vector3 rallyPosition { get; set; }

    public void SpawnUnit();

    public float GetUnitBuildTime();

    public void AssignUnit(GameObject _unit);
    public void AssignSpawnPosition(Vector3 _spawnPosition);
    public void AssignRallyPosition(Vector3 _rallyPosition);
}

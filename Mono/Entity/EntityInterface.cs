using UnityEngine;

public interface EntityInterface
{
    public enum EntityTypes
    {
        SelectableUnit,
        UnselectableUnit,
        SelectableStructure,
        UnselectableStructure
    }

    public enum RelationshipTypes
    {
        Ally,
        Enemy,
        Neutral
    }


    public void DisplayPosition();
    public void DisplayFootPosition();
    public void DisplaySize();
    public EntityTypes GetEntityType();
    public RelationshipTypes GetRelationshipType();
    public GameObject GetGameObject();
}

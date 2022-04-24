using UnityEngine;

public interface IEntity
{
    public enum SelectionType
    {
        Selectable,
        UnSelectable
    }

    public enum RelationshipType
    {
        Ally,
        Enemy,
        Neutral
    }

    public enum EntityType
    {
        Unit,
        Structure
    }


    public SelectionType GetSelectionType();
    public RelationshipType GetRelationshipType();
    public Sprite GetPortrait();

    public EntityType GetEntityType();

    public Transform GetTransform();
    public Vector3 GetWorldPosition();

    public dynamic GetSelf();

}

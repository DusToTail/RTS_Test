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

    public void DisplayPosition();
    public void DisplayFootPosition();
    public void DisplaySize();
}

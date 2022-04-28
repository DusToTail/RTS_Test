using UnityEngine;

public interface IMarineAction : IMove, IAttack
{
    public MarineAction.Type type { get; set; }
}

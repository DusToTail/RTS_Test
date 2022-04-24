using UnityEngine;

public interface IMarineAction : IMove, IAttack
{
    public enum Type
    {
        MoveTowards,
        Patrol,
        AttackTarget,
        AttackMove,
        StopMoving,
        Stop,
        None
    }

    public Type type { get; set; }
}

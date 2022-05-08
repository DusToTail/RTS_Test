using UnityEngine;

/// <summary>
/// English: Interface for attacking
/// ���{��F�U���̍s���p�̃C���^�[�t�F�[�X
/// </summary>
public interface IAttack : IAction
{
    public IEntity target { get; set; }

    /// <summary>
    /// English: Attack if within attack range
    /// ���{��F�͈͓��̏ꍇ�F�U������
    /// </summary>
    /// <param name="_rb"></param>
    /// <param name="_movementSpeed"></param>
    /// <param name="_attackDamage"></param>
    /// <param name="_attackRange"></param>
    /// <param name="_visionRange"></param>
    /// <param name="_canAttack"></param>
    public void AttackTarget(Vector3 _curPosition, float _attackDamage, float _attackRange, float _visionRange);
    
    /// <summary>
    /// English: Return the closest enemy basing on the unit vision by Physics.OverlapCapsule(). Return null if no enemy is detected.
    /// ���{��FPhysics.OverlapCapsule()�Ǝ��E�̋����ōł��߂��G��Ԃ��B�G���Ȃ���΁Anull��Ԃ��B
    /// </summary>
    /// <param name="_rb"></param>
    /// <param name="_visionRange"></param>
    /// <returns></returns>
    public IEntity ReturnNearbyEnemy(Vector3 _curPosition, float _visionRange);
    

    /// <summary>
    /// English: Assign a target for Attack Action
    /// ���{��F�U���̍s���̃^�[�Q�b�g�����蓖�Ă�B
    /// </summary>
    /// <param name="_target"></param>
    public void SetTarget(IEntity _target);

    

    

}

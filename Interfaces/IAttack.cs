using UnityEngine;

/// <summary>
/// English: Interface for attacking. Involve Movement (for chasing)
/// ���{��F�U���̍s���p�̃C���^�[�t�F�[�X�B�ړ�������i�ǋy�ł��邽�߁j
/// </summary>
public interface IAttack : IMove
{
    public IEntity curTarget { get; set; }
    public Vector3 attackMousePosition { get; set; }
    /// <summary>
    /// English: Move towards the target if outside attack range. Attack if within attack range
    /// ���{��F�U���͈͊O�̏ꍇ�F�^�[�Q�b�g�ɋ߂Â��B�͈͓��̏ꍇ�F�U������
    /// </summary>
    /// <param name="_rb"></param>
    /// <param name="_movementSpeed"></param>
    /// <param name="_attackDamage"></param>
    /// <param name="_attackRange"></param>
    /// <param name="_visionRange"></param>
    /// <param name="_canAttack"></param>
    public void AttackTarget(Rigidbody _rb, float _movementSpeed, float _attackDamage, float _attackRange, float _visionRange, bool _canAttack);
    /// <summary>
    /// English: Attack any nearby enemy whilst moving towards the specified position. Finish if destination is reached and there is no enemy nearby
    /// ���{��F���ӂ̓G���U�����Ȃ���A�w�肵���ʒu�܂ňړ�����B�ړI�n�ɓ������A���ӂɓG�����Ȃ��ꍇ�F��������B
    /// </summary>
    /// <param name="_rb"></param>
    /// <param name="_movementSpeed"></param>
    /// <param name="_attackDamage"></param>
    /// <param name="_attackRange"></param>
    /// <param name="_visionRange"></param>
    /// <param name="_canAttack"></param>
    public void AttackMove(Rigidbody _rb, float _movementSpeed, float _attackDamage, float _attackRange, float _visionRange, bool _canAttack);
    /// <summary>
    /// English: Return the closest enemy basing on the unit vision by Physics.OverlapCapsule(). Return null if no enemy is detected.
    /// ���{��FPhysics.OverlapCapsule()�Ǝ��E�̋����ōł��߂��G��Ԃ��B�G���Ȃ���΁Anull��Ԃ��B
    /// </summary>
    /// <param name="_rb"></param>
    /// <param name="_visionRange"></param>
    /// <returns></returns>
    public IEntity ReturnNearbyEnemy(Rigidbody _rb, float _visionRange);

    /// <summary>
    /// English: Assign a target for Attack Action
    /// ���{��F�U���̍s���̃^�[�Q�b�g�����蓖�Ă�B
    /// </summary>
    /// <param name="_target"></param>
    public void AssignTarget(IEntity _target);

}

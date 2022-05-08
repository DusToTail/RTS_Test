using UnityEngine;

/// <summary>
/// English: Interface for attacking. Involve Movement (for chasing)
/// ���{��F�U���̍s���p�̃C���^�[�t�F�[�X�B�ړ�������i�ǋy�ł��邽�߁j
/// </summary>
public interface IAttackMove : IMove, IAttack
{
    public Vector3 attackPosition { get; set; }

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
    public void AttackMove(Rigidbody _rb, float _movementSpeed, float _attackDamage, float _attackRange, float _visionRange);

}

using UnityEngine;

/// <summary>
/// English: Interface for any entity considered as a UNIT
/// ���{��F���j�b�g�Ƃ��ĔF�m�����Enitity�p�̃C���^�[�t�F�[�X
/// </summary>
public interface IUnit : IEntity
{
    /// <summary>
    /// English: Minus current health for the specified amount. Clamp it at 0
    /// ���{��F���݂̗̑͂��w�肵���ʃ}�C�i�X����B�O�ȉ��ɂȂ�΂O�ɂ���B
    /// </summary>
    /// <param name="_amount"></param>
    public void MinusHealth(float _amount);
    /// <summary>
    /// English: Plus current health for the specified amount. Clamp it at max health
    /// ���{��F���݂̗̑͂��w�肵���ʃv���X����B�̗͂̍ő�l�ȏ�ɂȂ�΍ő�l�ɂ���B
    /// </summary>
    /// <param name="_amount"></param>
    public void PlusHealth(float _amount);

    /// <summary>
    /// English: Return true if current health is equal to or below 0. Else return false
    /// ���{��F���݂̗̑͂��O�ȉ��̏ꍇ�Ftrue��Ԃ��B�����ł͂Ȃ��ꍇ�Afalse��Ԃ�
    /// </summary>
    /// <returns></returns>
    public bool HealthIsZero();

    /// <summary>
    /// English: Turn on/off the selected circle when selected
    /// ���{��F�I�΂ꂽ���ɁA�~���I���E�I�t�ɂ���
    /// </summary>
    /// <param name="isOn"></param>
    public void RenderSelectedCircle(bool _isOn);

    /// <summary>
    /// English: Set current health to a specific amount. Clamp at 0 and its max health
    /// ���{��F���݂̗̑͂��w�肵���ʂɂ���B�O�ƍő�l�̊Ԃ�Clamp����
    /// </summary>
    /// <param name="_setAmount"></param>
    public void SetCurrentHealth(float _setAmount);
    /// <summary>
    /// English: Set current attack cooldown to the specified time. used in the unit attack action
    /// ���{��F���݂̍U���N�[���_�E�����w�肵���l�ɂ���B�U���̍s���Ŏg�p�����
    /// </summary>
    /// <param name="_setTime"></param>
    public void SetCurrentAttackCooldown(float _setTime);

    /// <summary>
    /// English: Return true if current attack cooldown is greater than its set cooldown (set in inspector). Else, false
    /// ���{��F���݂̍U���̃N�[���_�E������߂��N�[���_�E���iInspector�Œ�߂�j���傫���ꍇ�Atrue��Ԃ��B�����ł͂Ȃ��Afalse��Ԃ��B
    /// </summary>
    /// <returns></returns>
    public bool CanAttack();

    /// <summary>
    /// English: Return max health of the unit
    /// ���{��F���j�b�g�̗̑͂̍ő�l��Ԃ��B
    /// </summary>
    /// <returns></returns>
    public float GetSetHealth();
    /// <summary>
    /// English: Return the movement speed of the unit
    /// ���{��F���j�b�g�̑��x��Ԃ��B
    /// </summary>
    /// <returns></returns>
    public float GetSetMovementSpeed();
    /// <summary>
    /// English: Return the vision range of the unit
    /// ���{��F���j�b�g�̃r�W�����̋�����Ԃ�
    /// </summary>
    /// <returns></returns>
    public float GetSetVisionRange();
    /// <summary>
    /// English: Return the attack damage of the unit
    /// ���{��F���j�b�g�̍U���̃_���[�W��Ԃ�
    /// </summary>
    /// <returns></returns>
    public float GetSetAttackDamage();
    /// <summary>
    /// English: Return the attack range of the unit
    /// ���{��F���j�b�g�̍U���̋�����Ԃ�
    /// </summary>
    /// <returns></returns>
    public float GetSetAttackRange();
    /// <summary>
    /// English: Return the attack cooldown (attack speed) of the unit
    /// ���{��F���j�b�g�̍U���̃N�[���_�E���i�U���X�s�[�h�j��Ԃ�
    /// </summary>
    /// <returns></returns>
    public float GetSetAttackCooldown();
    /// <summary>
    /// English: Return the build time of the unit
    /// ���{��F���j�b�g�̐��Y���Ԃ�Ԃ�
    /// </summary>
    /// <returns></returns>
    public float GetSetBuildingTime();
    /// <summary>
    /// English: Return the current health of the unit
    /// ���{��F���j�b�g�̌��݂̗̑͂�Ԃ�
    /// </summary>
    /// <returns></returns>
    public float GetCurrentHealth();
    /// <summary>
    /// English: Return the current attack cooldown of the unit
    /// ���{��F���j�b�g�̌��݂̍U���̃N�[���_�E����Ԃ�
    /// </summary>
    /// <returns></returns>
    public float GetCurrentAttackCooldown();
    /// <summary>
    /// English: Return the selected circle of the unit
    /// ���{��F���j�b�g�̑I�΂ꂽ���̉~�̕Ԃ�
    /// </summary>
    /// <returns></returns>
    public GameObject GetSelectedCircle();
    /// <summary>
    /// English: Return the Rigidbody of the unit
    /// ���{��F���j�b�g��Rigidbody��Ԃ�
    /// </summary>
    /// <returns></returns>
    public Rigidbody GetRigidbody();
    /// <summary>
    /// English: Return the Collider of the unit
    /// ���{��F���j�b�g�̃R���C�_�[��Ԃ�
    /// </summary>
    /// <returns></returns>
    public Collider GetCollider();

}

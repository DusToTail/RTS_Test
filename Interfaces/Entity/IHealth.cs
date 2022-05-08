using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///  English: Interface for health
/// ���{��F�̗͗p�̃C���^�[�t�F�[�X
/// </summary>
public interface IHealth
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
    /// English: Set _amount as the max health
    /// ���{��F_amount��̗͂̍ő�l��ݒ肷��
    /// </summary>
    /// <param name="_amount"></param>
    public void SetMaxHealth(float _amount);
    /// <summary>
    /// English: Set _amount as the current health
    /// ���{��F_amount�����݂̗̑͂�ݒ肷��
    /// </summary>
    /// <param name="_amount"></param>
    public void SetCurrentHealth(float _amount);

    /// <summary>
    /// English: Return max health
    /// ���{��F�̗͂̍ő�l��Ԃ��B
    /// </summary>
    /// <returns></returns>
    public float GetMaxHealth();
    /// <summary>
    /// English: Return current health
    /// ���{��F���݂̗̑͂�Ԃ��B
    /// </summary>
    /// <returns></returns>
    public float GetCurrentHealth();


    /// <summary>
    /// English: Return true if current health is equal to or below 0. Else return false
    /// ���{��F���݂̗̑͂��O�ȉ��̏ꍇ�Ftrue��Ԃ��B�����ł͂Ȃ��ꍇ�Afalse��Ԃ�
    /// </summary>
    /// <returns></returns>
    public bool HealthIsZero();
}

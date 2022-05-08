using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// English: A class for managing health.
/// 日本語：体力を管理するクラス。
/// </summary>
public class Health : MonoBehaviour, IHealth
{
    public float maxHealth;
    private float curHealth;

    private void Start()
    {
        curHealth = maxHealth;
    }

    private void Update()
    {
        if (HealthIsZero()) { Destroy(this.gameObject); }
    }

    public float GetCurrentHealth()
    {
        return curHealth;
    }

    public float GetMaxHealth()
    {
        return maxHealth;
    }

    public bool HealthIsZero()
    {
        return curHealth <= 0;
    }

    public void MinusHealth(float _amount)
    {
        if (_amount < 0) { _amount = 0; }
        curHealth -= _amount;
        if (curHealth < 0) { curHealth = 0; }
    }

    public void PlusHealth(float _amount)
    {
        if (_amount < 0) { _amount = 0; }
        curHealth += _amount;
        if (_amount > maxHealth) { _amount = maxHealth; }
    }

    public void SetCurrentHealth(float _amount)
    {
        curHealth = _amount;
        if (curHealth < 0) { curHealth = 0; }
        if (curHealth > maxHealth) { curHealth = maxHealth; }
    }

    public void SetMaxHealth(float _amount)
    {
        if(_amount < 0) { _amount = 0; }
        maxHealth = _amount;
    }
}

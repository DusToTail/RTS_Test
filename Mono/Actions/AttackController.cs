using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackController : MonoBehaviour
{
    [SerializeField]
    private float attackDamage;
    [SerializeField]
    private float attackRange;
    [SerializeField]
    private float attackCooldown;

    private float curAttackCooldown;
    private float cellRadius;

    public Rigidbody rb { get; private set; }
    private Vision vision;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        vision = GetComponent<Vision>();
        curAttackCooldown = 0;
        cellRadius = FindObjectOfType<GridController>().cellRadius;
    }

    private void Update()
    {
        if(!CanAttack())
            SetCurrentAttackCooldown(curAttackCooldown + Time.deltaTime);

    }

    

    /// <summary>
    /// English: Return true if current attack cooldown is greater than its set cooldown (set in inspector). Else, false
    /// 日本語：現在の攻撃のクールダウンが定めたクールダウン（Inspectorで定める）より大きい場合、trueを返す。そうではない、falseを返す。
    /// </summary>
    /// <returns></returns>
    public bool CanAttack()
    {
        return curAttackCooldown > attackCooldown;
    }
    /// <summary>
    /// English: Return the attack cooldown (attack speed)
    /// 日本語：攻撃のクールダウン（攻撃スピード）を返す
    /// </summary>
    /// <returns></returns>
    public float GetAttackCooldown()
    {
        return attackCooldown;
    }
    /// <summary>
    /// English: Return the attack damage
    /// 日本語：攻撃のダメージを返す
    /// </summary>
    /// <returns></returns>
    public float GetAttackDamage()
    {
        return attackDamage;
    }
    /// <summary>
    /// English: Return the attack range
    /// 日本語：攻撃の距離を返す
    /// </summary>
    /// <returns></returns>
    public float GetAttackRange()
    {
        return attackRange * cellRadius * 2;
    }
    /// <summary>
    /// English: Return the current attack cooldown
    /// 日本語：現在の攻撃のクールダウンを返す
    /// </summary>
    /// <returns></returns>
    public float GetCurrentAttackCooldown()
    {
        return curAttackCooldown;
    }

    public IEntity ReturnNearbyEnemy(Vector3 _curPosition, float _visionRange)
    {
        Collider[] colliders = Physics.OverlapCapsule(_curPosition + Vector3.down * 50, _curPosition + Vector3.up * 50, _visionRange, LayerMask.GetMask(Tags.Selectable));
        List<IEntity> enemyList = new List<IEntity>();

        foreach (Collider collider in colliders)
        {
            if (collider.gameObject.GetComponent<IEntity>().GetPlayerIndex() != collider.gameObject.GetComponent<IEntity>().GetPlayerIndex())
            {
                enemyList.Add(collider.gameObject.GetComponent<IEntity>());
            }
        }

        float closestDistance = float.MaxValue;
        IEntity closestEnemy = null;

        foreach (IEntity curEnemy in enemyList)
        {
            float distance = Vector3.Distance(curEnemy.GetWorldPosition(), _curPosition);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestEnemy = curEnemy;
            }
        }

        return closestEnemy;
    }

    /// <summary>
    /// English: Set the attack cooldown (attack speed)
    /// 日本語：攻撃のクールダウン（攻撃スピード）を割り当てる。
    /// </summary>
    /// <param name="_time"></param>
    public void SetAttackCooldown(float _time)
    {
        attackCooldown = _time;
    }
    /// <summary>
    /// English: Set the attack damage
    /// 日本語：攻撃のダメージを割り当てる。
    /// </summary>
    /// <param name="_amount"></param>
    public void SetAttackDamage(float _amount)
    {
        attackDamage = _amount;
    }
    /// <summary>
    /// English: Set the attack range
    /// 日本語：攻撃の距離を割り当てる。
    /// </summary>
    /// <param name="_range"></param>
    public void SetAttackRange(float _range)
    {
        attackRange = _range;
    }
    /// <summary>
    /// English: Set current attack cooldown to the specified time. used in the unit attack action
    /// 日本語：現在の攻撃クールダウンを指定した値にする。攻撃の行動で使用される
    /// </summary>
    /// <param name="_setTime"></param>
    public void SetCurrentAttackCooldown(float _setTime)
    {
        curAttackCooldown = _setTime;
    }

}

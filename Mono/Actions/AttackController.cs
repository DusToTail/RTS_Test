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
    /// ���{��F���݂̍U���̃N�[���_�E������߂��N�[���_�E���iInspector�Œ�߂�j���傫���ꍇ�Atrue��Ԃ��B�����ł͂Ȃ��Afalse��Ԃ��B
    /// </summary>
    /// <returns></returns>
    public bool CanAttack()
    {
        return curAttackCooldown > attackCooldown;
    }
    /// <summary>
    /// English: Return the attack cooldown (attack speed)
    /// ���{��F�U���̃N�[���_�E���i�U���X�s�[�h�j��Ԃ�
    /// </summary>
    /// <returns></returns>
    public float GetAttackCooldown()
    {
        return attackCooldown;
    }
    /// <summary>
    /// English: Return the attack damage
    /// ���{��F�U���̃_���[�W��Ԃ�
    /// </summary>
    /// <returns></returns>
    public float GetAttackDamage()
    {
        return attackDamage;
    }
    /// <summary>
    /// English: Return the attack range
    /// ���{��F�U���̋�����Ԃ�
    /// </summary>
    /// <returns></returns>
    public float GetAttackRange()
    {
        return attackRange * cellRadius * 2;
    }
    /// <summary>
    /// English: Return the current attack cooldown
    /// ���{��F���݂̍U���̃N�[���_�E����Ԃ�
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
    /// ���{��F�U���̃N�[���_�E���i�U���X�s�[�h�j�����蓖�Ă�B
    /// </summary>
    /// <param name="_time"></param>
    public void SetAttackCooldown(float _time)
    {
        attackCooldown = _time;
    }
    /// <summary>
    /// English: Set the attack damage
    /// ���{��F�U���̃_���[�W�����蓖�Ă�B
    /// </summary>
    /// <param name="_amount"></param>
    public void SetAttackDamage(float _amount)
    {
        attackDamage = _amount;
    }
    /// <summary>
    /// English: Set the attack range
    /// ���{��F�U���̋��������蓖�Ă�B
    /// </summary>
    /// <param name="_range"></param>
    public void SetAttackRange(float _range)
    {
        attackRange = _range;
    }
    /// <summary>
    /// English: Set current attack cooldown to the specified time. used in the unit attack action
    /// ���{��F���݂̍U���N�[���_�E�����w�肵���l�ɂ���B�U���̍s���Ŏg�p�����
    /// </summary>
    /// <param name="_setTime"></param>
    public void SetCurrentAttackCooldown(float _setTime)
    {
        curAttackCooldown = _setTime;
    }

}

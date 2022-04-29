using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// English: A class that control the execution and dequeueing actions of enqueued by the Select and CommandSystem class
/// ���{��FSelect�����CommandSystem�N���X�ɂ���ăL���[���ꂽ�s����dequeue�Ǝ��s���Ǘ�����N���X
/// </summary>
[RequireComponent(typeof(IUnit))]
public class UnitActionController : MonoBehaviour
{
    public Queue<IAction> actionQueue { get; private set; }
    private IAction curAction;

    // Components
    private IUnit unitInfo;

    private void Awake()
    {
        //Initialization of components, queue and variables;
        unitInfo = gameObject.GetComponent<IUnit>();
        actionQueue = new Queue<IAction>();

        curAction = unitInfo.ReturnNewAction();
        curAction.Stop();

    }
    private void Start()
    {
        
    }

    private void Update()
    {
        DequeueActionToCurrentAction();
    }

    private void FixedUpdate()
    {
        ExecuteCurrentAction();
    }

    /// <summary>
    /// English: Perform the current action (dequeue from the queue) per frame fixedUpdated.
    /// ���{��F�t���[�����ƂɁiFixedUpdate�j���݂̍s�������s����B
    /// </summary>
    private void ExecuteCurrentAction()
    {
        //Validity Check
        if (curAction == null || curAction.isFinished == true) { return; }
        switch ((int)curAction.GetSelf().type)
        {
            case 0:
                curAction.GetSelf().MoveTowards(unitInfo.GetRigidbody(), unitInfo.GetSetMovementSpeed());
                //Debug.Log($"{gameObject.name} is moving");
                break;
            case 1:
                curAction.GetSelf().Patrol(unitInfo.GetRigidbody(), unitInfo.GetSetMovementSpeed());
                //Debug.Log($"{gameObject.name} is patrolling");
                break;
            case 2:
                curAction.GetSelf().AttackTarget(unitInfo.GetRigidbody(), unitInfo.GetSetMovementSpeed(), unitInfo.GetSetAttackDamage(), unitInfo.GetSetAttackRange(), unitInfo.GetSetVisionRange(), unitInfo.CanAttack());
                //Debug.Log($"{gameObject.name} is attacking target");
                break;
            case 3:
                curAction.GetSelf().AttackMove(unitInfo.GetRigidbody(), unitInfo.GetSetMovementSpeed(), unitInfo.GetSetAttackDamage(), unitInfo.GetSetAttackRange(), unitInfo.GetSetVisionRange(), unitInfo.CanAttack());
                //Debug.Log($"{gameObject.name} is attack moving");
                break;
            case 4:
                curAction.GetSelf().StopMoving(unitInfo.GetRigidbody());
                //Debug.Log($"{gameObject.name} is stop moving");
                break;
            case 5:
                curAction.GetSelf().Stop();
                //Debug.Log($"{gameObject.name} is stopping");
                break;

            case 6:
                curAction.GetSelf().Cancel();
                break;

            default:
                break;

        }
    }

    /// <summary>
    /// English: Enqueue an action. If isInstant, clear the queue first
    /// ���{��F�s�����L���[����BisInstant�̏ꍇ�A��ɃL���[���N���A����B
    /// </summary>
    /// <param name="_action"></param>
    /// <param name="isInstant"></param>
    public void EnqueueAction(IAction _action, bool isInstant)
    {
        if (isInstant == true)
        {
            if (curAction != null) { curAction.Stop(); }

            
            while(actionQueue.Count > 0)
            {
                curAction = actionQueue.Dequeue();
                curAction.Stop();
            }
            actionQueue.Clear();
            actionQueue.TrimExcess();
            Debug.Log("Clear Action List");
        }
        actionQueue.Enqueue(_action);

        Debug.Log("Action Enqueued");
    }

    /// <summary>
    /// English: Dequeue an action to the current action.
    /// ���{��F�L���[����s�������o���A���݂̍s���Ɋ��蓖�Ă�B
    /// </summary>
    private void DequeueActionToCurrentAction()
    {
        //Validity Check
        if (curAction.isFinished == false) { return; }
        if(actionQueue.Count == 0) { return; }

        curAction = actionQueue.Dequeue();

        Debug.Log("Action Dequeued");

    }


}

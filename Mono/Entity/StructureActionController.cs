using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// English: A class that control the execution and dequeueing actions of enqueued by the Select and CommandSystem class
/// ���{��FSelect�����CommandSystem�N���X�ɂ���ăL���[���ꂽ�s����dequeue�Ǝ��s���Ǘ�����N���X
/// </summary>
[RequireComponent(typeof(IStructure))]
public class StructureActionController : MonoBehaviour
{
    public Queue<IAction> actionQueue { get; private set; }
    private IAction curAction;

    // Components
    private IStructure structureInfo;

    private void Awake()
    {
        //Initialization of components, queue and variables;
        structureInfo = gameObject.GetComponent<IStructure>();
        actionQueue = new Queue<IAction>();

        curAction = structureInfo.ReturnNewAction();
        curAction.Stop();
    }

    private void Start()
    {
        
    }

    private void Update()
    {
        DequeueActionToCurrentAction();

        ExecuteCurrentActionBasedOnStructureType();
    }

    /// <summary>
    /// English: Perform the current action (dequeue from the queue) per frame fixedUpdated.
    /// ���{��F�t���[�����ƂɁiFixedUpdate�j���݂̍s�������s����B
    /// </summary>
    private void ExecuteCurrentActionBasedOnStructureType()
    {
        //Validity Check
        if (curAction == null || curAction.isFinished == true) { return; }

        switch((int)curAction.GetSelf().type)
        {
            case 0:
                curAction.GetSelf().MoveTowards(structureInfo.GetRigidbody(), 0);
                //Debug.Log($"{gameObject.name} is moving");
                break;
            case 1:
                curAction.GetSelf().Patrol(structureInfo.GetRigidbody(), 0);
                //Debug.Log($"{gameObject.name} is patrolling");
                break;
            case 2:
                curAction.GetSelf().AttackTarget(structureInfo.GetRigidbody(), 0, structureInfo.GetSetAttackDamage(), structureInfo.GetSetAttackRange(), structureInfo.GetSetVisionRange(), structureInfo.CanAttack());
                //Debug.Log($"{gameObject.name} is attacking target");
                break;
            case 3:
                curAction.GetSelf().AttackMove(structureInfo.GetRigidbody(), 0, structureInfo.GetSetAttackDamage(), structureInfo.GetSetAttackRange(), structureInfo.GetSetVisionRange(), structureInfo.CanAttack());
                //Debug.Log($"{gameObject.name} is attack moving");
                break;
            case 4:
                curAction.GetSelf().StopMoving(structureInfo.GetRigidbody());
                //Debug.Log($"{gameObject.name} is stop moving");
                break;
            case 5:
                curAction.GetSelf().Stop();
                //Debug.Log($"{gameObject.name} is stopping");
                break;

            case 6:
                curAction.GetSelf().Cancel();
                //Debug.Log($"{gameObject.name} canceled");
                break;

            case 100:
                curAction.GetSelf().SpawnUnit();
                //Debug.Log($"{gameObject.name} is spawning unit");
                break;
            case 101:
                curAction.GetSelf().Upgrade();
                //Debug.Log($"{gameObject.name} is upgrading");
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


            while (actionQueue.Count > 0)
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
        if (actionQueue.Count == 0) { return; }

        curAction = actionQueue.Dequeue();

        Debug.Log("Action Dequeued");

    }

}

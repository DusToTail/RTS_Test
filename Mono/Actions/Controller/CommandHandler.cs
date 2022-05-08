using UnityEngine;

/// <summary>
/// English: A class that control the execution and dequeueing actions of enqueued by the Select and CommandSystem class
/// ���{��FSelect�����CommandSystem�N���X�ɂ���ăL���[���ꂽ�s����dequeue�Ǝ��s���Ǘ�����N���X
/// </summary>
public class CommandHandler : MonoBehaviour
{
    [SerializeField]
    private ActionProcessor actionProcessor;
    [SerializeField]
    private ProductionProcessor productionProcessor;

    public void EnqueueCommand(IAction _action, bool _isQueued)
    {
        if (_action is IProgress)
        {
            if (productionProcessor == null) { return; }
            if (!productionProcessor.QueueIsFull())
            {
                productionProcessor.EnqueueAction(_action);
            }
        }
        else
        {
            if(actionProcessor == null) { return; }
            actionProcessor.EnqueueAction(_action, _isQueued);
        }
    }



}

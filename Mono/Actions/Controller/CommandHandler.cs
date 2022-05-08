using UnityEngine;

/// <summary>
/// English: A class that control the execution and dequeueing actions of enqueued by the Select and CommandSystem class
/// 日本語：SelectおよびCommandSystemクラスによってキューされた行動のdequeueと実行を管理するクラス
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

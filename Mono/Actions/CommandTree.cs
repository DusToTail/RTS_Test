using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandTree : MonoBehaviour
{
    public CommandNode main;
    public int[] commandIntArray;

    private void Start()
    {
        main = new CommandNode(0, commandIntArray, 0);

    }



}

public class CommandNode
{
    public int selfCommandInt;

    public CommandNode[] branch;
    public int layerIndex;

    

    public CommandNode(int _selfCommandInt, int[] _commnadIntArray,int _layerIndex)
    {
        selfCommandInt = _selfCommandInt;
        layerIndex = _layerIndex;

        branch = new CommandNode[15];

    }


}

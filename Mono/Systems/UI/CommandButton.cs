using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CommandButton : MonoBehaviour, IButton
{
    public SelectManager selectManager;
    public CommandManager commandManager;
    

    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(() => OnClick());
        

    }

    public void OnClick()
    {

    }

    public void AssignCommand(int _commandInt)
    {

    }



}

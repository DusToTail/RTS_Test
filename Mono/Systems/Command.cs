using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Command
{
    public KeyCode shortcut;
    public string commandName;
    public string commandDescription;
    public Sprite icon;
    public int commandInt;

    public Command(KeyCode _key, string _name, string _description, Sprite _icon, int _commandInt)
    {
        shortcut = _key;
        commandName = _name;
        commandDescription = _description;
        icon = _icon;
        commandInt = _commandInt;
    }

}

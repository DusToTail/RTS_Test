using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hotkeys
{
    public static KeyCode moveCommand = KeyCode.M;
    public static KeyCode patrolCommand = KeyCode.P;
    public static KeyCode stopCommand = KeyCode.S;
    public static KeyCode holdCommand = KeyCode.H;
    public static KeyCode attackCommand = KeyCode.A;

    public static KeyCode buildBasicCommand = KeyCode.V;
    public static KeyCode buildAdvanceCommand = KeyCode.B;
    public static KeyCode upgradeCommand = KeyCode.U;


    public static KeyCode cancelCommand = KeyCode.Escape;


    public static Dictionary<KeyCode, CommandManager.CommandInt> hotkeyMap = new Dictionary<KeyCode, CommandManager.CommandInt>()
    {
        {moveCommand, CommandManager.CommandInt.Move},
        {patrolCommand, CommandManager.CommandInt.Patrol},
        {attackCommand, CommandManager.CommandInt.Attack},
        {holdCommand, CommandManager.CommandInt.Hold},
        {stopCommand, CommandManager.CommandInt.Stop},

        {buildBasicCommand, CommandManager.CommandInt.Build},
        {buildAdvanceCommand, CommandManager.CommandInt.Build},

        {upgradeCommand, CommandManager.CommandInt.Upgrade},

        {cancelCommand, CommandManager.CommandInt.Cancel}

    };
}

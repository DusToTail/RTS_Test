using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Upgrade
{
    public string name;
    public string description;

    public enum Type
    {
        Stats,
        Unlock
    }
    public Type type;


}

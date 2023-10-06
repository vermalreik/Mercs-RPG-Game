using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move // Its plain C# class
{
    public MoveBase Base { get; set; }
    // This is a short way of creating Properties.
    //C# will create automatically a private variable behind the scenes
    // You can NOT use this short way if you want your variabels to be shown in the inspector
    // It's a GOOD PRACTICE to capitalize the first letter of the Property name
    public int PP { get; set; }

    public Move(MoveBase pBase)
    {
        Base = pBase;
        PP = pBase.PP;
    }
}

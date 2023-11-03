using System;
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

    public Move(MoveSaveData saveData)
    {
        Base = MoveDB.GetMoveByName(saveData.name);
        PP = saveData.pp;
    }

    public MoveSaveData GetSaveData()
    {
        var saveData = new MoveSaveData()
        {
            name = Base.Name, // If you write 'name' instead of 'Name' there will be an Error at Runtime, because it will take the name of the Scriptable Object
            pp = PP
        };
        return saveData;
    }

    public void IncreasePP(int amount)
    {
        PP = Mathf.Clamp(PP + amount, 0, Base.PP);
    }
}

[Serializable]
public class MoveSaveData
{
    public string name;
    public int pp;

}

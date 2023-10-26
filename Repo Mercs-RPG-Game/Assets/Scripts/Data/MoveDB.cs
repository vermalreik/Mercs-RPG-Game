using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveDB
{
    static Dictionary<string, MoveBase> moves;

    public static void Init()
    {
        moves = new Dictionary<string, MoveBase>();

        // Load scriptable objects form code
        var moveList = Resources.LoadAll<MoveBase>("");
        // This function will only load the objects that are inside a folder called 'Resources'

        foreach(var move in moveList)
        {
            if(moves.ContainsKey(move.Name)) // you have to use "N" uppercase. If you use '.name' it will take the name of the scriptable object instead of 'Name' property from that class
            {
                Debug.Log($"There are two moves with the name {move.Name}");
                continue;
            }
            moves[move.Name] = move;
        }
    }

    public static MoveBase GetMoveByName(string name)
    {
        if(!moves.ContainsKey(name))
        {
            Debug.Log($"Move with name {name} not found in the database");
            return null;
        }

        return moves[name];
    }
}

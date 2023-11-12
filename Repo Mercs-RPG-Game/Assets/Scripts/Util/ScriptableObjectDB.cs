using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptableObjectDB<T> : MonoBehaviour where T : ScriptableObject
{
    static Dictionary<string, T> objects;

    public static void Init()
    {
        objects = new Dictionary<string, T>();

        // Load scriptable objects form code
        var objectArray = Resources.LoadAll<T>("");
        // This function will only load the objects that are inside a folder called 'Resources'

        foreach(var obj in objectArray)
        {
            if(objects.ContainsKey(obj.name)) // you have to use "N" uppercase. If you use '.name' it will take the name of the scriptable object instead of 'Name' property from that class
            {
                Debug.Log($"There are two pokemons with the name {obj.name}");
                continue;
            }
            objects[obj.name] = obj;
        }
    }

    public static T GetObjectByName(string name)
    {
        if(!objects.ContainsKey(name))
        {
            Debug.Log($"Object with name {name} not found in the database");
            return null;
        }

        return objects[name];
    }
}

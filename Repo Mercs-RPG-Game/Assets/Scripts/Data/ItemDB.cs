using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDB
{
    static Dictionary<string, ItemBase> items;

    public static void Init()
    {
        items = new Dictionary<string, ItemBase>();

        // Load scriptable objects form code
        var itemList = Resources.LoadAll<ItemBase>("");
        // This function will only load the objects that are inside a folder called 'Resources'

        foreach(var item in itemList)
        {
            if(items.ContainsKey(item.Name)) // you have to use "N" uppercase. If you use '.name' it will take the name of the scriptable object instead of 'Name' property from that class
            {
                Debug.Log($"There are two items with the name {item.Name}");
                continue;
            }
            items[item.Name] = item;
        }
    }

    public static ItemBase GetItemByName(string name)
    {
        if(!items.ContainsKey(name))
        {
            Debug.Log($"Item with name {name} not found in the database");
            return null;
        }

        return items[name];
    }
}

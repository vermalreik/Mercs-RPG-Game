using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Xml.Schema;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class ItemBase : ScriptableObject
{
    [SerializeField] string name;
    [SerializeField] string description;
    [SerializeField] Sprite icon;

    public virtual string Name => name;
    public string Descrption => description;
    public Sprite Icon => icon; 

    public virtual bool Use(Pokemon pokemon)
    {
        return false;
    }

    public virtual bool isReusable => false;
    public virtual bool CanUseInBattle => true;
    public virtual bool CanUseOutsideBattle => true;
}

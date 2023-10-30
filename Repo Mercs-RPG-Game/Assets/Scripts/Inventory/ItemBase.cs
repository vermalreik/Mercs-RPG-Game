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

    public string Name => name;
    public string Descrption => description;
    public Sprite Icon => icon; 
}

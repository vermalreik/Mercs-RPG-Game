using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using UnityEngine;
using UnityEngine.Android;

public class Condition
{
    public string Name { get; set; }
    public string Description { get; set; }
    public string StartMessage { get; set; }

    public Action<Pokemon> OnAfterTurn { get; set; }
}

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

    public Action<Pokemon> OnStart { get; set; }
    public Func<Pokemon, bool> OnBeforeMove { get; set; }
    public Action<Pokemon> OnAfterTurn { get; set; }
}

// When we ues Actions we can only assign functions that doesn't return any value
// So if we want to return a value then we should use Func instance
// NOPE: public Action<Pokemon> OnBeforeMove { get; set; }
//  YEP: public Func<Pokemon, bool> OnBeforeMove { get; set; }
// we specify the retrun type after the parameters

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLayers : MonoBehaviour
{
    [SerializeField] LayerMask solidObjectsLayer;
    [SerializeField] LayerMask interactableLayer;
    [SerializeField] LayerMask grassLayer;

    // Singleton pattern
    public static GameLayers i { get;set; } // i = instance
    private void Awake() {
        i = this;
    }
    // :D fin singleton

    public LayerMask SolidLayer{
        get => solidObjectsLayer;
    }

    public LayerMask InteractableLayer{
        get => interactableLayer;
    }

    public LayerMask GrassLayer{
        get => grassLayer;
    }

}

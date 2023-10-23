using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLayers : MonoBehaviour
{
    [SerializeField] LayerMask solidObjectsLayer;
    [SerializeField] LayerMask interactableLayer;
    [SerializeField] LayerMask grassLayer;
    [SerializeField] LayerMask playerLayer;
    [SerializeField] LayerMask fovLayer;
    [SerializeField] LayerMask portalLayer;

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

    public LayerMask PlayerLayer{
        get => playerLayer;
    }

    public LayerMask FovLayer{
        get => fovLayer;
    }

    public LayerMask PortalLayer{
        get => portalLayer;
    }

    public LayerMask TriggerableLayers{
        get => grassLayer | fovLayer | portalLayer;
    }

}

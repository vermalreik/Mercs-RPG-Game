using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour, IPlayerTriggerable
{
    //private void OnTriggerEnter2D(Collider2D other){}
    // we can't use this apporach for a tile base game, the reason is because we don't want to trigger the code as soon as the player enters the trigger
    // we only want to do that once the player reaches the center of the tile that has the trigger
    
    public void onPlayerTriggered(PlayerController player)
    {
        Debug.Log("Player entered the portal");
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// all Game Objects that need to be triggered by the player will implement this interface
public interface IPlayerTriggerable
{
    void onPlayerTriggered(PlayerController player);
}

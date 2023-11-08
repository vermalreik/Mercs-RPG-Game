using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoryItem : MonoBehaviour, IPlayerTriggerable
{
    [SerializeField] Dialog dialog;

     public void onPlayerTriggered(PlayerController player)
    {
        //Debug.Log("Story item is working");
        player.Character.Animator.IsMoving = false;
        StartCoroutine(DialogManager.Instance.ShowDialog(dialog));
    }

    public bool TriggerRepeatedly => false;
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCController : MonoBehaviour, Interactable
{
    [SerializeField] Dialog dialog;
    [SerializeField] QuestBase questToStart;
    [SerializeField] List<Vector2> movementPattern;
    [SerializeField] float timeBetweenPattern;

    NPCState state;
    float idleTimer = 0f; // it will keep track of the time
    int currentPattern = 0;
    Quest activeQuest;

    Character character;
    ItemGiver itemGiver;
    
    private void Awake() {
        character = GetComponent<Character>();
        itemGiver = GetComponent<ItemGiver>();
    }

    public IEnumerator Interact(Transform initiator)
    {
        //Debug.Log("Interacting with NPC");
        // xD test NPC walks away when player interacts with it
        //StartCoroutine(character.Move(new Vector2(0, 2)));

        if(state == NPCState.Idle)
        {
            state = NPCState.Dialog;
            character.LookTowards(initiator.position);

            if(itemGiver != null && itemGiver.CanBeGiven())
            {
                yield return itemGiver.GiveItem(initiator.GetComponent<PlayerController>());
            }
            else if(questToStart != null)
            {
                activeQuest = new Quest(questToStart);
                yield return activeQuest.StartQuest();
                questToStart = null;
            }
            else if(activeQuest != null)
            {
                if(activeQuest.CanBeCompleted())
                {
                    yield return activeQuest.CompleteQuest(initiator);
                    activeQuest = null;
                }
                else
                {
                    yield return DialogManager.Instance.ShowDialog(activeQuest.Base.InProgressDialogue);
                }
            }
            else
            {
                yield return DialogManager.Instance.ShowDialog(dialog);
            }

            idleTimer = 0f;
            state = NPCState.Idle;
        }
    }

    private void Update()
    {
        if(state == NPCState.Idle)
        {
            idleTimer += Time.deltaTime;
            if(idleTimer > timeBetweenPattern)
            { 
                idleTimer = 0f;
                if(movementPattern.Count > 0)
                    StartCoroutine(Walk());
            }
        }

        character.HandleUpdate();
    }

    IEnumerator Walk()
    {
        state = NPCState.Walking;

        var oldPos = transform.position;

        yield return character.Move(movementPattern[currentPattern]);

        if(transform.position != oldPos)
            currentPattern = (currentPattern + 1) % movementPattern.Count; // we ues "% movementPattern.Count" so when we reach the last pattern we go back to the first pattern

        state = NPCState.Idle;
    }

}

public enum NPCState{ Idle, Walking, Dialog }

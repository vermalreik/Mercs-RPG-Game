using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Cutscene : MonoBehaviour, IPlayerTriggerable
{
    [SerializeReference] // Serialize it as a reference instead of as values
    [SerializeField] List<CutsceneAction> actions;

    public bool TriggerRepeatedly => false;

    public IEnumerator Play()
    {
        GameController.Instance.StartCutsceneState();

        foreach (var action in actions)
        {
            if(action.WaitForCompletion)
                yield return action.Play();
            else
                StartCoroutine(action.Play());
        }

        GameController.Instance.StartFreeRoamState();
    }

    public void AddAction(CutsceneAction action)
    {
        // preprocessor directive so it will only compile this line if we're in the Editor, not when we're trying tobuild the game
        #if UNITY_EDITOR
        Undo.RegisterCompleteObjectUndo(this, "Add action to cutscene.");
        #endif
        action.Name = action.GetType().ToString();
        actions.Add(action);
    }

    public void onPlayerTriggered(PlayerController player)
    {
        player.Character.Animator.IsMoving = false;
        StartCoroutine(Play());
    }
}

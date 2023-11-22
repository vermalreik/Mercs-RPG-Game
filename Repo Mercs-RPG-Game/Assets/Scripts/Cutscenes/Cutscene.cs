using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cutscene : MonoBehaviour
{
    [SerializeReference] // Serialize it as a reference instead of as values
    [SerializeField] List<CutsceneAction> actions;

    public void AddAction(CutsceneAction action)
    {
        action.Name = action.GetType().ToString();
        actions.Add(action);
    }
}

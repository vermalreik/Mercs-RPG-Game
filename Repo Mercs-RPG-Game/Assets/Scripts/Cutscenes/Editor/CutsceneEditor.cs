using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Cutscene))]
public class CutsceneEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var cutscene = target as Cutscene; // target = get the object that is being inspected

        if(GUILayout.Button("Add Dialogue Action")) // returns true if the button is click
            cutscene.AddAction(new DialogueAction());

        else if(GUILayout.Button("Add Move Actor Action"))
            cutscene.AddAction(new MoveActorAction());
        
        base.OnInspectorGUI();
    }
}

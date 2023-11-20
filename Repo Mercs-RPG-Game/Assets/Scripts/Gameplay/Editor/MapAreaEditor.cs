using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MapArea))]
public class MapAreaEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        // All the variables of the MapArea Script will be in a property called serializedObject
        int totalChance = serializedObject.FindProperty("totalChance").intValue;

        var style = new GUIStyle();
        style.fontStyle = FontStyle.Bold;

        GUILayout.Label($"Total Chance = {totalChance}", style);

        if(totalChance != 100)
            EditorGUILayout.HelpBox("The total chance percentage is not 100", MessageType.Error);
    }
}

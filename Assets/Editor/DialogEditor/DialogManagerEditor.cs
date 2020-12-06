using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(DialogManager))]
public class DialogManagerEditor : Editor
{
    // Only show typewriter speed in DialogManager inspector window when the flag is selected
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        DialogManager dm = target as DialogManager;

        if (dm.TypewriterEffect)
        {
            dm.TypewriterSpeed = EditorGUILayout.Slider("    Typewriter Speed", dm.TypewriterSpeed, 1, 100);
        }

        if (GUILayout.Button("Set Dialog Save Path"))
        {
            EditorPrefs.SetString("DialogSavePath", dm.SavePath);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Dialogue))]
public class DialogueEditor : Editor
{
    public override void OnInspectorGUI()
    {
        Dialogue dialog = target as Dialogue;


        SerializedObject sDialog = new SerializedObject(dialog);
        SerializedProperty sTitle = sDialog.FindProperty("_title");
        SerializedProperty sDefActor = sDialog.FindProperty("_defaultActor");

        EditorGUILayout.BeginVertical();
        EditorGUILayout.PropertyField(sTitle, new GUIContent("Title"));
        EditorGUILayout.ObjectField(sDefActor, new GUIContent("Default Actor", "Each new sentence automatically inherits this actor."));
        EditorGUILayout.EndVertical();

        sDialog.ApplyModifiedProperties();


        EditorGUILayout.LabelField($"Sentences: {dialog.SentenceCount}", EditorStyles.miniLabel);
        EditorGUILayout.LabelField($"Responses: {dialog.ResponseCount}", EditorStyles.miniLabel);
        if(dialog.DeletedCount != 0){
            EditorGUILayout.LabelField(new GUIContent($"Soft-deleted nodes: {dialog.DeletedCount}", "Only for debug, no worries."), EditorStyles.miniLabel);
        }


        if (dialog.GetCountOfType(Sentence.SentenceType.Start) < 1)
        {
            EditorGUILayout.LabelField($"Add a START sentence.", EditorStyles.boldLabel);
        }
        if (dialog.GetCountOfType(Sentence.SentenceType.Start) > 1)
        {
            EditorGUILayout.LabelField($"Too many START sentences.", EditorStyles.boldLabel);
        }


        if (GUILayout.Button("Edit")) 
        {
            GraphEditorWindow.Open(dialog);

            NodeEditorWindow.Open();
        }
    }
}

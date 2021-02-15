using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace DialogueSystem
{
    /// <summary>
    /// Custom Inspector of Dialogue assets
    /// </summary>
    [CustomEditor(typeof(Dialogue))]
    public class DialogueEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            // the dialogue that is inspected
            Dialogue dialog = target as Dialogue;

            // serialized properties
            SerializedObject sDialog = new SerializedObject(dialog);
            SerializedProperty sTitle = sDialog.FindProperty("_title");
            SerializedProperty sDefActor = sDialog.FindProperty("_defaultActor");

            // title label
            EditorGUILayout.PropertyField(sTitle, new GUIContent("Title"));

            // default actor field
            EditorGUILayout.ObjectField(sDefActor, new GUIContent("Default Actor", "Each new sentence automatically inherits this actor."));

            // node counters
            EditorGUILayout.LabelField($"Sentences: {dialog.SentenceCount}", EditorStyles.miniLabel);
            EditorGUILayout.LabelField($"Responses: {dialog.ResponseCount}", EditorStyles.miniLabel);
            if (dialog.DeletedCount != 0)
            {
                EditorGUILayout.LabelField(new GUIContent($"Soft-deleted nodes: {dialog.DeletedCount}", "Only for debug, no worries."), EditorStyles.miniLabel);
            }

            // suggestions / alerts
            if (dialog.GetSentenceCount(Sentence.Variant.Start) < 1)
            {
                EditorGUILayout.LabelField($"Add a START sentence.", EditorStyles.boldLabel);
            }
            if (dialog.GetSentenceCount(Sentence.Variant.Start) > 1)
            {
                EditorGUILayout.LabelField($"Too many START sentences.", EditorStyles.boldLabel);
            }

            // edit button
            if (GUILayout.Button("Edit"))
            {
                GraphEditorWindow.Open(dialog);
                NodeEditorWindow.Open();
            }

            // save the edited properties
            sDialog.ApplyModifiedProperties();
        }
    }

}
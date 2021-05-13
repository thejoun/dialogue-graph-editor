using DialogueSystem.Runtime.Data;
using UnityEditor;
using UnityEngine;

namespace DialogueSystem.Editor
{
    /// <summary>
    /// Custom Inspector for Dialogue assets
    /// </summary>
    [CustomEditor(typeof(Dialogue))]
    public class DialogueCustomEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            // serialized objects and properties
            Dialogue dialog = target as Dialogue;
            SerializedObject sDialog = new SerializedObject(dialog);
            SerializedProperty sTitle = sDialog.FindProperty("_title");
            SerializedProperty sDefActor = sDialog.FindProperty("_defaultActor");

            // default fields
            EditorGUILayout.PropertyField(sTitle, new GUIContent("Title"));
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
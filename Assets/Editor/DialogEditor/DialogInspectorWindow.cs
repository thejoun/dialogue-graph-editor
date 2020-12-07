using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class DialogInspectorWindow : EditorWindow
{
    public static DialogInspectorWindow Instance => (DialogInspectorWindow)GetWindow(typeof(DialogInspectorWindow));


    private Node _selectedNode;

    private Vector2 _scrollPosition = new Vector2(0, 0);



    [MenuItem("Window/Dialog Editor/Inspector")]
    public static void ShowWindow()
    {
        var w = GetWindow(typeof(DialogInspectorWindow));
        w.titleContent = new GUIContent("Dialog Inspector");
    }

    public void SetNode(Node node)
    {
        _selectedNode = node;
    }

    private void OnGUI()
    {
        if(_selectedNode == null)
        {
            EditorGUILayout.LabelField("Select a node in Dialog Graph Editor");

            return;
        }
        else
        {
            EditorGUILayout.LabelField(_selectedNode.ToString(), EditorStyles.boldLabel);

            EditorGUILayout.Space();

            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);
            {
                EditorGUILayout.BeginVertical();
                {
                    if (_selectedNode.GetType() == typeof(SentenceNode))
                    {
                        ShowSentenceInfo(((SentenceNode)_selectedNode).Sentence);
                    }
                    else if (_selectedNode.GetType() == typeof(ResponseNode))
                    {
                        ShowResponseInfo(((ResponseNode)_selectedNode).Response);
                    }
                }
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndScrollView();
        }
    }

    private void ShowSentenceInfo(Sentence sentence)
    {
        sentence.Type = (Sentence.SentenceType)EditorGUILayout.EnumPopup(sentence.Type);

        if(sentence.Type == Sentence.SentenceType.End)
        {
            return;
        }

        sentence.Actor = (Actor)EditorGUILayout.ObjectField(sentence.Actor, typeof(Actor), false);

        sentence.Text = EditorGUILayout.TextArea(sentence.Text, GUILayout.Height(100));

        EditorGUILayout.LabelField($"Characters: {sentence.Text.Length}", EditorStyles.miniLabel);

        EditorGUILayout.Space();

        EditorGUI.indentLevel += 1;

        int rindex = -1;

        foreach(Response response in sentence.Responses)
        {
            rindex++;

            EditorGUILayout.LabelField($"Response {rindex}", EditorStyles.boldLabel);

            response.Trigger = EditorGUILayout.TextField(response.Trigger);

            response.Text = EditorGUILayout.TextArea(response.Text, GUILayout.Height(50));
        }
    }

    private void ShowResponseInfo(Response response)
    {
        EditorGUILayout.LabelField("It's a response");
    }
}

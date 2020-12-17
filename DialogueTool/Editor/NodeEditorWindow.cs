using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class NodeEditorWindow : EditorWindow
{
    public static NodeEditorWindow Instance => (NodeEditorWindow)GetWindow(typeof(NodeEditorWindow));


    private Node _selectedNode;

    private Vector2 _scrollPosition = new Vector2(0, 0);



    [MenuItem("Tools/Dialogue Tool/Node Inspector")]
    public static void Open()
    {
        NodeEditorWindow inspectorWindow = (NodeEditorWindow)GetWindow(typeof(NodeEditorWindow));
        if (inspectorWindow == null)
        {
            inspectorWindow = CreateInstance<NodeEditorWindow>();
        }
        inspectorWindow.titleContent = new GUIContent("Node Inspector");
    }



    public void SetNode(Node node)
    {
        _selectedNode = node;
    }

    private void OnGUI()
    {
        if(_selectedNode == null)
        {
            EditorGUILayout.LabelField("Select a node in the Graph Editor");

            return;
        }
        else
        {
            EditorGUILayout.LabelField(_selectedNode.ToString(), EditorStyles.boldLabel);

            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);
            {
                EditorGUILayout.BeginVertical();
                {
                    ShowSentenceInfo(_selectedNode.Sentence);
                }
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndScrollView();
        }
    }

    private void ShowSentenceInfo(Sentence sentence)
    {
        var wordWrap = EditorStyles.textArea.wordWrap;
        EditorStyles.textArea.wordWrap = true;

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Type", EditorStyles.label);
        sentence.Type = (Sentence.SentenceType)EditorGUILayout.EnumPopup(sentence.Type);
        EditorGUILayout.EndHorizontal();
        if(sentence.Type == Sentence.SentenceType.End)
        {
            return;
        }


        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Actor", EditorStyles.label);
        sentence.Actor = (Actor)EditorGUILayout.ObjectField(sentence.Actor, typeof(Actor), false);
        EditorGUILayout.EndHorizontal();


        GUILayout.Label("Text", EditorStyles.label);
        EditorStyles.textArea.wordWrap = true;
        sentence.Text = EditorGUILayout.TextArea(sentence.Text, EditorStyles.textArea, GUILayout.Height(100));
        EditorGUILayout.LabelField($"Characters: {sentence.Text.Length}", EditorStyles.miniLabel);


        EditorGUILayout.Space();


        int id = -1;
        Response toDelete = null;

        foreach(Response response in sentence.Responses)
        {
            id++;

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label($"Response {id}", EditorStyles.boldLabel);
            GUILayout.Label($"to Node {response.NextId}");
            if(GUILayout.Button(new GUIContent("Delete"))){
                toDelete = response;
            }
            EditorGUILayout.EndHorizontal();

            EditorGUI.indentLevel += 1;

            response.Text = EditorGUILayout.TextArea(response.Text, EditorStyles.textArea, GUILayout.Height(40));

            EditorGUILayout.LabelField($"Characters: {response.Text.Length}", EditorStyles.miniLabel);

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label($"     Triggers");
            response.Triggers = EditorGUILayout.TextField(response.Triggers);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label($"     Requisites");
            response.Requisites = EditorGUILayout.TextField(response.Requisites);
            EditorGUILayout.EndHorizontal();

            EditorGUI.indentLevel -= 1;

            EditorGUILayout.Space();
        }

        if(toDelete != null)
        {
            sentence.Responses.Remove(toDelete);
            GraphEditorWindow.Refresh();
        }

        EditorStyles.textArea.wordWrap = wordWrap;
    }
}

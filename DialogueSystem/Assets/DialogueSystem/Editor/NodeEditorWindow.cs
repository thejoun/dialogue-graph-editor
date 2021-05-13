using System.Linq;
using DialogueSystem.Runtime.Data;
using UnityEditor;
using UnityEngine;

namespace DialogueSystem.Editor
{
    /// <summary>
    /// Custom Editor Window for Nodes
    /// </summary>
    public class NodeEditorWindow : EditorWindow
    {
        public static NodeEditorWindow Instance => (NodeEditorWindow)GetWindow(typeof(NodeEditorWindow));
        
        public Node SelectedNode { get; set; }
        
        private Vector2 _scrollPosition = new Vector2(0, 0);


        [MenuItem("Tools/DialogueSystem/Node Editor")]
        public static NodeEditorWindow Open()
        {
            NodeEditorWindow inspectorWindow = (NodeEditorWindow)GetWindow(typeof(NodeEditorWindow)) 
                                               ?? CreateInstance<NodeEditorWindow>();
            inspectorWindow.titleContent = new GUIContent("Node Editor");
            return inspectorWindow;
        }

        private void OnGUI()
        {
            // write only a note if no node selected
            if (SelectedNode == null)
            {
                EditorGUILayout.LabelField("Select a node in the Graph Editor");
                return;
            }
            
            // title label
            EditorGUILayout.LabelField(SelectedNode.ToString(), EditorStyles.boldLabel);

            // show the sentence content
            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);
            {
                EditorGUILayout.BeginVertical();
                {
                    ShowSentence(SelectedNode.Sentence);
                }
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndScrollView();
        }

        // draw sentence content - method 1 (dirty)
        private void ShowSentence(Sentence sentence)
        {
            var wordWrap = EditorStyles.textArea.wordWrap;
            EditorStyles.textArea.wordWrap = true;

            // SENTENCE TYPE
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Type", EditorStyles.label);
            sentence.Type = (Sentence.Variant)EditorGUILayout.EnumPopup(sentence.Type);
            EditorGUILayout.EndHorizontal();
            if (sentence.Type == Sentence.Variant.End)
            {
                return;
            }

            // ACTOR OBJECT FIELD
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Actor", EditorStyles.label);
            sentence.Actor = (Actor)EditorGUILayout.ObjectField(sentence.Actor, typeof(Actor), false);
            EditorGUILayout.EndHorizontal();

            // ACTOR EXPRESSION CHOICE
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Expression", EditorStyles.label);
            var expressionOptions = sentence.Actor?.Expressions.Select(e => e.Title).ToArray();
            if(expressionOptions != null)
            {
                sentence._expressionID = EditorGUILayout.Popup(sentence._expressionID, expressionOptions);
            }
            EditorGUILayout.EndHorizontal();

            // SENTENCE TEXT AREA
            GUILayout.Label("Text", EditorStyles.label);
            var sentenceTextStyle = new GUIStyle(EditorStyles.textArea) { wordWrap = true };
            var sentenceTextContent = new GUIContent($"{sentence.Text}\n");
            var sentenceTextWidth = EditorGUIUtility.currentViewWidth;
            var sentenceTextHeight = sentenceTextStyle.CalcHeight(sentenceTextContent, sentenceTextWidth);
            sentence.Text = EditorGUILayout.TextArea(sentence.Text, sentenceTextStyle, GUILayout.Height(sentenceTextHeight));
            EditorGUILayout.LabelField($"Characters: {sentence.Text.Length}", EditorStyles.miniLabel);

            // TRIGGERS
            /*EditorGUILayout.BeginHorizontal();
            GUILayout.Label($"Triggers");
            var triggers = sentence.Triggers;
            int newCount = Mathf.Max(0, EditorGUILayout.IntField(triggers.Count));
            while (newCount < triggers.Count) triggers.RemoveAt(triggers.Count - 1);
            while (newCount > triggers.Count) triggers.Add(string.Empty);
            EditorGUILayout.EndHorizontal();
            EditorGUI.indentLevel++;
            for (int i = 0; i < triggers.Count; i++)
            {
                //triggers[i] = (Trigger)EditorGUILayout.ObjectField(triggers[i], typeof(Trigger), false);
                triggers[i] = EditorGUILayout.TextField(triggers[i]);
            }
            EditorGUI.indentLevel--;*/

            // SPACE
            EditorGUILayout.Space();

            // RESPONSES
            int id = -1;
            Response toDelete = null;
            foreach (Response response in sentence.Responses)
            {
                id++;

                // LABEL
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label($"Response {id}", EditorStyles.boldLabel);
                GUILayout.Label($"to Node {response.NextId}");
                if (GUILayout.Button(new GUIContent("Delete")))
                {
                    toDelete = response;
                }
                EditorGUILayout.EndHorizontal();

                EditorGUI.indentLevel += 1;

                // RESPONSE TEXT AREA
                var responseTextStyle = new GUIStyle(EditorStyles.textArea) { wordWrap = true };
                var responseTextContent = new GUIContent($"{response.Text}\n");
                var responseTextWidth = EditorGUIUtility.currentViewWidth;
                var responseTextHeight = responseTextStyle.CalcHeight(responseTextContent, responseTextWidth);
                response.Text = EditorGUILayout.TextArea(response.Text, responseTextStyle, GUILayout.Height(responseTextHeight));
                EditorGUILayout.LabelField($"Characters: {response.Text.Length}", EditorStyles.miniLabel);

                // CONDITIONS
                // EditorGUILayout.BeginHorizontal();
                // GUILayout.Label($"     Conditions");
                // response.Requisites = EditorGUILayout.TextField(response.Requisites);
                // EditorGUILayout.EndHorizontal();

                EditorGUI.indentLevel -= 1;

                EditorGUILayout.Space();
            }

            if (toDelete != null)
            {
                sentence.Responses.Remove(toDelete);
                GraphEditorWindow.Refresh();
            }

            EditorStyles.textArea.wordWrap = wordWrap;
        }

        // draw sentence content - method 2 (serialized)
        public void ShowSentence(Dialogue dialogue, int index)
        {
            if (index < 0 || dialogue.SentenceCount <= index) return;

            var sentence = dialogue?.Nodes[index]?.Sentence;

            var sDialogue = new SerializedObject(dialogue);
            var sNodes = sDialogue.FindProperty(nameof(Dialogue._nodes));
            var sNode = sNodes.GetArrayElementAtIndex(index);
            var sSentence = sNode.FindPropertyRelative(nameof(Node._sentence));
            var sTriggers = sSentence.FindPropertyRelative(nameof(Sentence._triggers));
            var sResponses = sSentence.FindPropertyRelative(nameof(Sentence._responses));

            // SENTENCE TYPE
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Type", EditorStyles.label);
            sentence.Type = (Sentence.Variant)EditorGUILayout.EnumPopup(sentence.Type);
            //EditorGUILayout.PropertyField(sentenceType);
            EditorGUILayout.EndHorizontal();

            // RETURN IF END NODE
            if (sentence.Type == Sentence.Variant.End)
            {
                return;
            }

            // ACTOR OBJECT FIELD
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Actor", EditorStyles.label);
            sentence.Actor = (Actor)EditorGUILayout.ObjectField(sentence.Actor, typeof(Actor), false);
            EditorGUILayout.EndHorizontal();

            // ACTOR EXPRESSION CHOICE
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Expression", EditorStyles.label);
            var expressionOptions = sentence.Actor?.Expressions.Select(e => e.Title).ToArray();
            if (expressionOptions != null)
            {
                sentence._expressionID = EditorGUILayout.Popup(sentence._expressionID, expressionOptions);
            }
            EditorGUILayout.EndHorizontal();

            // SENTENCE TEXT AREA
            GUILayout.Label("Text", EditorStyles.label);
            var sentenceTextStyle = new GUIStyle(EditorStyles.textArea) { wordWrap = true };
            var sentenceTextContent = new GUIContent($"{sentence.Text}\n");
            var sentenceTextWidth = EditorGUIUtility.currentViewWidth;
            var sentenceTextHeight = sentenceTextStyle.CalcHeight(sentenceTextContent, sentenceTextWidth);
            sentence.Text = EditorGUILayout.TextArea(sentence.Text, sentenceTextStyle, GUILayout.Height(sentenceTextHeight));
            EditorGUILayout.LabelField($"Characters: {sentence.Text.Length}", EditorStyles.miniLabel);

            // TRIGGERS
            //EditorGUILayout.PropertyField(sTriggers, true);

            // SPACE
            EditorGUILayout.Space();

            // RESPONSES
            EditorGUILayout.PropertyField(sResponses, true);

            //sDialogue.ApplyModifiedProperties();
        }
    }

    
}
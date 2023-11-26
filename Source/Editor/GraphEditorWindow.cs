using System.Collections.Generic;
using DialogueSystem.Runtime.Data;
using UnityEditor;
using UnityEngine;

namespace DialogueSystem.Editor
{
    /// <summary>
    /// Custom Editor Window for Dialogues
    /// </summary>
    public class GraphEditorWindow : EditorWindow
    {
        private readonly Vector2Int SENTENCE_NODE_DIM = new Vector2Int(130, 60);
        private readonly Vector2Int RESPONSE_NODE_DIM = new Vector2Int(100, 40);
        private readonly Vector2Int BUTTON_DIM = new Vector2Int(50, 30);

        private readonly Vector2 ADD_BUTTON_POS = new Vector2(15, 10);
        private readonly Vector2 DELETE_BUTTON_POS = new Vector2(80, 10);

        private const int TOP_PANEL_HEIGHT = 50;

        private const float ARROW_HEAD_SIZE = 9f;
        private const float TEXTURE_COLOR_PADDING = 3;

        private readonly Color BOX_COLOR = new Color(0.35f, 0.35f, 0.35f);
        private readonly Color LINE_COLOR = new Color(0.4f, 0.9f, 1f);
        private readonly Color ARROW_COLOR = new Color(0.8f, 0.9f, 1f);
        private readonly Color LIGHT_TEXT_COLOR = new Color(0.9f, 0.9f, 0.9f);
        private static Color DefaultColor => EditorGUIUtility.isProSkin ? 
            new Color32(56, 56, 56, 255) : new Color32(194, 194, 194, 255);

        private const int SENTENCE_TEXT_LENGTH = 21;
        private const int RESPONSE_TEXT_LENGTH = 16;

        private const float SCALE_MIN = 0.6f;
        private const float SCALE_MAX = 1.2f;
        
        private GUIStyle _titleLabelStyle;
        private GUIStyle _deleteButtonStyle;
        private GUIStyle _addButtonStyle;
        private GUIStyle _sentenceNodeStyle;
        private GUIStyle _startNodeStyle;
        private GUIStyle _endNodeStyle;
        private GUIStyle _responseNodeStyle;
        private GUIStyle _resetButtonStyle;
        
        private Rect BoxRect => new Rect(0, TOP_PANEL_HEIGHT, position.width, Mathf.Max(0, position.height - TOP_PANEL_HEIGHT));
        private Rect AddNodeButtonRect => new Rect(ADD_BUTTON_POS, BUTTON_DIM);
        private Rect DeleteNodeButtonRect => new Rect(DELETE_BUTTON_POS, BUTTON_DIM);
        private Rect NewNodeRect => new Rect((BoxRect.center - SENTENCE_NODE_DIM / 2 - new Vector2(Pan.x, Pan.y)) / Zoom, SENTENCE_NODE_DIM);
        private Rect ResetButtonRect => new Rect(new Vector2(405, 10), BUTTON_DIM);
        
        private static Rect TitleTextRect => new Rect(500, 10, 300, 30);
        private static Rect ScaleLabelRect => new Rect(150, 7, 100, 20);
        private static Rect ScaleSliderRect => new Rect(150, 25, 150, 20);
        private static Rect PositionLabelRect => new Rect(325, 7, 55, 20);
        private static Rect PositionValueRect => new Rect(325, 25, 55, 20);

        private bool IsConnecting { get; set; }                         // if currently connecting nodes             
        private Node ConnectingFrom { get; set; }                       // node that we are connecting from
        
        private Dialogue CurrentDialogue { get; set; }                  // current open dialogue
        private List<Node> Nodes => CurrentDialogue.Nodes;              // get all nodes
        private Node SelectedNode { get; set; }                         // currently selected node
        
        private float Zoom { get; set; } = 1;                             // current graph zoom
        private float RoundedZoom => Mathf.Round(Zoom * 100f) / 100f;   // get the rounded zoom
        private Vector2 Pan { get; set; } = Vector2.zero;                 // current graph position (panning)
        
        private int SentenceTextLength => Mathf.RoundToInt(SENTENCE_TEXT_LENGTH * Zoom);    // modified max sentence display length
        private int ResponseTextLength => Mathf.RoundToInt(RESPONSE_TEXT_LENGTH * Zoom);    // modified max response display length
        
        public Dialogue GetCurrentDialogue() => CurrentDialogue;
        public SerializedObject GetSerializedDialogue() => new SerializedObject(CurrentDialogue);
        public int GetSelectedID() => Nodes.IndexOf(SelectedNode);
        
        public static GraphEditorWindow Instance => (GraphEditorWindow)GetWindow(typeof(GraphEditorWindow));
        
        
        // open the window
        [MenuItem("Tools/DialogueSystem/Graph Editor")]
        public static GraphEditorWindow Open()
        {
            GraphEditorWindow graphEditorWindow = (GraphEditorWindow)GetWindow(typeof(GraphEditorWindow));
            if (graphEditorWindow == null)
            {
                graphEditorWindow = CreateInstance<GraphEditorWindow>();
            }
            Instance.titleContent = new GUIContent("Graph Editor");
            Instance.PrepareStyles();
            return graphEditorWindow;
        }

        // open a dialogue
        public static GraphEditorWindow Open(Dialogue dialogue)
        {
            var w = Open();
            w.CurrentDialogue = dialogue;
            EditorUtility.SetDirty(dialogue);
            return w;
        }

        // when an asset is selected in Unity, show it if it's a Dialogue
        private void OnSelectionChange()
        {
            UnityEngine.Object selection = Selection.activeObject;
            if (selection != null && selection.GetType() == typeof(Dialogue))
            {
                Open((Dialogue)selection);
                Repaint();
            }
        }

        // method called by the Node Inspector when changes are made to the graph
        public static void Refresh()
        {
            var g = Open();
            g.Repaint();
        }

        // draws the whole GUI
        private void OnGUI()
        {
            // return if no dialogue selected
            if (CurrentDialogue is null)
            {
                EditorGUILayout.BeginVertical();
                EditorGUILayout.LabelField($"Select a Dialogue asset.");
                EditorGUILayout.LabelField($"You can create one with RMB > Create > Dialogue.");
                EditorGUILayout.EndVertical();
                return;
            }

            // check if textures are loaded
            if (_sentenceNodeStyle.normal.background is null)
            {
                PrepareStyles();
                Repaint();
            }

            // mouse events grabbed at the beginning, because they become confusing later
            var current = Event.current;
            var leftClicked = Event.current.type == EventType.MouseDown && current.button == 0;
            var rightClicked = Event.current.type == EventType.MouseDown && current.button == 1;

            // draw background behind the graph
            EditorGUI.DrawRect(BoxRect, BOX_COLOR);

            // nodes
            int i = -1;
            BeginWindows();
            foreach (Node node in Nodes)
            {
                // skip this node if it's soft-deleted
                if (node.IsDeleted)
                    continue;

                i++;

                Sentence sentence = node.Sentence;

                // show text on the node
                string cutText = node.Sentence.Text.Cut(SentenceTextLength);
                string actor = string.Empty;
                if (sentence.Actor != null)
                    actor = sentence.Actor.Title.Cut(SentenceTextLength);

                // apply current zoom and pan
                node.Scale = RoundedZoom;
                node.Position = Pan;

                // draw windows based on node type
                if (sentence.Type == Sentence.Variant.Start)
                {
                    node.WindowRect = GUI.Window(node.Id, node.WindowRect, MakeNode,
                        new GUIContent($"{actor}\n{cutText}"), _startNodeStyle);
                }
                else if (sentence.Type == Sentence.Variant.End)
                {
                    node.WindowRect = GUI.Window(node.Id, node.WindowRect, MakeNode, $"END", _endNodeStyle);
                }
                else
                {
                    node.WindowRect = GUI.Window(node.Id, node.WindowRect, MakeNode, $"{actor}\n{cutText}",
                        _sentenceNodeStyle);
                }

                // check for node interactions if the mouse is over it
                if (node.WindowRect.Contains(Event.current.mousePosition))
                {
                    // right click on a node
                    if (rightClicked)
                    {
                        // if currently connecting, connect to this node
                        if (IsConnecting)
                        {
                            int id = Nodes.FindIndex(n => n.Equals(node));
                            if (!ConnectingFrom.Sentence.Responses.Exists(r => r.NextId == id))
                            {
                                Response response = new Response() {NextId = id, Text = ""};
                                ConnectingFrom.Sentence.Responses.Add(response);
                            }

                            IsConnecting = false;
                        }
                        // if not connecting, start connecting
                        else if (!node.Sentence.Type.Equals(Sentence.Variant.End))
                        {
                            ConnectingFrom = node;
                            IsConnecting = true;
                        }

                        Event.current.Use();
                    }
                    // left click on a node - select node and show in node inspector
                    else if (leftClicked)
                    {
                        GUI.BringWindowToFront(i);
                        SelectNode(node);
                        Event.current.Use();
                    }
                }

                // draw lines and responses
                Vector3 currentPos = node.WindowRect.center;
                foreach (Response response in node.Sentence.Responses)
                {
                    // line
                    Vector3 targetPos = Nodes[response.NextId].WindowRect.center;
                    Handles.color = LINE_COLOR;
                    Handles.DrawLine(currentPos, targetPos);

                    // box and arrows
                    Handles.color = ARROW_COLOR;
                    if (!response.Text.Equals(string.Empty))    // draw a response box
                    {
                        DrawArrow(currentPos, targetPos, 0.3f);
                        DrawArrow(currentPos, targetPos, 0.75f);
                        Vector2 rectPos = currentPos + (targetPos - currentPos) * 0.5f;
                        rectPos -= RESPONSE_NODE_DIM / 2;
                        Rect responseRect = new Rect(rectPos + (Vector2) RESPONSE_NODE_DIM * (1 - Zoom) / 2,
                            (Vector2) RESPONSE_NODE_DIM * Zoom);
                        string rCutText = response.Text.Cut(ResponseTextLength);
                        GUI.Box(responseRect, $"{rCutText}", _responseNodeStyle);
                    }
                    else                        // draw only an arrow
                    {
                        DrawArrow(currentPos, targetPos, 0.5f);
                    }
                }
            }
            EndWindows();

            // stop connecting if base node deleted
            if (ConnectingFrom != null && ConnectingFrom.IsDeleted)
            {
                IsConnecting = false;
            }

            // draw the current connection line
            if (IsConnecting)
            {
                Vector2 mousePos = Event.current.mousePosition;
                Rect nodeRect = ConnectingFrom.WindowRect;
                Handles.color = LINE_COLOR;
                Handles.DrawLine(nodeRect.center, mousePos);
                Repaint();
            }

            // draw the top panel
            EditorGUILayout.BeginHorizontal();
            {
                // draw the box
                EditorGUI.DrawRect(new Rect(0, 0, position.width, TOP_PANEL_HEIGHT), DefaultColor);

                // title
                EditorGUI.LabelField(TitleTextRect, new GUIContent($"{CurrentDialogue.Title}"), _titleLabelStyle);

                // "add node" button
                if (GUI.Button(AddNodeButtonRect, "+", _addButtonStyle))
                {
                    AddNode();
                }

                // "delete node" button if a node is selected
                if (SelectedNode != null)
                    if (GUI.Button(DeleteNodeButtonRect, "-", _deleteButtonStyle))
                        RemoveNode(SelectedNode);

                // scale slider
                GUI.Label(ScaleLabelRect, new GUIContent($"Zoom: {RoundedZoom}x"), EditorStyles.boldLabel);
                Zoom = GUI.HorizontalSlider(ScaleSliderRect, Zoom, SCALE_MIN, SCALE_MAX);

                // position value
                GUI.Label(PositionLabelRect, new GUIContent($"Position:"), EditorStyles.boldLabel);
                GUI.Label(PositionValueRect, new GUIContent($"({Mathf.Round(Pan.x)}, {Mathf.Round(Pan.y)})"),
                    EditorStyles.centeredGreyMiniLabel);

                // "reset" button
                if (GUI.Button(ResetButtonRect, "R", _resetButtonStyle))
                {
                    Zoom = 1f;
                    Pan = Vector2.zero;
                }
            }
            EditorGUILayout.EndHorizontal();

            // stop connecting if context click not on a node
            if (Event.current.type == EventType.ContextClick)
            {
                IsConnecting = false;
                Event.current.Use();
            }

            // deselect the current node if non-context click not on a node
            if (Event.current.type == EventType.MouseDown)
            {
                DeselectNode();
                Event.current.Use();
            }

            // panning the graph
            if (Event.current.type == EventType.MouseDrag)
            {
                Pan += current.delta;
                Event.current.Use();
                Repaint();
            }

            // zooming the graph
            if (Event.current.type == EventType.ScrollWheel)
            {
                Zoom -= Event.current.delta.y / 50f;
                Zoom = Mathf.Clamp(Zoom, SCALE_MIN, SCALE_MAX);
                Event.current.Use();
                Repaint();
            }
        }

        // add a node to the current dialogue
        private void AddNode()
        {
            // create the node
            Node node = new Node {Rect = NewNodeRect, Sentence = {Actor = CurrentDialogue.DefaultActor}};
            
            // get a soft deleted node's ID
            int softDeletedId = Nodes.FindIndex(n => n.IsDeleted);
            
            // select an index for the node
            if (softDeletedId != -1)            // replace if soft deleted node found
            {
                node.Id = softDeletedId;
                Nodes[softDeletedId] = node;
            }
            else                                // create new if no soft deleted nodes found
            {
                node.Id = Nodes.Count;
                Nodes.Add(node);
            }

            // select this node
            SelectNode(node);
        }

        // remove a node form the current dialogue
        private void RemoveNode(Node node)
        {
            // remove this node's responses
            node.Sentence.Responses.Clear();

            // remove responses linked to this node
            List<Node> sentences = new List<Node>();
            List<Response> responses = new List<Response>();
            foreach (Node s in Nodes)
                foreach (Response r in s.Sentence.Responses)
                    if (node.Equals(Nodes[r.NextId]))
                    {
                        sentences.Add(s);
                        responses.Add(r);
                    }
            for (int i = 0; i < sentences.Count; i++)
                sentences[i].Sentence.Responses.Remove(responses[i]);

            // deselect the node
            DeselectNode();

            // delete (soft if node is not at the end of the list)
            if (node.Id == Nodes.Count - 1)
                Nodes.Remove(node);
            else
                node.SoftDelete();
        }

        // for making sentence windows (windowId is necessary)
        private void MakeNode(int windowId)
        {
            GUI.Label(new Rect(5, 4, 20, 20), $"{windowId}");
            GUI.DragWindow();
        }

        // select / deselect a node
        private void SelectNode(Node node)
        {
            SelectedNode = node;
            NodeEditorWindow.Instance.SelectedNode = node;
        }
        private void DeselectNode() => SelectNode(null);

        // draws an arrow at inter interpolation between v1 and v2
        private void DrawArrow(Vector2 v1, Vector2 v2, float inter)
        {
            Vector2 dirVector = v2 - v1;
            Vector2 dirUnitVector = dirVector.normalized;
            Vector2 pDirUnitVector = new Vector2(-dirUnitVector.y, dirUnitVector.x).normalized;
            Vector2 triangleHeight = dirUnitVector * ARROW_HEAD_SIZE;
            Vector2 arrowPos = v1 + dirVector * inter;
            Vector2 triangleBase = new Vector2(arrowPos.x - triangleHeight.x, arrowPos.y - triangleHeight.y);
            Vector3[] vertices = {
                arrowPos,
                new Vector3(triangleBase.x + ARROW_HEAD_SIZE / 2 * pDirUnitVector.x, triangleBase.y + ARROW_HEAD_SIZE / 2 * pDirUnitVector.y, 0),
                new Vector3(triangleBase.x - ARROW_HEAD_SIZE / 2 * pDirUnitVector.x, triangleBase.y - ARROW_HEAD_SIZE / 2 * pDirUnitVector.y, 0),
            };
            Handles.color = ARROW_COLOR;
            Handles.DrawAAConvexPolygon(vertices);
        }
        
        // Prepares textures and styles for the GUI
        private void PrepareStyles()
        {
            // sentence node textures
            
            var endNodeTexture = new Texture2D(SENTENCE_NODE_DIM.x, SENTENCE_NODE_DIM.y);
            var startNodeTexture = new Texture2D(SENTENCE_NODE_DIM.x, SENTENCE_NODE_DIM.y);
            var defaultNodeTexture = new Texture2D(SENTENCE_NODE_DIM.x, SENTENCE_NODE_DIM.y);
            for (int y = 0; y < SENTENCE_NODE_DIM.y; y++)
            {
                for (int x = 0; x < SENTENCE_NODE_DIM.x; x++)
                {
                    if (x < TEXTURE_COLOR_PADDING || y < TEXTURE_COLOR_PADDING || x > SENTENCE_NODE_DIM.x - TEXTURE_COLOR_PADDING || y > SENTENCE_NODE_DIM.y - TEXTURE_COLOR_PADDING)
                    {
                        defaultNodeTexture.SetPixel(x, y, new Color(1f / 100, 1f / 100, 1f / 100) * (50 + 0.6f * y));
                        startNodeTexture.SetPixel(x, y, new Color(0, 1f / 100, 0) * (50 + 0.6f * y));
                        endNodeTexture.SetPixel(x, y, new Color(1f / 100, 0, 0) * (50 + 0.6f * y));
                    }
                    else
                    {
                        defaultNodeTexture.SetPixel(x, y, new Color(1f / 100, 1f / 100, 1f / 100) * (10 + 0.4f * y));
                        startNodeTexture.SetPixel(x, y, new Color(1f / 100, 1f / 100, 1f / 100) * (10 + 0.4f * y));
                        endNodeTexture.SetPixel(x, y, new Color(1f / 100, 1f / 100, 1f / 100) * (10 + 0.4f * y));
                    }

                }
            }
            endNodeTexture.Apply();
            startNodeTexture.Apply();
            defaultNodeTexture.Apply();

            // response node textures
            
            var responseNodeTexture = new Texture2D(RESPONSE_NODE_DIM.x, RESPONSE_NODE_DIM.y);
            for (int y = 0; y < RESPONSE_NODE_DIM.y; y++)
            {
                for (int x = 0; x < RESPONSE_NODE_DIM.x; x++)
                {
                    if (x < TEXTURE_COLOR_PADDING || y < TEXTURE_COLOR_PADDING || x > RESPONSE_NODE_DIM.x - TEXTURE_COLOR_PADDING || y > RESPONSE_NODE_DIM.y - TEXTURE_COLOR_PADDING)
                    {
                        responseNodeTexture.SetPixel(x, y, new Color(1f / 100, 1f / 100, 1f / 100) * (30 + 0.65f * y));
                    }
                    else
                    {
                        responseNodeTexture.SetPixel(x, y, new Color(1f / 100, 1f / 100, 1f / 100) * (10 + 0.4f * y));
                    }

                }
            }
            responseNodeTexture.Apply();

            // button textures
            
            var addButtonTexture = new Texture2D(BUTTON_DIM.x, BUTTON_DIM.y);
            var deleteButtonTexture = new Texture2D(BUTTON_DIM.x, BUTTON_DIM.y);
            var resetButtonTexture = new Texture2D(BUTTON_DIM.x, BUTTON_DIM.y);
            for (int y = 0; y < BUTTON_DIM.y; y++)
            {
                for (int x = 0; x < BUTTON_DIM.x; x++)
                {
                    if (x < TEXTURE_COLOR_PADDING || y < TEXTURE_COLOR_PADDING || x > BUTTON_DIM.x - TEXTURE_COLOR_PADDING || y > BUTTON_DIM.y - TEXTURE_COLOR_PADDING)
                    {
                        addButtonTexture.SetPixel(x, y, new Color(0, 1f / 100, 0) * (50 + 0.9f * y));
                        deleteButtonTexture.SetPixel(x, y, new Color(1f / 100, 0, 0) * (50 + 0.9f * y));
                        resetButtonTexture.SetPixel(x, y, new Color(1f / 100, 1f / 100, 1f / 100) * (50 + 0.9f * y));
                    }
                    else
                    {
                        addButtonTexture.SetPixel(x, y, new Color(1f / 100, 1f / 100, 1f / 100) * (10 + 0.5f * y));
                        deleteButtonTexture.SetPixel(x, y, new Color(1f / 100, 1f / 100, 1f / 100) * (10 + 0.5f * y));
                        resetButtonTexture.SetPixel(x, y, new Color(1f / 100, 1f / 100, 1f / 100) * (10 + 0.5f * y));
                    }

                }
            }
            addButtonTexture.Apply();
            deleteButtonTexture.Apply();
            resetButtonTexture.Apply();

            // base GUI styles

            var buttonStyle = new GUIStyle
            {
                fontSize = 20,
                alignment = TextAnchor.MiddleCenter,
                fontStyle = FontStyle.Bold,
                normal = {textColor = LIGHT_TEXT_COLOR}
            };

            var nodeStyle = new GUIStyle
            {
                fontSize = 11,
                alignment = TextAnchor.MiddleCenter,
                fontStyle = FontStyle.Normal,
                padding = new RectOffset(7, 7, 7, 7),
                normal = {textColor = LIGHT_TEXT_COLOR}
            };

            _titleLabelStyle = new GUIStyle
            {
                fontSize = 17,
                alignment = TextAnchor.MiddleLeft,
                fontStyle = FontStyle.BoldAndItalic,
                normal = {textColor = LIGHT_TEXT_COLOR}
            };

            // add textures to styles

            _addButtonStyle = new GUIStyle(buttonStyle) {normal = {background = addButtonTexture}};
            _deleteButtonStyle = new GUIStyle(buttonStyle) {normal = {background = deleteButtonTexture}};
            _resetButtonStyle = new GUIStyle(buttonStyle) {normal = {background = resetButtonTexture}};
            _sentenceNodeStyle = new GUIStyle(nodeStyle) {normal = {background = defaultNodeTexture}};
            _startNodeStyle = new GUIStyle(nodeStyle) {normal = {background = startNodeTexture}};
            _endNodeStyle = new GUIStyle(nodeStyle) {normal = {background = endNodeTexture}};
            _responseNodeStyle = new GUIStyle(nodeStyle) {normal = {background = responseNodeTexture}};
        }
    }
}
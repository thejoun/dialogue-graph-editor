using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

namespace DialogueSystem
{
    /// <summary>
    /// Custom dialogue graph editor window
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
        private Color DefaultColor => EditorGUIUtility.isProSkin ? new Color32(56, 56, 56, 255) : new Color32(194, 194, 194, 255);

        private const int SENTENCE_TEXT_LENGTH = 18;
        private const int RESPONSE_TEXT_LENGTH = 14;

        private const float SCALE_MIN = 0.6f;
        private const float SCALE_MAX = 1.2f;


        private GUIStyle titleLabelStyle;
        private GUIStyle deleteButtonStyle;
        private GUIStyle addButtonStyle;
        private GUIStyle sentenceNodeStyle;
        private GUIStyle startNodeStyle;
        private GUIStyle endNodeStyle;
        private GUIStyle responseNodeStyle;
        private GUIStyle resetButtonStyle;


        private Rect BoxRect => new Rect(0, TOP_PANEL_HEIGHT, position.width, Mathf.Max(0, position.height - TOP_PANEL_HEIGHT));
        private Rect AddNodeButtonRect => new Rect(ADD_BUTTON_POS, BUTTON_DIM);
        private Rect DeleteNodeButtonRect => new Rect(DELETE_BUTTON_POS, BUTTON_DIM);
        private Rect NewNodeRect => new Rect((BoxRect.center - SENTENCE_NODE_DIM / 2 - new Vector2(Pan.x, Pan.y)) / Zoom, SENTENCE_NODE_DIM);
        private Rect TitleTextRect => new Rect(500, 10, 300, 30);
        private Rect ScaleLabelRect => new Rect(150, 7, 100, 20);
        private Rect ScaleSliderRect => new Rect(150, 25, 150, 20);
        private Rect PositionLabelRect => new Rect(325, 7, 55, 20);
        private Rect PositionValueRect => new Rect(325, 25, 55, 20);
        private Rect ResetButtonRect => new Rect(new Vector2(405, 10), BUTTON_DIM);


        private bool IsConnecting { get; set; } = false;                // if we are connecting nodes              
        private Node ConnectingFrom { get; set; } = null;               // node that we are connecting from


        private Dialogue Dialogue { get; set; } = null;                 // current open dialogue
        private List<Node> Nodes => Dialogue.Nodes;             // get all nodes
        private Node SelectedNode { get; set; } = null;                 // currently selected node


        private float Zoom { get; set; } = 1;                           // current graph zoom
        private float RoundedZoom => Mathf.Round(Zoom * 100f) / 100f;   // get the rounded zoom
        private Vector2 Pan { get; set; } = Vector2.zero;               // current graph position (panning)


        private int SentenceTextLength => Mathf.RoundToInt(SENTENCE_TEXT_LENGTH * Zoom);    // modified max sentence display length
        private int ResponseTextLength => Mathf.RoundToInt(RESPONSE_TEXT_LENGTH * Zoom);    // modified max response display length


        public Dialogue GetDialogue() => Dialogue;
        public SerializedObject GetSerializedDialogue() => new SerializedObject(Dialogue);
        public int GetSelectedID() => Nodes.IndexOf(SelectedNode);


        public static GraphEditorWindow Instance => (GraphEditorWindow)GetWindow(typeof(GraphEditorWindow));


        // Open the window
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

        // Opens a dialogue
        public static GraphEditorWindow Open(Dialogue dialogue)
        {
            var w = Open();
            w.Dialogue = dialogue;
            EditorUtility.SetDirty(dialogue);
            return w;
        }

        // when the window is enabled
        private void OnEnable()
        {
            
        }

        // When an asset is selected in Unity, show it if it's a Dialogue
        private void OnSelectionChange()
        {
            UnityEngine.Object selection = Selection.activeObject;
            if (selection != null && selection.GetType() == typeof(Dialogue))
            {
                Open((Dialogue)selection);
                Repaint();
            }
        }

        // Method called by the Node Inspector when changes are made to the graph
        public static void Refresh()
        {
            var g = Open();
            g.Repaint();
        }

        // Draws the whole GUI
        private void OnGUI()
        {
            // RETURN if no dialogue selected
            if (Dialogue == null)
            {
                EditorGUILayout.BeginVertical();
                EditorGUILayout.LabelField($"Select a Dialogue asset.");
                EditorGUILayout.LabelField($"You can create one with RMB > Create > Dialogue.");
                EditorGUILayout.EndVertical();
                return;
            }

            // Check if textures are loaded
            if(sentenceNodeStyle.normal.background == null)
            {
                PrepareStyles();
                Repaint();
            }

            // MOUSE EVENTS grabbed at the beginning, because they become confusing later
            var current = Event.current;
            var leftClicked = Event.current.type == EventType.MouseDown && current.button == 0;
            var rightClicked = Event.current.type == EventType.MouseDown && current.button == 1;

            // BOX - background behind the graph
            EditorGUI.DrawRect(BoxRect, BOX_COLOR);

            // NODES start here
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
                    node.WindowRect = GUI.Window(node.Id, node.WindowRect, MakeNode, new GUIContent($"{actor}\n{cutText}"), startNodeStyle);
                }
                else if (sentence.Type == Sentence.Variant.End)
                {
                    node.WindowRect = GUI.Window(node.Id, node.WindowRect, MakeNode, $"END", endNodeStyle);
                }
                else
                {
                    node.WindowRect = GUI.Window(node.Id, node.WindowRect, MakeNode, $"{actor}\n{cutText}", sentenceNodeStyle);
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
                                Response response = new Response() { NextId = id, Text = "" };
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
                    // left click on a node - select it and show in node inspector
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
                    if (!response.Text.Equals(string.Empty))
                    {
                        DrawArrow(currentPos, targetPos, 0.3f);
                        DrawArrow(currentPos, targetPos, 0.75f);
                        Vector2 rectPos = currentPos + (targetPos - currentPos) * 0.5f;
                        rectPos -= RESPONSE_NODE_DIM / 2;
                        Rect responseRect = new Rect(rectPos + (Vector2)RESPONSE_NODE_DIM * (1 - Zoom) / 2, (Vector2)RESPONSE_NODE_DIM * Zoom);
                        string rCutText = response.Text.Cut(ResponseTextLength);
                        GUI.Box(responseRect, $"{rCutText}", responseNodeStyle);
                    }
                    else
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

            // draw the CONNECTION LINE
            if (IsConnecting)
            {
                Vector2 mousePos = Event.current.mousePosition;
                Rect nodeRect = ConnectingFrom.WindowRect;
                Handles.color = LINE_COLOR;
                Handles.DrawLine(nodeRect.center, mousePos);
                Repaint();
            }

            // draw the TOP PANEL
            EditorGUILayout.BeginHorizontal();
            {
                // draw the box
                EditorGUI.DrawRect(new Rect(0, 0, position.width, TOP_PANEL_HEIGHT), DefaultColor);

                // title
                EditorGUI.LabelField(TitleTextRect, new GUIContent($"{Dialogue.Title}"), titleLabelStyle);

                // "add node" button
                if (GUI.Button(AddNodeButtonRect, "+", addButtonStyle))
                {
                    AddNode();
                }

                // "delete node" button if a node is selected
                if (SelectedNode != null)
                    if (GUI.Button(DeleteNodeButtonRect, "-", deleteButtonStyle))
                        RemoveNode(SelectedNode);

                // scale slider
                GUI.Label(ScaleLabelRect, new GUIContent($"Zoom: {RoundedZoom}x"), EditorStyles.boldLabel);
                Zoom = GUI.HorizontalSlider(ScaleSliderRect, Zoom, SCALE_MIN, SCALE_MAX);

                // position value
                GUI.Label(PositionLabelRect, new GUIContent($"Position:"), EditorStyles.boldLabel);
                GUI.Label(PositionValueRect, new GUIContent($"({Mathf.Round(Pan.x)}, {Mathf.Round(Pan.y)})"), EditorStyles.centeredGreyMiniLabel);

                // "reset" button
                if (GUI.Button(ResetButtonRect, "R", resetButtonStyle))
                {
                    Zoom = 1f;
                    Pan = Vector2.zero;
                }
            }
            EditorGUILayout.EndHorizontal();

            // Stop connecting if context click not on a node
            if (Event.current.type == EventType.ContextClick)
            {
                IsConnecting = false;
                Event.current.Use();
            }

            // Deselect the current node if non-context click not on a node
            if (Event.current.type == EventType.MouseDown)
            {
                DeselectNode();
                Event.current.Use();
            }

            // Panning the graph
            if (Event.current.type == EventType.MouseDrag)
            {
                Pan += current.delta;
                Event.current.Use();
                Repaint();
            }

            // Zooming the graph
            if (Event.current.type == EventType.ScrollWheel)
            {
                Zoom -= Event.current.delta.y / 50f;
                Zoom = Mathf.Clamp(Zoom, SCALE_MIN, SCALE_MAX);
                Event.current.Use();
                Repaint();
            }
        }

        // Draws an arrow at inter interpolation between v1 and v2
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

        // Adds a node to the current dialogue
        private void AddNode()
        {
            Node node = new Node() { Rect = NewNodeRect };
            node.Sentence.Actor = Dialogue.DefaultActor;

            int id = Nodes.FindIndex(n => n.IsDeleted);

            if (id != -1)
            {
                node.Id = id;
                Nodes[id] = node;
            }
            else
            {
                node.Id = Nodes.Count;
                Nodes.Add(node);
            }

            SelectNode(node);
        }

        // Removes a node form teh current dialogue
        private void RemoveNode(Node node)
        {
            // find node's position
            int id = Nodes.FindIndex(n => n.Equals(node));

            // remove this node's responses
            node.Sentence.Responses.Clear();

            // remove responses linked to this node
            List<Node> sentences = new List<Node>();
            List<Response> responses = new List<Response>();
            foreach (Node s in Nodes)
            {
                foreach (Response r in s.Sentence.Responses)
                {
                    if (node.Equals(Nodes[r.NextId]))
                    {
                        sentences.Add(s);
                        responses.Add(r);
                    }
                }
            }
            for (int i = 0; i < sentences.Count; i++)
            {
                sentences[i].Sentence.Responses.Remove(responses[i]);
            }

            // deselect the node
            DeselectNode();

            // delete (soft if node is not at the end of the list)
            if (node.Id == Nodes.Count - 1)
            {
                Nodes.Remove(node);
            }
            else
            {
                node.SoftDelete();
            }
        }

        // For making sentence windows (windowId is necessary)
        private void MakeNode(int windowId)
        {
            GUI.Label(new Rect(5, 4, 20, 20), $"{windowId}");
            GUI.DragWindow();
        }

        // Select / deselect a node
        private void SelectNode(Node node)
        {
            SelectedNode = node;
            NodeEditorWindow.Instance.SetNode(SelectedNode);
        }
        private void DeselectNode()
        {
            SelectedNode = null;
            NodeEditorWindow.Instance.SetNode(null);
        }


        // Prepares textures and styles for the GUI
        private void PrepareStyles()
        {
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


            var buttonStyle = new GUIStyle()
            {
                fontSize = 20,
                alignment = TextAnchor.MiddleCenter,
                fontStyle = FontStyle.Bold
            };
            buttonStyle.normal.textColor = LIGHT_TEXT_COLOR;

            var nodeStyle = new GUIStyle()
            {
                fontSize = 11,
                alignment = TextAnchor.MiddleCenter,
                fontStyle = FontStyle.Normal,
                padding = new RectOffset(7, 7, 7, 7)
            };
            nodeStyle.normal.textColor = LIGHT_TEXT_COLOR;

            titleLabelStyle = new GUIStyle()
            {
                fontSize = 17,
                alignment = TextAnchor.MiddleLeft,
                fontStyle = FontStyle.BoldAndItalic
            };
            titleLabelStyle.normal.textColor = LIGHT_TEXT_COLOR;


            addButtonStyle = new GUIStyle(buttonStyle);
            addButtonStyle.normal.background = addButtonTexture;

            deleteButtonStyle = new GUIStyle(buttonStyle);
            deleteButtonStyle.normal.background = deleteButtonTexture;

            resetButtonStyle = new GUIStyle(buttonStyle);
            resetButtonStyle.normal.background = resetButtonTexture;


            sentenceNodeStyle = new GUIStyle(nodeStyle);
            sentenceNodeStyle.normal.background = defaultNodeTexture;

            startNodeStyle = new GUIStyle(nodeStyle);
            startNodeStyle.normal.background = startNodeTexture;

            endNodeStyle = new GUIStyle(nodeStyle);
            endNodeStyle.normal.background = endNodeTexture;


            responseNodeStyle = new GUIStyle(nodeStyle);
            responseNodeStyle.normal.background = responseNodeTexture;
        }
    }
}
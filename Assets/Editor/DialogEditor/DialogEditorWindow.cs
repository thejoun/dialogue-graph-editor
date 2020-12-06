using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

public class DialogEditorWindow : EditorWindow
{
    // constants
    private const int START_NODE_INDEX = 0;
    
    private readonly Dimensions SENTENCE_NODE_DIM = new Dimensions(130, 60);
    private readonly Dimensions RESPONSE_NODE_DIM = new Dimensions(70, 40);

    private readonly Dimensions BUTTON_DIM = new Dimensions(50, 28);

    private const int TOP_PANEL_HEIGHT = 50;
    private const int BOTTOM_PANEL_HEIGHT = 100;
    private const int RIGHT_PANEL_WIDTH = 150;

    private const int BOX_PADDING = 0;

    private const float ARROW_HEAD_SIZE = 12f;

    private readonly Color BOX_COLOR = new Color(0.4f, 0.4f, 0.4f);
    private readonly Color LINE_COLOR = Color.cyan;
    private readonly Color ARROW_COLOR = Color.cyan;
    private readonly Color DARK_TEXT_COLOR = new Color(0.1f, 0.1f, 0.1f);
    private readonly Color LIGHT_TEXT_COLOR = new Color(0.9f, 0.9f, 0.9f);



    private Conversation _selectedConversation;
    private Conversation Conversation => _selectedConversation;


    private List<SentenceNode> Nodes => Conversation.Nodes;


    private int _selectedNodeIndex = -1;
    private SentenceNode SelectedNode => (_selectedNodeIndex >= Nodes?.Count || _selectedNodeIndex < 0) ? null : Nodes[_selectedNodeIndex];


    //private string currentSentence = "";
    //private List<Response> currentResponses;


    // dragging
    private bool _draggingNodeLink = false;
    private int _draggingFromIndex = -1;


    // styles
    private GUIStyle titleLabelStyle;
    private GUIStyle deleteNodeButtonStyle;
    private GUIStyle addNodeButtonStyle;
    private GUIStyle sentenceNodeStyle;
    private GUIStyle responseNodeStyle;
    private GUIStyle startNodeStyle;
    private GUIStyle endNodeStyle;


    // calculated rects
    private Rect BoxRect => new Rect(0, TOP_PANEL_HEIGHT, position.width, Mathf.Max(0, position.height - BOTTOM_PANEL_HEIGHT - TOP_PANEL_HEIGHT));
    private Rect AddNodeButtonRect => new Rect(15, 10, 50, 30);
    private Rect DeleteNodeButtonRect => new Rect(80, 10, 50, 30);
    private Rect DefaultStartNodeRect => new Rect(new Vector2(BoxRect.center.x, BoxRect.yMin) - SENTENCE_NODE_DIM / 2 + new Vector2(0, 50), SENTENCE_NODE_DIM);
    private Rect DefaultEndNodeRect => new Rect(new Vector2(BoxRect.center.x, BoxRect.yMax) - SENTENCE_NODE_DIM / 2 + new Vector2(0, -50), SENTENCE_NODE_DIM);
    private Rect NewNodeRect => new Rect(BoxRect.center - SENTENCE_NODE_DIM / 2, SENTENCE_NODE_DIM);


    // other
    private Vector2 _scrollPosition = new Vector2(0, 0);


    [MenuItem("Window/Dialog Editor")]
    public static void ShowWindow()
    {
        var w = GetWindow(typeof(DialogEditorWindow));
        w.titleContent = new GUIContent("Dialog Editor");
    }


    private void OnGUI()
    {
        // STYLES

        //PrepareStyles();

        // TITLE

        //GUILayout.Label("DIALOG EDITOR", EditorStyles.boldLabel);

        if (_selectedConversation == null)
        {
            GUILayout.Label($"Select a Conversation object to begin", EditorStyles.centeredGreyMiniLabel);
            return;
        }
        else
        {
            GUILayout.Label($"{_selectedConversation.name}", titleLabelStyle);
        }

        // BOX

        EditorGUI.DrawRect(BoxRect, BOX_COLOR);

        // NEW NODE BUTTON

        if (GUI.Button(AddNodeButtonRect, "+", addNodeButtonStyle))
        {
           Nodes.Add(new SentenceNode() { Rect = NewNodeRect, Text = "Write the dialog text here." });
        }

        // DELETE NODE BUTTON

        if (SelectedNode != null && !SelectedNode.Type.Equals(SentenceNode.NodeType.Start))
        {
            if (GUI.Button(DeleteNodeButtonRect, "-", deleteNodeButtonStyle))
            {
                RemoveNode(SelectedNode);
            }
        }

        // START NODE, END NODE

        if (Nodes.Count < 1)
        {
            Nodes.Clear();
            Nodes.Add(new SentenceNode() { Rect = NewNodeRect, Text = "Start Node", Type = SentenceNode.NodeType.Start });
        }

        // NODES

        BeginWindows();

        int i = -1;
        int index = 1;

        foreach (SentenceNode node in Nodes)
        {
            i++;

            // create node

            int cutLength = 20;
            string cutText = node.Text;
            if(node.Text.Length > cutLength)
                cutText = node.Text.Substring(0, cutLength - 2) + "...";

            string actor = node.Content.Actor;

            if (node.Type == SentenceNode.NodeType.Start)
            {
                node.Rect = GUI.Window(i, node.Rect, SentenceNodeWindow, $"START", startNodeStyle);
            }
            else if(node.Type == SentenceNode.NodeType.End)
            {
                node.Rect = GUI.Window(i, node.Rect, SentenceNodeWindow, $"END", endNodeStyle);
            }
            else
            {
                node.Rect = GUI.Window(i, node.Rect, SentenceNodeWindow, $"Node {index} \n{actor} \n{cutText}", sentenceNodeStyle);
                index++;
            }

            // handle node interactions

            if (node.Rect.Contains(Event.current.mousePosition))
            {
                if(Event.current.type == EventType.ContextClick)
                {
                    if (_draggingNodeLink)
                    {
                        if(!node.Content.Responses.Exists(r => r.NextId == i))
                        {
                            Response response = new Response() { Text = "Write the response text here.", NextId = i };
                            Nodes[_draggingFromIndex].Content.Responses.Add(response);
                        }
                        _draggingNodeLink = false;
                    }
                    else
                    {
                        _draggingFromIndex = i;
                        _draggingNodeLink = true;
                    }
                    Event.current.Use();
                }
                else if(Event.current.type == EventType.Layout)
                {
                    _selectedNodeIndex = i;
                }
            }

            // draw arrows

            Handles.color = ARROW_COLOR;
            Vector3 currentPos = node.Rect.center;
            foreach(Response response in node.Content.Responses)
            {
                Vector3 targetPos = Nodes[response.NextId].Rect.center;

                Handles.color = LINE_COLOR;
                Handles.DrawLine(currentPos, targetPos);

                Vector3 dirVector = targetPos - currentPos;
                Vector3 dirUnitVector = dirVector.normalized;
                Vector3 pDirUnitVector = new Vector2(-dirUnitVector.y, dirUnitVector.x).normalized;

                Vector3 triangleHeight = dirUnitVector * ARROW_HEAD_SIZE;

                //Vector3 arrowPos = 0.5f * targetPos + 0.5f * currentPos;
                Vector3 arrowPos = currentPos + dirVector - dirUnitVector * 80f;

                Vector3 triangleBase = new Vector2(arrowPos.x - triangleHeight.x, arrowPos.y - triangleHeight.y);
                Vector3[] vertices = {
                   arrowPos,
                   new Vector3(triangleBase.x + (ARROW_HEAD_SIZE / 2 * pDirUnitVector.x), triangleBase.y + (ARROW_HEAD_SIZE / 2 * pDirUnitVector.y), 0),
                   new Vector3(triangleBase.x - (ARROW_HEAD_SIZE / 2 * pDirUnitVector.x), triangleBase.y - (ARROW_HEAD_SIZE / 2 * pDirUnitVector.y), 0),
                };
                Handles.color = ARROW_COLOR;
                Handles.DrawAAConvexPolygon(vertices);
            }
        }

        EndWindows();

        // Draws a line from selected node to the cursor position when creating a new node connection in the graph

        if (_draggingNodeLink)
        {
            Vector2 mousePos = Event.current.mousePosition;
            Rect nodeRect = Nodes[_draggingFromIndex].Rect;
            Handles.color = LINE_COLOR;
            //Handles.DrawLine(new Vector3(mousePos.x, mousePos.y, 0), new Vector3(nodePos.x + NODE_WIDTH / 2, nodePos.y + NODE_HEIGHT / 2, 0));
            Handles.DrawLine(nodeRect.center, mousePos);
            Repaint();
        }

        // End node connection attempt if second left click not over a node

        if (Event.current.type == EventType.ContextClick)
        {
            _draggingNodeLink = false;
            Event.current.Use();
        }

        // If a node is selected in the graph, display its contents at the bottom of the gui window.

        DisplayNodeInfo(SelectedNode);


        // Label to say which node is currently highlighted in the dialog graph

        GUI.Label(new Rect(660, position.height * 0.86f, 100, 70), "Selected Node:");
        GUI.Label(new Rect(660, position.height * 0.89f, 100, 70), _selectedNodeIndex == -1 ? "No node selected." : _selectedNodeIndex == 0 ? "Start Node" : _selectedNodeIndex == 1 ? "End Node" : "Node " + (_selectedNodeIndex - 1));

        // Save the updated dialog graph:

        /*
        if (_selectedConversation != null)
        {
            _selectedConversation.Nodes = Nodes;
        }
        */
    }


    private void SentenceNodeWindow(int id)
    {  
        GUI.DragWindow();    // Enables drag and drop functionality of nodes.
    }

    private void ResponseNodeWindow(int id)
    {
        
    }

    // Displays information about the selected node
    private void DisplayNodeInfo(SentenceNode node)
    {
        if (node == null)
        {
            return;
        }

        var responses = node.Content.Responses;

        // TEXT

        //GUI.Label(new Rect(20, position.height * 0.83f, 200, 30), "Edit the selected node's content:");

        node.Content.Text = GUI.TextArea(new Rect(20, position.height * 0.86f, 150, 70), node.Content.Text);

        // RESPONSES

        /*
        if (_selectedNodeIndex == END_NODE_INDEX)
        {
            return;
        }
        */

        int responsesCount = responses.Count;

        /*
        _scrollPosition = GUI.BeginScrollView(new Rect(220, position.height * 0.83f, 400, 100), _scrollPosition, new Rect(0, 0, responsesCount * 175, 80));
        for (int i = 0; i < responsesCount; i++)
        {
            Response response = responses[i];

            string destination = "Goes to node " + (response.NextId);

            response.Text = GUI.TextArea(new Rect(175 * i, 18, 150, 40), response.Text);
            GUI.Label(new Rect(175 * i, 80, 150, 20), destination);
            response.Trigger = GUI.TextField(new Rect(175 * i, 60, 125, 20), _nodes[_selectedNodeIndex].Content.Responses[i].Trigger);


            // Button for deleting connectons

            if (GUI.Button(new Rect((175 * i) + 130, 59, BUTTON_DIM.X, BUTTON_DIM.Y), "X", deleteNodeButtonStyle))
            {
                responses.Remove(response);
                //responsesCount--;
            }
        }
        GUI.EndScrollView();
        */
    }


    // Removes a specific node in the graph and updates all references to it accordingly
    private void RemoveNode(SentenceNode node)
    {
        /*
        _selectedNodeIndex = -1;
        for (int i = 0; i < Nodes.Count; i++)
        {
            List<Response> dialogResponses = Nodes[i].Content.Responses;
            for (int j = 0; j < dialogResponses.Count; j++)
            {
                if (dialogResponses[j].NextId == index)
                {
                    Nodes[i].Content.Responses.RemoveAt(j);
                    continue;   // Only one response maximum can reference the next node 
                }
            }
        }
        Nodes.RemoveAt(index);
        */

        DeselectNode();
        foreach(SentenceNode n in Nodes)
        {
            foreach(Response r in n.Content.Responses)
            {
                if (node.Equals(Nodes[r.NextId]))
                {
                    n.Content.Responses.Remove(r);
                }
            }
        }
        Nodes.Remove(node);
    }

    
    private void DeselectNode()
    {
        _selectedNodeIndex = -1;
    }


    private void Save(Conversation conversation)
    {

    }

    private void Load(Conversation conversation)
    {
        _selectedConversation = conversation;
    }



    private void OnDisable()
    {
        if (Conversation != null)
        {
            Save(Conversation);
        }
    }

    private void OnSelectionChange()
    {
        var conversation = Selection.GetFiltered(typeof(Conversation), SelectionMode.Assets).FirstOrDefault() as Conversation;

        if (conversation != null)
        {
            Load(conversation);
            PrepareStyles();
        }
    }



    private void PrepareStyles()
    {
        var redNodeTexture = new Texture2D(SENTENCE_NODE_DIM.X, SENTENCE_NODE_DIM.Y);
        var greenNodeTexture = new Texture2D(SENTENCE_NODE_DIM.X, SENTENCE_NODE_DIM.Y);
        var greyNodeTexture = new Texture2D(SENTENCE_NODE_DIM.X, SENTENCE_NODE_DIM.Y);
        int padding = 3;
        for (int y = 0; y < SENTENCE_NODE_DIM.Y; y++)
        {
            for (int x = 0; x < SENTENCE_NODE_DIM.X; x++)
            {
                if(x < padding || y < padding || x > SENTENCE_NODE_DIM.X - padding || y > SENTENCE_NODE_DIM.Y - padding)
                {
                    redNodeTexture.SetPixel(x, y, new Color(1f / 100, 0, 0) * (60 + (0.6f * y)));
                    greenNodeTexture.SetPixel(x, y, new Color(0, 1f / 100, 0) * (60 + (0.6f * y)));
                    greyNodeTexture.SetPixel(x, y, new Color(1f / 100, 1f / 100, 1f / 100) * (60 + (0.6f * y)));
                }
                else
                {
                    redNodeTexture.SetPixel(x, y, new Color(1f / 100, 1f / 100, 1f / 100) * (10 + (0.4f * y)));
                    greenNodeTexture.SetPixel(x, y, new Color(1f / 100, 1f / 100, 1f / 100) * (10 + (0.4f * y)));
                    greyNodeTexture.SetPixel(x, y, new Color(1f / 100, 1f / 100, 1f / 100) * (10 + (0.4f * y)));
                }
                
            }
        }
        redNodeTexture.Apply();
        greenNodeTexture.Apply();
        greyNodeTexture.Apply();


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
            fontSize = 20,
            alignment = TextAnchor.MiddleCenter,
            fontStyle = FontStyle.Italic
        };
        titleLabelStyle.normal.textColor = LIGHT_TEXT_COLOR;

        addNodeButtonStyle = new GUIStyle(buttonStyle);
        addNodeButtonStyle.normal.background = greenNodeTexture;

        deleteNodeButtonStyle = new GUIStyle(buttonStyle);
        deleteNodeButtonStyle.normal.background = redNodeTexture;

        sentenceNodeStyle = new GUIStyle(nodeStyle);
        sentenceNodeStyle.normal.background = greyNodeTexture;

        responseNodeStyle = new GUIStyle(nodeStyle);
        responseNodeStyle.normal.background = greyNodeTexture;

        startNodeStyle = new GUIStyle(nodeStyle);
        startNodeStyle.fontStyle = FontStyle.Bold;
        startNodeStyle.normal.background = greenNodeTexture;

        endNodeStyle = new GUIStyle(nodeStyle);
        endNodeStyle.fontStyle = FontStyle.Bold;
        endNodeStyle.normal.background = redNodeTexture;
    }
}




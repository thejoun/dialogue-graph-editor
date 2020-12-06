using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

/// <summary>
/// Class to encapsulate the dialog graph for a certain character.
/// </summary>
//[ExecuteInEditMode]
public class Conversation : MonoBehaviour
{
    // temporary
    private const string _savePath = "Assets/Conversations";
    private string Path => $"{_savePath}{Id}.json";


    [SerializeField]
    public int Id = 0;

    // List of nodes in format for interpretation by the DialogEditorWindow class
    [SerializeField]
    private List<SentenceNode> _nodes;

    public List<SentenceNode> Nodes
    {
        get => _nodes == null ? new List<SentenceNode>() : _nodes;
        set => _nodes = value;
    }

    public List<Sentence> Sentences
    {
        get
        {
            var nodes = new List<Sentence>();
            _nodes.ForEach(g => nodes.Add(g.Content));
            return nodes;
        }
    }

    // Opens the dialog editor window for this Conversation instance.
    /*
    public void OpenDialogEditorWindow()
    {
        LoadSavedDialog();
        (new DialogEditorWindow()).ShowWindowForCharacter(this);
    }
    */

    /*
    private void Awake()
    {
        LoadSavedDialog();
    }
    */

    // Starts the conversation.
    public void StartConversation()
    {
        // Reference to the class that contains defitions for functions triggered by this conversation
        // In effect, this is the part of the NPCs behaviour.
        IConversable npcRef = gameObject.GetComponentInParent(typeof(IConversable)) as IConversable;
        Debug.Log("Length: " + _nodes.Count);
        DialogManager.Instance.StartConversation(this.Sentences, npcRef);
    }

    // Loads and deserializes the data
    public bool Load()
    {
        if (File.Exists(Path))
        {
            StreamReader reader = new StreamReader(Path);
            string data = reader.ReadToEnd();
            reader.Close();
            _nodes = JsonHelper.FromJson<SentenceNode>(data);
            return true;
        }
        return false;
    }

    // Serializes and saves the data
    public void Save()
    {
        string data = JsonHelper.ToJson(_nodes);
        StreamWriter writer = new StreamWriter(Path, false);
        writer.WriteLine(data);
        writer.Close();
    }
}

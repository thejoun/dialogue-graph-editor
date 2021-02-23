using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace DialogueSystem
{
    /// <summary>
    /// This ScriptableObject represents a dialogue, which is a graph (list of connected sentences).
    /// </summary>
    [Serializable]
    [CreateAssetMenu(fileName = "Dialogue", menuName = "DialogueSystem/Dialogue", order = 1)]
    public class Dialogue : ScriptableObject
    {
        public Dialogue()
        {
            _nodes = new List<Node>();
        }

        [Tooltip("The display name of this dialogue")]
        [SerializeField]
        private string _title;
        public string Title
        {
            get => _title;
            set => _title = value;
        }

        [Tooltip("This actor will be automatically set to newly created sentence nodes")]
        [SerializeField]
        private Actor _defaultActor;
        public Actor DefaultActor
        {
            get => _defaultActor;
            set => _defaultActor = value;
        }

        [Tooltip("The actual list of nodes (sentences)")]
        [SerializeField]
        public List<Node> _nodes;
        public List<Node> Nodes => _nodes;


        // Get all the nodes that are not soft-deleted
        private List<Node> NotDeletedNodes => new List<Node>(_nodes.Where(n => !n.IsDeleted));

        // Get the count of nodes that are not soft-deleted
        public int SentenceCount => Nodes.Count(n => !n.IsDeleted);

        // Get the count of nodes that are soft-deleted.
        public int DeletedCount => Nodes.Count(n => n.IsDeleted);

        // Get the count of all sentence responses
        public int ResponseCount => Nodes.Select(n => n.Sentence.Responses.Count).Sum();

        // Get the count of sentences of a specific type (Start / End / Default)
        public int GetSentenceCount(Sentence.Variant type) => NotDeletedNodes.Count(n => n.Sentence.Type.Equals(type));

        // Get the (first) Start sentence
        public Sentence Start => NotDeletedNodes.Find(n => n.Sentence.Type.Equals(Sentence.Variant.Start)).Sentence;

        // Get sentence of a specified ID
        public Sentence GetSentence(int id) => Nodes[id].Sentence;

        // ToString override
        public override string ToString() => $"Dialogue '{Title}'";
    }
}
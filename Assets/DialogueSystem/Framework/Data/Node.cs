using UnityEngine;
using System;

namespace DialogueSystem
{
    /// <summary>
    /// This is a single node / sentence in a dialogue.
    /// It stores info about its content (Sentence) and its position and size on the graph.
    /// </summary>
    [Serializable]
    public class Node
    {
        private static readonly int default_width = 50;
        private static readonly int default_height = 50;

        public Node()
        {
            _sentence = new Sentence();
            _rect = new Rect(0, 0, default_width, default_height);
        }

        [Tooltip("Node's ID (unique in a dialogue)")]
        [SerializeField]
        private int _id;
        public int Id
        {
            get => _id;
            set => _id = value;
        }

        [Tooltip("Sentence content")]
        [SerializeField]
        public Sentence _sentence;
        public Sentence Sentence
        {
            get => _sentence;
            set => _sentence = value;
        }

        [Tooltip("Unscaled position and dimensions of the node")]
        [SerializeField]
        private Rect _rect;
        public Rect Rect
        {
            private get => _rect != null ? _rect : new Rect(0, 0, default_width, default_height);
            set => _rect = value;
        }

        [Tooltip("A flag specifying if this node has been soft-deleted")]
        [SerializeField]
        private bool _deletedFlag;
        public bool IsDeleted
        {
            get => _deletedFlag;
            private set => _deletedFlag = value;
        }

        // Non-persistent info about the node's display in graph editor window
        public float Scale { get; set; }
        public Vector2 Position { get; set; }
        public Rect WindowRect
        {
            get
            {
                Rect rect = new Rect(Rect);
                rect.position *= Scale;
                rect.position += Position;
                rect.size *= Scale;
                return rect;
            }
            set
            {
                Rect rect = new Rect(value);
                rect.position -= Position;
                rect.position /= Scale;
                rect.size /= Scale;
                _rect = rect;
            }
        }

        // Set this node as soft-deleted
        public void SoftDelete()
        {
            _deletedFlag = true;
        }

        // To Strgin override
        public override string ToString()
        {
            return $"Node {Id}";
        }
    }
}
using UnityEngine;
using System;

namespace DialogueSystem
{
    [Serializable]
    public class Node
    {
        private static readonly int default_width = 50;
        private static readonly int default_height = 50;

        public Node()
        {
            _sentence = new Sentence();
        }


        [SerializeField]
        private int _id;
        public int Id
        {
            get => _id;
            set => _id = value;
        }

        [SerializeField]
        public Sentence _sentence;
        public Sentence Sentence
        {
            get => _sentence;
            set => _sentence = value;
        }

        [SerializeField]
        private Rect _rect = new Rect(0, 0, default_width, default_height);
        public Rect Rect
        {
            private get => _rect != null ? _rect : new Rect(0, 0, default_width, default_height);
            set => _rect = value;
        }

        [SerializeField]
        private bool _deletedFlag;
        public bool IsDeleted
        {
            get => _deletedFlag;
            set => _deletedFlag = value;
        }


        // Only for showing in the graph editor. Not persistent.
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


        public void SoftDelete()
        {
            _deletedFlag = true;
        }


        public override string ToString()
        {
            return $"Node {Id}";
        }
    }
}
using UnityEngine;
using System;
using System.Collections.Generic;

namespace DialogueSystem
{
    /// <summary>
    /// This class represents a player's response to a sentence.
    /// A sentence can have one or multiple of these.
    /// </summary>
    [Serializable]
    public class Response
    {
        [Tooltip("The response's text content")]
        [SerializeField]
        private string _text;
        public string Text
        {
            get => _text;
            set => _text = value;
        }

        [Tooltip("ID of the node that this response leads to")]
        [SerializeField]
        private int _nextId;
        public int NextId
        {
            get => _nextId;
            set => _nextId = value;
        }

        [Tooltip("Conditions for displaying this response")]
        [SerializeField]
        private string _conditions;
        public string Requisites
        {
            get => _conditions;
            set => _conditions = value;
        }

        // Check if this response has no content
        public bool IsEmpty => Text.Equals(string.Empty);
    }
}
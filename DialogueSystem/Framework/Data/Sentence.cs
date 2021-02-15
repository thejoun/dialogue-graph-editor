using System.Collections.Generic;
using System;
using UnityEngine;

namespace DialogueSystem
{
    /// <summary>
    /// This class represents the content of a dialogue node.
    /// It consists of text, an set actor and expression, a list of triggers and a list of responses
    /// There are 3 types (variants) of sentences - Start, Default, End
    /// </summary>
    [Serializable]
    public class Sentence
    {
        [Serializable]
        public enum Variant
        {
            Default, Start, End
        }

        public Sentence()
        {
            _responses = new List<Response>();
            _text = "";
        }

        [Tooltip("The actor that says this sentence")]
        [SerializeField]
        private Actor _actor;
        public Actor Actor
        {
            get => _actor;
            set => _actor = value;
        }

        [Tooltip("The actor's expression")]
        [SerializeField]
        public int _expressionID;
        public int ExpressionID
        {
            get => _expressionID;
            set => _expressionID = value;
        }

        [Tooltip("The text content of this sentence")]
        [SerializeField]
        private string _text;
        public string Text
        {
            get => _text;
            set => _text = value;
        }

        [Tooltip("The sentence's type / variant")]
        [SerializeField]
        private Variant _type;
        public Variant Type
        {
            get => _type;
            set => _type = value;
        }

        [Tooltip("List of triggers")]
        [SerializeField]
        public List<string> _triggers;
        public List<string> Triggers
        {
            get => _triggers == null ? new List<string>() : _triggers;
            set => _triggers = value;
        }

        [Tooltip("List of responses")]
        [SerializeField]
        public List<Response> _responses;
        public List<Response> Responses
        {
            get => _responses;
        }

        // Get the response with a specific ID on the list
        public Response GetResponse(int id) => Responses[id];

        // Get the actor's expression
        public Expression Expression => _actor.GetExpression(_expressionID);

        // Get the first (default) response on the list
        public Response FirstResponse => GetResponse(0);

        // Check if a choice must be made
        public bool HasChoice => Responses.Count > 0 && !FirstResponse.IsEmpty;

        // Check if there is no choice to be made
        public bool HasNoChoice => Responses.Count > 0 && FirstResponse.IsEmpty;

        // Check if there are no further connections
        public bool IsEnd => Responses.Count == 0;
    }
}
using System.Collections.Generic;
using System;
using UnityEngine;

namespace DialogueSystem
{
    [Serializable]
    public class Sentence
    {
        [Serializable]
        public enum SentenceType
        {
            Default, Start, End
        }

        
        public Sentence()
        {
            _responses = new List<Response>();
            _text = "";
        }


        [SerializeField]
        private Actor _actor;
        public Actor Actor
        {
            get => _actor;
            set => _actor = value;
        }

        [SerializeField]
        public int _expressionID;
        public int ExpressionID
        {
            get => _expressionID;
            set => _expressionID = value;
        }

        [SerializeField]
        private string _text;
        public string Text
        {
            get => _text;
            set => _text = value;
        }

        [SerializeField]
        private SentenceType _type;
        public SentenceType Type
        {
            get => _type;
            set => _type = value;
        }

        [SerializeField]
        public List<string> _triggers;
        public List<string> Triggers
        {
            get => _triggers == null ? new List<string>() : _triggers;
            set => _triggers = value;
        }

        [SerializeField]
        private List<string> _effects;
        public List<string> Effects
        {
            get => _effects;
            set => _effects = value;
        }

        [SerializeField]
        public List<Response> _responses;
        public List<Response> Responses
        {
            get => _responses;
        }


        public Response GetResponse(int id) => Responses[id];

        public Response FirstResponse => GetResponse(0);

        public bool HasResponseChoice => Responses.Count != 0 && !FirstResponse.IsEmpty;

        public bool HasNextSentence => Responses.Count == 1 && FirstResponse.IsEmpty;

        public bool IsDialogueEnd => Responses.Count == 0;

        public Expression Expression => _actor.GetExpression(_expressionID);
    }
}
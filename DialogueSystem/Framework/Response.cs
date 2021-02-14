using UnityEngine;
using System;
using System.Collections.Generic;

namespace DialogueSystem
{
    [Serializable]
    public class Response
    {
        [SerializeField]
        private string _text;
        public string Text
        {
            get => _text;
            set => _text = value;
        }

        [SerializeField]
        private int _nextId;
        public int NextId
        {
            get => _nextId;
            set => _nextId = value;
        }

        [SerializeField]
        private string _requisites;
        public string Requisites
        {
            get => _requisites;
            set => _requisites = value;
        }


        public bool IsEmpty => Text.Equals(string.Empty);
    }
}
using System;
using UnityEngine;

namespace DialogueSystem
{
    [Serializable]
    public class Expression
    {
        [SerializeField]
        private string _title;
        public string Title
        {
            get => _title;
            set => _title = value;
        }

        [SerializeField]
        private Sprite _sprite;
        public Sprite Sprite
        {
            get => _sprite;
            set => _sprite = value;
        }
    }
}
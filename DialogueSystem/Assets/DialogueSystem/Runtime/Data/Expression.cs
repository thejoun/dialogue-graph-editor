using System;
using UnityEngine;

namespace DialogueSystem.Runtime.Data
{
    /// <summary>
    /// This class represents a possible expression of an actor.
    /// These are set up in the Actor object.
    /// </summary>
    [Serializable]
    public class Expression
    {
        [Tooltip("The expression's name / short description")]
        [SerializeField]
        private string _title;
        public string Title
        {
            get => _title;
            set => _title = value;
        }

        [Tooltip("Sprite of the actor with this expression")]
        [SerializeField]
        private Sprite _sprite;
        public Sprite Sprite
        {
            get => _sprite;
            set => _sprite = value;
        }
    }
}
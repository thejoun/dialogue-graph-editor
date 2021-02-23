using UnityEngine;
using System;
using System.Collections.Generic;

namespace DialogueSystem
{
    /// <summary>
    /// This is a ScriptableObject representing an actor.
    /// Actors have a name and several Expressions, which are basically just different sprites / images.
    /// They are set for each Sentence in a Dialogue.
    /// </summary>
    [Serializable]
    [CreateAssetMenu(fileName = "Actor", menuName = "DialogueSystem/Actor", order = 1)]
    public class Actor : ScriptableObject
    {
        public Actor()
        {
            _expressions = new List<Expression>();
        }

        [Tooltip("The actor's name")]
        [SerializeField]
        private string _title;
        public string Title
        {
            get => _title;
            set => _title = value;
        }

        [Tooltip("List of possible expressions")]
        [SerializeField]
        private List<Expression> _expressions;
        public List<Expression> Expressions
        {
            get => _expressions;
            set => _expressions = value;
        }

        // Get the expression with a specified ID
        public Expression GetExpression(int id) => _expressions[id];
    }
}
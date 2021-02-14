using UnityEngine;
using System;
using System.Collections.Generic;

namespace DialogueSystem
{
    [Serializable]
    [CreateAssetMenu(fileName = "Actor", menuName = "Dialogues/Actor", order = 1)]
    public class Actor : ScriptableObject
    {
        public Actor()
        {
            _expressions = new List<Expression>();
        }


        [SerializeField]
        private string _title;
        public string Title
        {
            get => _title;
            set => _title = value;
        }

        [SerializeField]
        private List<Expression> _expressions;
        public List<Expression> Expressions
        {
            get => _expressions;
            set => _expressions = value;
        }


        public Expression GetExpression(int id) => _expressions[id];
    }
}
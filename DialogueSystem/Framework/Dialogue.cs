using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace DialogueSystem
{
    [Serializable]
    [CreateAssetMenu(fileName = "Dialogue", menuName = "Dialogues/Dialogue", order = 1)]
    public class Dialogue : ScriptableObject
    {
        [SerializeField]
        private string _title;
        public string Title
        {
            get => _title;
            set => _title = value;
        }

        [SerializeField]
        private Actor _defaultActor;
        public Actor DefaultActor
        {
            get => _defaultActor;
            set => _defaultActor = value;
        }


        [SerializeField]
        private int _id;
        public int Id
        {
            get => _id;
            set => _id = value;
        }


        [SerializeField]
        public List<Node> _sentenceNodes;
        public List<Node> SentenceNodes
        {
            get
            {
                if (_sentenceNodes == null)
                    _sentenceNodes = new List<Node>();
                return _sentenceNodes;
            }
        }



        public override string ToString()
        {
            return $"Conversation {_id}";
        }

        private List<Node> NotDeletedNodes => new List<Node>(_sentenceNodes.Where(n => !n.IsDeleted));

        public int SentenceCount => SentenceNodes.Count(n => !n.IsDeleted);

        public int DeletedCount => SentenceNodes.Count(n => n.IsDeleted);

        public int ResponseCount
        {
            get
            {
                int sum = 0;
                SentenceNodes.ForEach(s => sum += s.Sentence.Responses.Count);
                return sum;
            }
        }

        public int GetCountOfType(Sentence.SentenceType type)
        {
            return NotDeletedNodes.Count(n => n.Sentence.Type.Equals(type));
        }


        public Sentence Start => NotDeletedNodes.Find(n => n.Sentence.Type.Equals(Sentence.SentenceType.Start)).Sentence;

        public Sentence GetSentence(int id) => SentenceNodes[id].Sentence;
    }
}
﻿using UnityEngine;

namespace DialogueSystem
{
    public class DialogueController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField]
        private SentenceView sentenceView;
        [SerializeField]
        private TriggerHandler triggerHandler;

        [Header("Settings")]
        public float TypingSpeed;


        public static DialogueController Instance { get; private set; }


        // internal state
        private Dialogue _currentDialogue;
        private Sentence _currentSentence;



        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            sentenceView.HideImmediately();
        }


        // ------------------------------------------- PUBLIC METHODS

        public void StartDialogue(Dialogue dialogue)
        {
            if (_currentDialogue != null)
            {
                Debug.Log("A dialogue is already running, can't start a new one");
                return;
            }

            _currentDialogue = dialogue;
            ShowSentence(_currentDialogue.Start);
        }

        public void ChooseResponse(Response response)
        {
            sentenceView.HideResponses();
            ShowSentence(_currentDialogue.GetSentence(response.NextId));
        }

        public void ToNextSentence()
        {
            if (_currentSentence.HasNextSentence)
                ShowSentence(_currentDialogue.GetSentence(_currentSentence.FirstResponse.NextId));
            else
                ShowSentence(null);
        }

        public void OnTypingFinished()
        {
            if (_currentSentence.HasResponseChoice)
                sentenceView.ShowResponses();
            else
                sentenceView.WaitForClick();
        }


        // ----------------------------------------------- PRIVATE METHODS

        private void ShowSentence(Sentence sentence)
        {
            bool end = sentence == null || sentence.Type == Sentence.SentenceType.End;
            bool enter = _currentSentence == null;
            bool sameActor = _currentSentence?.Actor == sentence.Actor;

            sentenceView.Sentence = sentence;

            PlayTriggers(sentence);

            if (end)
            {
                sentenceView.Hide();
                EndDialogue();
                return;
            }
            else if(enter)
            {
                sentenceView.Enter();
            }
            else if (sameActor)
            {
                sentenceView.SameActor();
            }
            else if(!sameActor)
            {
                sentenceView.DifferentActor();
            }

            _currentSentence = sentence;
        }

        private void EndDialogue()
        {
            _currentDialogue = null;
            _currentSentence = null;
        }

        private void PlayTriggers(Sentence sentence)
        {
            sentence.Triggers.ForEach(t => triggerHandler.Handle(t));
        }
        
    }
}
using System;
using System.Collections.Generic;
using DialogueSystem.Runtime.Data;
using DialogueSystem.Runtime.Logic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace DialogueSystem.Runtime.View
{
    /// <summary>
    /// View for sentences - main UI panel.
    /// Shows sentences and accepts clicks.
    /// </summary>
    public class SentenceView : MonoBehaviour, IPointerClickHandler
    {
        [Header("References")]
        public List<Tweener> tweeners;
        public TextTyper typer;

        [Header("Composition")]
        public TextMeshProUGUI titleUi;
        public TextMeshProUGUI textUi;
        public Image imageUi;
        public RectTransform responsesUi;

        [Header("Prefabs")]
        public GameObject responsePanelPrefab;


        public Sentence Sentence { set { _sentence = value; } }


        private DialogueController Controller => DialogueController.Instance;
        private Action ClickCallback => Controller.ToNextSentence;
        private Action TypingCallback => Controller.OnTypingFinished;


        private Sentence _sentence;
        private bool _waitingForClick;
        private List<ResponseView> _responseViews;


        // apply set data and start showing text
        private void Apply()
        {
            typer.Begin(_sentence.Text, Controller.TypingSpeed, textUi, TypingCallback);
            titleUi.text = _sentence.Actor.Title;
            imageUi.sprite = _sentence.Expression.Sprite;
        }

        // when a new dialogue begins - show everything
        public void Enter()
        {
            Apply();
            tweeners.ForEach(t => t.Show());
        }

        // when a new sentence is shown with the same actor - only apply
        public void SameActor()
        {
            Apply();
        }

        // when a new sentence starts with a different actor - re-enter everything
        public void DifferentActor()
        {
            tweeners.ForEach(t => t.ReEnter());
            LeanTween.delayedCall(tweeners[0].HideTime, Apply);
        }

        // when the dialogue ends - hide everything
        public void Hide()
        {
            tweeners.ForEach(t => t.Hide());
        }

        // immediately set everything as hidden
        public void HideImmediately()
        {
            tweeners.ForEach(t => t.HideImmediately());
        }

        // tell the view that it should accept clicks now
        public void WaitForClick()
        {
            _waitingForClick = true;
        }

        // show the sentence's responses
        public void ShowResponses()
        {
            _responseViews = new List<ResponseView>();
            for (int i = 0; i < _sentence.Responses.Count; i++)
            {
                Response response = _sentence.Responses[i];
                GameObject go = Instantiate(responsePanelPrefab, responsesUi);
                if (go.TryGetComponent<ResponseView>(out var r))
                {
                    r.Response = response;
                    r.Show(i);
                    _responseViews.Add(r);
                }
            }
        }

        // hide all the shown responses
        public void HideResponses()
        {
            for (int i = 0; i < _responseViews.Count; i++)
            {
                ResponseView r = _responseViews[i];
                r.Hide(i);
            }
        }

        // when the panel is clicked
        public void OnPointerClick(PointerEventData eventData)
        {
            if (typer.Text != string.Empty && !typer.Finished)
            {
                typer.Finish();
            }
            else if (_waitingForClick)
            {
                _waitingForClick = false;
                ClickCallback?.Invoke();
            }
        }
    }
}
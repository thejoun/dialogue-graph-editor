using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.EventSystems;
using System.Linq;

namespace DialogueSystem
{
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


        // shortcuts
        private DialogueController Controller => DialogueController.Instance;
        private Action ClickCallback => Controller.ToNextSentence;
        private Action TypingCallback => Controller.OnTypingFinished;


        // internal state
        public Sentence Sentence;
        private bool _waitingForClick;
        private List<ResponseView> _responseViews;


        private void Apply()
        {
            typer.Begin(Sentence.Text, Controller.TypingSpeed, textUi, TypingCallback);
            titleUi.text = Sentence.Actor.Title;
            imageUi.sprite = Sentence.Expression.Sprite;
        }

        public void Enter()
        {
            Apply();
            tweeners.ForEach(t => t.Show());
        }
        public void SameActor()
        {
            Apply();
        }
        public void DifferentActor()
        {
            tweeners.ForEach(t => t.ReEnter());
            LeanTween.delayedCall(tweeners[0].HideTime, Apply);
        }

        public void Hide()
        {
            tweeners.ForEach(t => t.Hide());
        }
        public void HideImmediately()
        {
            tweeners.ForEach(t => t.HideImmediately());
        }

        public void WaitForClick()
        {
            _waitingForClick = true;
        }

        public void ShowResponses()
        {
            _responseViews = new List<ResponseView>();
            for (int i = 0; i < Sentence.Responses.Count; i++)
            {
                Response response = Sentence.Responses[i];
                GameObject go = Instantiate(responsePanelPrefab, responsesUi);
                if (go.TryGetComponent<ResponseView>(out var r))
                {
                    r.Response = response;
                    r.Show(i);
                    _responseViews.Add(r);
                }
            }
        }
        public void HideResponses()
        {
            for (int i = 0; i < _responseViews.Count; i++)
            {
                ResponseView r = _responseViews[i];
                r.Hide(i);
            }
        }

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
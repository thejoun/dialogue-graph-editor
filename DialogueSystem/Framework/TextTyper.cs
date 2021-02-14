using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace DialogueSystem
{
    public class TextTyper : MonoBehaviour
    {
        public string Text { get; set; }
        public float Speed { get; set; }
        public TextMeshProUGUI TextUi { get; set; }
        public Action Callback { get; set; }


        private float _timer;


        private int Length => Text.Length;
        private float TypingTime => Length / Speed;
        private float Progress => _timer / TypingTime;
        private int TypedLength => Mathf.Min(Length, Mathf.RoundToInt(Length * Progress));
        private string TypedText => Text.Substring(0, TypedLength);
        public bool Finished => Progress >= 1;


        public void Begin(string text, float speed, TextMeshProUGUI ui, Action callback)
        {
            Text = text;
            Speed = speed;
            TextUi = ui;
            Callback = callback;

            StartCoroutine("Type");
        }

        private IEnumerator Type()
        {
            _timer = 0;
            while (!Finished)
            {
                _timer += Time.deltaTime;
                TextUi.text = TypedText;
                yield return null;
            }
            Callback?.Invoke();
        }

        public void Finish()
        {
            _timer = TypingTime;
            TextUi.text = TypedText;
        }
    }
}
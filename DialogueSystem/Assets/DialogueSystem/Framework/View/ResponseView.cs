using System;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

namespace DialogueSystem
{
    /// <summary>
    /// View for a response button.
    /// Shows the response and accepts clicks.
    /// The actual UI button must be set to call OnClick().
    /// </summary>
    public class ResponseView : MonoBehaviour
    {
        [Header("References")]
        public RectTransform rect;
        public Tweener tweener;

        [Header("Composition")]
        public TextMeshProUGUI textUi;

        [Header("Values")]
        public float height;
        public float delay;


        public Response Response { get; set; }


        private DialogueController Controller => DialogueController.Instance;
        private Action<Response> Callback => Controller.ChooseResponse;
        private float GetDelay(int nr) => nr * delay;


        private bool _acceptClicks;


        // Show this response as the nr-th in the list
        public void Show(int nr)
        {
            textUi.text = Response.Text;
            tweener.HideImmediately();
            rect.anchoredPosition = rect.anchoredPosition + new Vector2(0, nr * height);
            tweener.Show(GetDelay(nr));
            _acceptClicks = true;
        }

        // Hide this response as the nr-th in the list
        public void Hide(int nr)
        {
            tweener.Hide(GetDelay(nr));
            Destroy(gameObject, tweener.HideTime + GetDelay(nr));
            _acceptClicks = false;
        }

        // Method called by a button click
        public void OnClick()
        {
            if (_acceptClicks)
            {
                Callback?.Invoke(Response);
            }
        }
    }
}
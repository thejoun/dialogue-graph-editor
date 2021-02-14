using System;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

namespace DialogueSystem
{
    public class ResponseView : MonoBehaviour/*, IPointerClickHandler*/
    {
        [Header("References")]
        public RectTransform rect;
        public Tweener tweener;

        [Header("Composition")]
        public TextMeshProUGUI textUi;

        [Header("Values")]
        public float height;
        public float delay;


        // public
        public Response Response { get; set; }


        // shortcuts
        private DialogueController Controller => DialogueController.Instance;
        private Action<Response> Callback => Controller.ChooseResponse;
        private float GetDelay(int nr) => nr * delay;


        // internal state
        private bool _acceptClicks;     // if the panel should be accepting clicks now


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

        public void OnClick()
        {
            if (_acceptClicks)
            {
                Callback?.Invoke(Response);
            }
        }

            /*
        // When the panel is clicked
        public void OnPointerClick(PointerEventData eventData)
        {
            if (_acceptClicks)
            {
                Callback?.Invoke(Response);
            }
        }
            */
    }
}
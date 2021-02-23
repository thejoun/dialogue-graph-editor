using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DialogueSystem
{
    /// <summary>
    /// Universal tweener for UI panels.
    /// Can tween X position, Y position and alpha (transparency)
    /// </summary>
    public class Tweener : MonoBehaviour
    {
        [Header("References")]
        [SerializeField]
        [Tooltip("Set this if you want to tween a UI object")]
        private RectTransform rect;
        [SerializeField]
        [Tooltip("Set this if you want to tween the object's alpha (transparency)")]
        private CanvasGroup canvasGroup;

        [Header("Settings")]
        [SerializeField]
        [Tooltip("If the objects should move in the X axis")]
        private bool moveInX;
        [SerializeField]
        [Tooltip("If the objects should move in the Y axis")]
        private bool moveInY;
        [SerializeField]
        [Tooltip("If the object's alpha (transparency) should be tweened")]
        private bool tweenAlpha;

        [Header("Values")]
        [SerializeField]
        [Tooltip("Position at which the object will show (0,0 is recommended)")]
        private Vector2 showPos;
        [SerializeField]
        [Tooltip("Position at which the object will hide")]
        private Vector2 hidePos;
        [SerializeField]
        [Tooltip("Alpha (transparency) value when the object is shown")]
        private float showAlpha;
        [SerializeField]
        [Tooltip("Alpha (transparency) value when the object is hidden")]
        private float hideAlpha;
        [SerializeField]
        [Tooltip("How long it takes for the object to show")]
        private float showTime;
        [SerializeField]
        [Tooltip("How long it takes for the object to hide")]
        private float hideTime;
        [SerializeField]
        [Tooltip("Easing type when the object is being shown")]
        private LeanTweenType showEaseType;
        [SerializeField]
        [Tooltip("Easing type when the object is being hidden")]
        private LeanTweenType hideEaseType;

        public float HideTime => hideTime;

        // Show the panel
        public void Show(float delay)
        {
            LeanTween.cancel(rect);
            if (moveInX)
                LeanTween.moveX(rect, showPos.x, showTime).setEase(showEaseType).setDelay(delay);
            if (moveInY)
                LeanTween.moveY(rect, showPos.y, showTime).setEase(showEaseType).setDelay(delay);
            if (tweenAlpha)
                LeanTween.alphaCanvas(canvasGroup, showAlpha, showTime).setEase(showEaseType).setDelay(delay);
        }
        public void Show()
        {
            Show(0);
        }

        // Hide the panel
        public void Hide(float delay)
        {
            LeanTween.cancel(rect);
            if (moveInX)
                LeanTween.moveX(rect, hidePos.x, hideTime).setEase(hideEaseType).setDelay(delay);
            if (moveInY)
                LeanTween.moveY(rect, hidePos.y, hideTime).setEase(hideEaseType).setDelay(delay);
            if (tweenAlpha)
                LeanTween.alphaCanvas(canvasGroup, hideAlpha, hideTime).setEase(hideEaseType).setDelay(delay);
        }
        public void Hide()
        {
            Hide(0);
        }

        // Hide, invoke callback and then show
        public void ReEnter(float delay)
        {
            Hide(delay);
            LeanTween.delayedCall(hideTime, Show);
        }
        public void ReEnter()
        {
            ReEnter(0);
        }

        // Immediately change to hide state
        public void HideImmediately()
        {
            LeanTween.cancel(rect);
            if (moveInX)
                LeanTween.moveX(rect, hidePos.x, 0);
            if (moveInY)
                LeanTween.moveY(rect, hidePos.y, 0);
            if (tweenAlpha)
                LeanTween.alphaCanvas(canvasGroup, hideAlpha, 0);
        }
    }
}
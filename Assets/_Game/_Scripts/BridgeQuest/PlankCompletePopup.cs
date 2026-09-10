using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TMKOC.BridgeQuest
{
    /// <summary>
    /// The "you got it!" card between questions -- Bridge Quest's ItemCompletePopup.
    ///
    /// Structurally identical to RocketRun's: freeze the world, pop a card over
    /// slowly turning shine rays, and wait for either a tap or an auto-dismiss before
    /// invoking the callback that advances the flow. The auto-dismiss matters more
    /// here than it did in RocketRun -- a three-year-old who does not realise the
    /// card is waiting on them should never be stuck.
    ///
    /// Deliberately shows no score and no counter. The bridge on screen behind this
    /// card is the progress display.
    /// </summary>
    public class PlankCompletePopup : MonoBehaviour
    {
        [Header("Refs")]
        [SerializeField] private GameObject root;

        [Tooltip("The round card, scaled in on show.")]
        [SerializeField] private RectTransform bubble;

        [Tooltip("Shine rays behind the icon, same sprite as the win panel. Turns slowly while the card is up.")]
        [SerializeField] private RectTransform shine;

        [SerializeField] private Image icon;
        [SerializeField] private TextMeshProUGUI label;

        [Header("Timing (unscaled seconds)")]
        [SerializeField] private float popInDuration = 0.3f;

        [Tooltip("Degrees per second the shine rays turn. Negative to spin the other way.")]
        [SerializeField] private float shineSpinSpeed = 10f;

        [Tooltip("How long the card stays up before auto-resuming. A tap dismisses it sooner.")]
        [SerializeField] private float autoDismissDelay = 2.5f;

        [Tooltip("Taps ignored for this long after the card lands, so the tap that answered the question cannot also dismiss the card.")]
        [SerializeField] private float tapGrace = 0.3f;

        private Action onDismissed;
        private bool showing;
        private float previousTimeScale = 1f;

        public bool IsShowing { get { return showing; } }

        private void Awake()
        {
            if (root != null) root.SetActive(false);
        }

        public void Show(string message, Sprite iconSprite, Action onDismissedCallback)
        {
            if (showing)
            {
                if (onDismissedCallback != null) onDismissedCallback();
                return;
            }

            onDismissed = onDismissedCallback;

            if (icon != null)
            {
                icon.sprite = iconSprite;
                icon.enabled = iconSprite != null;
            }
            if (label != null) label.text = message;

            if (root != null) root.SetActive(true);
            showing = true;

            previousTimeScale = Time.timeScale > 0f ? Time.timeScale : 1f;
            Time.timeScale = 0f;

            StopAllCoroutines();
            StartCoroutine(PlayInThenWaitForTap());
        }

        private IEnumerator PlayInThenWaitForTap()
        {
            float t = 0f;
            if (bubble != null) bubble.localScale = Vector3.zero;

            while (t < popInDuration)
            {
                t += Time.unscaledDeltaTime;
                float k = Mathf.Sin(Mathf.Clamp01(t / popInDuration) * Mathf.PI * 0.5f);
                if (bubble != null) bubble.localScale = Vector3.one * k;
                yield return null;
            }
            if (bubble != null) bubble.localScale = Vector3.one;

            float waited = 0f;
            while (waited < autoDismissDelay)
            {
                waited += Time.unscaledDeltaTime;
                if (waited >= tapGrace && TapDetected()) break;
                yield return null;
            }

            Hide();
        }

        private bool TapDetected()
        {
            if (Input.GetMouseButtonDown(0)) return true;
            for (int i = 0; i < Input.touchCount; i++)
            {
                if (Input.GetTouch(i).phase == TouchPhase.Began) return true;
            }
            return false;
        }

        private void Update()
        {
            // unscaled: the world is frozen for as long as this card is up
            if (showing && shine != null)
            {
                shine.Rotate(0f, 0f, -shineSpinSpeed * Time.unscaledDeltaTime);
            }
        }

        private void Hide()
        {
            if (root != null) root.SetActive(false);
            showing = false;
            Time.timeScale = previousTimeScale;

            Action cb = onDismissed;
            onDismissed = null;
            if (cb != null) cb();
        }
    }
}

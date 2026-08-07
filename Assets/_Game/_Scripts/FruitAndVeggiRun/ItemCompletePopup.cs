using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace TMKOC.FruitAndVeggiRun
{

    /// <summary>
    /// Full-screen "you got it!" card shown when a shopping-list item is fully
    /// collected. Freezes the world (Time.timeScale = 0) and waits for a tap
    /// anywhere on screen before resuming play and invoking the completion callback.
    /// </summary>
    public class ItemCompletePopup : MonoBehaviour
    {
        [SerializeField] private GameObject root;       // whole popup, inactive by default
        [SerializeField] private RectTransform bubble;   // the round card, scaled in on show
        [Tooltip("Shine rays behind the icon, same sprite as the win and lose panels. Turns slowly while the card is up.")]
        [SerializeField] private RectTransform shine;
        [SerializeField] private Image icon;
        [SerializeField] private TextMeshProUGUI label;

        [Header("Timing")]
        [Tooltip("Pop-in duration and minimum time before a tap can dismiss the card, in unscaled seconds.")]
        [SerializeField] private float popInDuration = 0.3f;

        [Tooltip("Degrees per second the shine rays turn. Negative to spin the other way.")]
        [SerializeField] private float shineSpinSpeed = 10f;

        [Tooltip("How long the card stays up before auto-resuming, in unscaled seconds. A tap dismisses it sooner.")]
        [SerializeField] private float autoDismissDelay = 3f;

        private Action onDismissed;
        private bool showing;

        private void Awake()
        {
            if (root != null) root.SetActive(false);
        }

        public void Show(string itemName, Sprite iconSprite, Action onDismissedCallback)
        {
            onDismissed = onDismissedCallback;

            if (icon != null) icon.sprite = iconSprite;
            if (label != null)
            {
                label.text = itemName;
                label.color = GameDefs.ColorOf(itemName);
            }

            if (root != null) root.SetActive(true);
            showing = true;
            Time.timeScale = 0f;

            StopAllCoroutines();
            StartCoroutine(PlayInThenWaitForTap());
        }

    private IEnumerator PlayInThenWaitForTap()
        {
            // Unscaled-time pop-in so it still animates while the world is paused.
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

            // Auto-resume after autoDismissDelay, or as soon as the player taps.
            float waited = 0f;
            while (waited < autoDismissDelay && !TapDetected())
            {
                waited += Time.unscaledDeltaTime;
                yield return null;
            }

            Hide();
        }

        private bool TapDetected()
        {
            if (Input.GetMouseButtonDown(0)) return true;
            for (int i = 0; i < Input.touchCount; i++)
                if (Input.GetTouch(i).phase == TouchPhase.Began) return true;
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
            Time.timeScale = 1f;

            Action cb = onDismissed;
            onDismissed = null;
            cb?.Invoke();
        }
    }
}

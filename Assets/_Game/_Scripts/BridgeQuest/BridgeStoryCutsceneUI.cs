using System;
using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TMKOC.BridgeQuest
{
    /// <summary>
    /// A storyboard played as a scrapbook collage: each panel flies in, settles at a
    /// slight angle overlapping the one before, and stays put, so by the end every
    /// beat is on screen together.
    ///
    /// Structurally this is RocketRun's StoryCutsceneUI, with one deliberate change:
    /// the panels are NOT authored on the component. They are passed in per call via
    /// <see cref="Play"/>, so a single instance in the scene serves all twelve Bridge
    /// Quest storyboards (six openings and six endings) straight off MissionData.
    /// RocketRun could hard-place its two because it only ever had two.
    ///
    /// Everything runs on unscaled time because the world is frozen underneath. The
    /// child can tap to hurry to the next panel, or hit Skip to drop straight through.
    /// <see cref="IsPlaying"/> is what BridgeQuestTutorial waits on.
    /// </summary>
    public class BridgeStoryCutsceneUI : MonoBehaviour
    {
        // refcounted rather than a bool: nothing stops a second storyboard being
        // queued while one is closing, and a bool would race on that
        private static int activeCount;

        /// <summary>True while any storyboard is running.</summary>
        public static bool IsPlaying { get { return activeCount > 0; } }

        [Header("Refs")]
        [SerializeField] private GameObject root;
        [SerializeField] private Image backdrop;
        [SerializeField] private RectTransform panelLayer;

        [Tooltip("Pooled card slots. Must be at least as long as the longest storyboard in MissionData.")]
        [SerializeField] private Image[] panelImages;

        [SerializeField] private TextMeshProUGUI captionText;
        [SerializeField] private Button skipButton;

        [Header("Timing (unscaled seconds)")]
        [SerializeField] private float flyInDuration = 0.55f;
        [SerializeField] private float captionFadeDuration = 0.25f;

        [Tooltip("Ignore taps for this long after a panel lands, so one tap cannot skip two panels.")]
        [SerializeField] private float tapGrace = 0.25f;

        [SerializeField] private float outroDuration = 0.4f;

        [Header("Layout")]
        [Tooltip("The design space the panel poses were authored in. The collage is scaled down uniformly so this box always fits inside the real canvas, on any aspect ratio.")]
        [SerializeField] private Vector2 designSize = new Vector2(1920f, 1080f);

        private StoryPanel[] panels;
        private bool skipRequested;
        private bool running;
        private Action onComplete;
        private float previousTimeScale = 1f;

        private void Awake()
        {
            if (root != null) root.SetActive(false);
            ResetPanels();
        }

        private void OnEnable()
        {
            if (skipButton != null) skipButton.onClick.AddListener(RequestSkip);
        }

        private void OnDisable()
        {
            if (skipButton != null) skipButton.onClick.RemoveListener(RequestSkip);
        }

        public void RequestSkip()
        {
            skipRequested = true;
        }

        /// <summary>
        /// Plays a storyboard. Freezes the world for the duration and calls
        /// <paramref name="onCompleteCallback"/> once the cards have faded out --
        /// including when there is nothing to play, so a mission with no ending
        /// storyboard authored yet still advances rather than hanging the flow.
        /// </summary>
        public void Play(StoryPanel[] storyPanels, Action onCompleteCallback)
        {
            if (running)
            {
                // never silently swallow the callback -- the caller is waiting on it
                if (onCompleteCallback != null) onCompleteCallback();
                return;
            }

            if (storyPanels == null || storyPanels.Length == 0)
            {
                if (onCompleteCallback != null) onCompleteCallback();
                return;
            }

            panels = storyPanels;
            onComplete = onCompleteCallback;
            skipRequested = false;
            running = true;
            activeCount++;

            if (root != null)
            {
                root.SetActive(true);
                CanvasGroup cg = root.GetComponent<CanvasGroup>();
                if (cg != null)
                {
                    cg.alpha = 1f;
                    cg.interactable = true;
                    cg.blocksRaycasts = true;
                }
            }

            ResetPanels();

            previousTimeScale = Time.timeScale > 0f ? Time.timeScale : 1f;
            Time.timeScale = 0f;

            FitCollageToCanvas();
            StartCoroutine(PlayStory());
        }

        /// <summary>
        /// Shrinks the collage so the authored 1920x1080 layout always fits the real
        /// canvas. Without this the top card runs off the top of tall-and-narrow
        /// phones and off the sides of 4:3 tablets.
        /// </summary>
        private void FitCollageToCanvas()
        {
            if (panelLayer == null) return;

            RectTransform canvasRT = GetComponent<RectTransform>();
            if (canvasRT == null) return;

            float w = canvasRT.rect.width;
            float h = canvasRT.rect.height;
            if (w <= 0f || h <= 0f || designSize.x <= 0f || designSize.y <= 0f) return;

            float fit = Mathf.Min(w / designSize.x, h / designSize.y);
            panelLayer.localScale = Vector3.one * fit;
        }

        private void ResetPanels()
        {
            if (panelImages != null)
            {
                for (int i = 0; i < panelImages.Length; i++)
                {
                    if (panelImages[i] != null) panelImages[i].gameObject.SetActive(false);
                }
            }

            if (captionText != null)
            {
                captionText.DOKill();
                captionText.alpha = 0f;
            }
        }

        private IEnumerator PlayStory()
        {
            int authored = panels != null ? panels.Length : 0;
            int slots = panelImages != null ? panelImages.Length : 0;
            int shown = Mathf.Min(authored, slots);

            if (authored > slots)
            {
                // silent truncation here would read as "the artist forgot a panel"
                Debug.LogWarning("[BridgeQuest] Storyboard has " + authored
                    + " panels but only " + slots + " card slots -- the tail will not be shown.", this);
            }

            for (int i = 0; i < shown && !skipRequested; i++)
            {
                yield return FlyPanelIn(i);
                if (skipRequested) break;
                yield return WaitOrTap(panels[i].hold);
            }

            yield return Outro();
            Finish();
        }

        private IEnumerator FlyPanelIn(int index)
        {
            StoryPanel p = panels[index];
            Image img = panelImages[index];
            if (img == null) yield break;

            RectTransform rt = img.rectTransform;
            // Only paint when the data actually carries art. Assigning unconditionally
            // wiped whatever the scene had authored on the panel Image the moment a
            // mission left StoryPanel.art empty -- which every mission currently does.
            // Same fallback shape as BridgeBuilderUI.ResetBridge: data wins, scene holds.
            if (p.art != null) img.sprite = p.art;
            img.gameObject.SetActive(true);
            img.transform.SetAsLastSibling(); // newest card sits on top of the pile

            rt.anchoredPosition = p.restPosition + p.fromOffset;
            rt.localRotation = Quaternion.Euler(0f, 0f, p.fromRotation);
            rt.localScale = Vector3.one * (p.restScale * 0.8f);

            // the line is spoken as the card flies in, so narration lands with the picture
            BridgeQuestVoice.Play(p.voiceKey);

            if (captionText != null)
            {
                captionText.DOKill();
                captionText.text = p.caption;
                captionText.alpha = 0f;
                captionText.DOFade(1f, captionFadeDuration).SetUpdate(true);
            }

            Sequence seq = DOTween.Sequence().SetUpdate(true);
            seq.Append(rt.DOAnchorPos(p.restPosition, flyInDuration).SetEase(Ease.OutBack, 1.4f));
            seq.Join(rt.DOLocalRotate(new Vector3(0f, 0f, p.restRotation), flyInDuration).SetEase(Ease.OutBack));
            seq.Join(rt.DOScale(p.restScale, flyInDuration).SetEase(Ease.OutBack));

            while (seq.IsActive() && !seq.IsComplete())
            {
                if (skipRequested)
                {
                    seq.Complete();
                    break;
                }
                yield return null;
            }

            // land it exactly, whatever happened above
            rt.anchoredPosition = p.restPosition;
            rt.localRotation = Quaternion.Euler(0f, 0f, p.restRotation);
            rt.localScale = Vector3.one * p.restScale;
            if (captionText != null) captionText.alpha = 1f;
        }

        private IEnumerator WaitOrTap(float seconds)
        {
            float t = 0f;
            while (t < seconds && !skipRequested)
            {
                t += Time.unscaledDeltaTime;
                if (t >= tapGrace && TapDetected()) yield break;
                yield return null;
            }
        }

        private IEnumerator Outro()
        {
            CanvasGroup cg = root != null ? root.GetComponent<CanvasGroup>() : null;
            if (cg == null)
            {
                yield return new WaitForSecondsRealtime(0.05f);
                yield break;
            }

            cg.interactable = false;
            cg.blocksRaycasts = false;

            float t = 0f;
            while (t < outroDuration)
            {
                t += Time.unscaledDeltaTime;
                cg.alpha = 1f - Mathf.Clamp01(t / outroDuration);
                yield return null;
            }
            cg.alpha = 0f;
        }

        private void Finish()
        {
            if (captionText != null) captionText.DOKill();
            if (root != null) root.SetActive(false);

            Time.timeScale = previousTimeScale;

            if (running)
            {
                running = false;
                activeCount = Mathf.Max(0, activeCount - 1);
            }

            panels = null;

            Action cb = onComplete;
            onComplete = null;
            if (cb != null) cb();
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

        private void OnDestroy()
        {
            // never leave the rest of the game waiting on a storyboard that is gone
            if (running)
            {
                running = false;
                activeCount = Mathf.Max(0, activeCount - 1);
            }
        }
    }
}

using System;
using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TMKOC.FruitAndVeggiRun
{

    /// <summary>
    /// A story cut-scene played as a scrapbook collage on the starry wall backdrop.
    /// Drives both the opening (Pinku is hungry, the fridge is empty, Mum hands him
    /// the shopping list) and the closing (the fridge is full again).
    ///
    /// Presented as a scrapbook collage on the starry wall: each panel flies in,
    /// settles at a slight angle overlapping the one before, and stays put, so by
    /// the end all three beats are on screen together. Everything runs on unscaled
    /// time because the world is frozen (Time.timeScale = 0) underneath.
    ///
    /// The player can tap to hurry to the next panel, or hit Skip to drop straight
    /// into the tutorial. <see cref="IsPlaying"/> is what RocketRunTutorial waits on.
    /// </summary>
    public class StoryCutsceneUI : MonoBehaviour
    {
        // how many cut-scenes are mid-run right now; there are two in the scene (opening
        // and closing) so a plain bool would race on Awake
        private static int activeCount;

        /// <summary>True while any story cut-scene is running.</summary>
        public static bool IsPlaying { get { return activeCount > 0; } }

        [Serializable]
        public class Panel
        {
            public Sprite art;
            [TextArea(1, 2)] public string caption;
            [Tooltip("voiceover_title from the VO sheet, e.g. story_open_1. Left blank, the panel is silent.")]
            public string voiceKey;

            [Header("Resting pose (anchored position / rotation / scale)")]
            public Vector2 restPosition;
            public float restRotation = -3f;
            public float restScale = 0.55f;

            [Header("Fly-in start offset from the resting pose")]
            public Vector2 fromOffset = new Vector2(0f, 900f);
            public float fromRotation = -18f;

            [Tooltip("Seconds this panel stays on screen before the next one flies in.")]
            public float hold = 2.5f;
        }

        [Header("Refs")]
        [SerializeField] private GameObject root;
        [SerializeField] private Image backdrop;
        [SerializeField] private RectTransform panelLayer;
        [SerializeField] private Image[] panelImages;
        [SerializeField] private TextMeshProUGUI captionText;
        [SerializeField] private Button skipButton;

        [Header("Story")]
        [SerializeField] private Panel[] panels;
        [Tooltip("On for the opening cut-scene. Off for the closing one, which GameFlow triggers via Play().")]
        [SerializeField] private bool playOnStart = true;

        [Header("Timing (unscaled seconds)")]
        [SerializeField] private float flyInDuration = 0.55f;
        [SerializeField] private float captionFadeDuration = 0.25f;
        [Tooltip("Ignore taps for this long after a panel lands, so one tap can't skip two panels.")]
        [SerializeField] private float tapGrace = 0.25f;
        [SerializeField] private float outroDuration = 0.4f;

        [Header("Layout")]
        [Tooltip("The design space the panel poses were authored in. The collage is scaled down uniformly so this box always fits inside the real canvas, on any aspect ratio.")]
        [SerializeField] private Vector2 designSize = new Vector2(1920f, 1080f);

        private bool skipRequested;
        private bool running;
        private Coroutine runner;
        private Action onComplete;
        private bool restoreTimeScale;
        private float previousTimeScale = 1f;

    private void Awake()
        {
            if (playOnStart)
            {
                activeCount++;
                running = true;
            }

            if (root != null) root.SetActive(playOnStart);

            ResetPanels();
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


        private void OnEnable()
        {
            if (skipButton != null) skipButton.onClick.AddListener(RequestSkip);
        }

        private void OnDisable()
        {
            if (skipButton != null) skipButton.onClick.RemoveListener(RequestSkip);
        }

    private void Start()
        {
            if (!playOnStart) return;

            // freeze the world for the whole cut-scene; the tutorial takes over the
            // freeze straight after, so we never actually hand control back early
            Time.timeScale = 0f;

            // canvas rect is only valid once layout has run, so fit here rather than in Awake
            FitCollageToCanvas();
            runner = StartCoroutine(PlayStory());
        }

        public void RequestSkip()
        {
            skipRequested = true;
        }

    /// <summary>
        /// Runs the cut-scene on demand -- this is how the closing story gets played
        /// once the shopping list is finished. Freezes the world for the duration and
        /// calls <paramref name="onCompleteCallback"/> once the cards have faded out.
        /// </summary>
        public void Play(Action onCompleteCallback)
        {
            if (running) return;

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

            // unlike the opening -- where the tutorial inherits the freeze -- an
            // on-demand cut-scene has to give the world back exactly as it found it
            previousTimeScale = Time.timeScale > 0f ? Time.timeScale : 1f;
            restoreTimeScale = true;
            Time.timeScale = 0f;

            FitCollageToCanvas();
            runner = StartCoroutine(PlayStory());
        }

        /// <summary>Hides every card and clears the caption, ready for a (re)run.</summary>
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
            int count = panels != null ? panels.Length : 0;
            int shown = Mathf.Min(count, panelImages != null ? panelImages.Length : 0);

            for (int i = 0; i < shown && !skipRequested; i++)
            {
                yield return FlyPanelIn(i);
                if (skipRequested) break;
                yield return WaitOrTap(panels[i].hold);
            }

            yield return Outro();
            Finish();
        }

        /// <summary>Slides one panel in from off its resting pose and fades its caption up.</summary>
        private IEnumerator FlyPanelIn(int index)
        {
            Panel p = panels[index];
            Image img = panelImages[index];
            if (img == null) yield break;

            RectTransform rt = img.rectTransform;
            img.sprite = p.art;
            img.gameObject.SetActive(true);
            img.transform.SetAsLastSibling(); // newest panel sits on top of the pile

            rt.anchoredPosition = p.restPosition + p.fromOffset;
            rt.localRotation = Quaternion.Euler(0f, 0f, p.fromRotation);
            rt.localScale = Vector3.one * (p.restScale * 0.8f);

            // the line is spoken as the card flies in, so the narration lands with the picture
            RocketRunVoice.Play(p.voiceKey);

            // caption swaps while the panel is still in flight
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

        /// <summary>Holds for the given time, cut short by a tap (after a short grace period).</summary>
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

        /// <summary>Fades the whole collage away so the tutorial can start on a clean screen.</summary>
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
            DOTween.Kill(captionText);
            if (root != null) root.SetActive(false);

            if (restoreTimeScale)
            {
                restoreTimeScale = false;
                Time.timeScale = previousTimeScale;
            }

            if (running)
            {
                running = false;
                activeCount = Mathf.Max(0, activeCount - 1);
            }
            runner = null;

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
            // never leave the rest of the game waiting on a cut-scene that's gone
            if (running)
            {
                running = false;
                activeCount = Mathf.Max(0, activeCount - 1);
            }
        }
    }
}

using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace TMKOC.BridgeQuest
{
    /// <summary>
    /// End-of-mission screen. Freezes the world, drops a badge over slowly turning
    /// shine rays, and offers three ways out: next mission, play this one again, or
    /// back to Playschool.
    ///
    /// Unlike RocketRun's EndScreenUI this component has ONE instance, not two.
    /// Bridge Quest is no-fail -- a wrong tap costs nothing and there is no attempt
    /// limit -- so there is no path that reaches a TRY AGAIN badge. If a soft fail is
    /// ever added, this is the class to give a second instance to, exactly as
    /// RocketRun does.
    ///
    /// Replay is an in-place reset (BridgeQuestFlow.RestartMission), not a scene
    /// reload, so the opening storyboard and the tutorial never replay.
    /// </summary>
    public class BridgeQuestEndScreenUI : MonoBehaviour
    {
        [Header("Refs")]
        [SerializeField] private GameObject root;

        [Tooltip("Everything that punches in together: rays, badge and buttons.")]
        [SerializeField] private RectTransform badgeGroup;

        [Tooltip("Shine rays that turn slowly behind the badge.")]
        [SerializeField] private RectTransform shine;

        [Tooltip("Hidden on the final mission, where there is no next.")]
        [SerializeField] private Button nextButton;

        [SerializeField] private Button replayButton;
        [SerializeField] private Button playschoolButton;

        [Header("Voice-over")]
        [Tooltip("voiceover_title from the VO sheet. Blank for silence.")]
        [SerializeField] private string voiceKey = "win_screen";

        [Header("Tuning")]
        [SerializeField] private float popInDuration = 0.45f;

        [Tooltip("Degrees per second the shine rays turn. Negative to spin the other way.")]
        [SerializeField] private float shineSpinSpeed = 10f;

        [Tooltip("Gentle breathing on the badge once it has landed.")]
        [SerializeField] private float idlePulseScale = 0.04f;

        [SerializeField] private float idlePulseDuration = 1.4f;

        private Action onNext;
        private Action onReplay;
        private bool visible;
        private float previousTimeScale = 1f;

        /// <summary>True while this screen is up and waiting on the child.</summary>
        public bool IsShowing { get { return visible; } }

        private void Awake()
        {
            if (root != null) root.SetActive(false);
        }

        private void OnEnable()
        {
            if (nextButton != null) nextButton.onClick.AddListener(OnNextPressed);
            if (replayButton != null) replayButton.onClick.AddListener(OnReplayPressed);
            if (playschoolButton != null) playschoolButton.onClick.AddListener(OnPlayschoolPressed);
        }

        private void OnDisable()
        {
            if (nextButton != null) nextButton.onClick.RemoveListener(OnNextPressed);
            if (replayButton != null) replayButton.onClick.RemoveListener(OnReplayPressed);
            if (playschoolButton != null) playschoolButton.onClick.RemoveListener(OnPlayschoolPressed);
        }

        /// <summary>
        /// Shows the badge and freezes the game. Pass a null
        /// <paramref name="onNextCallback"/> on the last mission -- the next button
        /// hides itself rather than dead-ending the child.
        /// </summary>
        public void Show(Action onNextCallback, Action onReplayCallback)
        {
            if (visible) return;

            onNext = onNextCallback;
            onReplay = onReplayCallback;
            visible = true;

            if (nextButton != null) nextButton.gameObject.SetActive(onNextCallback != null);

            previousTimeScale = Time.timeScale > 0f ? Time.timeScale : 1f;
            Time.timeScale = 0f;

            if (root != null) root.SetActive(true);

            BridgeQuestVoice.Play(voiceKey);

            StopAllCoroutines();
            StartCoroutine(PopIn());
        }

        private IEnumerator PopIn()
        {
            if (badgeGroup == null) yield break;

            badgeGroup.DOKill();
            badgeGroup.localScale = Vector3.zero;

            Tween pop = badgeGroup.DOScale(1f, popInDuration).SetEase(Ease.OutBack, 1.6f).SetUpdate(true);
            yield return pop.WaitForCompletion();

            if (!visible) yield break;

            badgeGroup.localScale = Vector3.one;
            badgeGroup
                .DOScale(1f + idlePulseScale, idlePulseDuration)
                .SetEase(Ease.InOutSine)
                .SetLoops(-1, LoopType.Yoyo)
                .SetUpdate(true);
        }

        private void Update()
        {
            // unscaled, because the world is frozen while this is up
            if (visible && shine != null)
            {
                shine.Rotate(0f, 0f, -shineSpinSpeed * Time.unscaledDeltaTime);
            }
        }

        private void OnNextPressed()
        {
            if (!visible) return;
            BridgeQuestSfx.Tap();
            Hide();

            Action cb = onNext;
            onNext = null;
            if (cb != null) cb();
        }

        private void OnReplayPressed()
        {
            if (!visible) return;
            BridgeQuestSfx.Tap();
            Hide();

            Action cb = onReplay;
            onReplay = null;
            if (cb != null) cb();
        }

        private void OnPlayschoolPressed()
        {
            if (!visible) return;
            BridgeQuestSfx.Tap();

            // whatever happens next, don't hand the app on with a frozen clock
            Time.timeScale = 1f;
            visible = false;

            if (GameManager.Instance != null) GameManager.Instance.GoBackToPlayschool();
        }

        private void Hide()
        {
            visible = false;

            StopAllCoroutines();
            if (badgeGroup != null)
            {
                badgeGroup.DOKill();
                badgeGroup.localScale = Vector3.one;
            }

            if (root != null) root.SetActive(false);
            Time.timeScale = previousTimeScale;
        }
    }
}

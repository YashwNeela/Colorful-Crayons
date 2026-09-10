using System;
using System.Collections;
using DG.Tweening;
using TMKOC;
using UnityEngine;
using UnityEngine.UI;

namespace TMKOC.FruitAndVeggiRun
{

    /// <summary>
    /// End-of-run screen. Freezes the world, drops a badge over slowly turning shine
    /// rays, and offers two ways out: play again, or back to Playschool.
    ///
    /// One component, two instances: the lose screen (TRY AGAIN! badge, shown when the
    /// last life goes) and the win screen (YOU WIN! badge, shown after the closing
    /// cut-scene). Only the badge sprite and the spoken line differ.
    ///
    /// Playing again is a straight in-place reset of the run (GameFlow.RestartRun) --
    /// nothing reloads -- so the opening cut-scene and the tutorial never replay.
    /// </summary>
    public class EndScreenUI : MonoBehaviour
    {
        [Header("Refs")]
        [SerializeField] private GameObject root;
        [Tooltip("Everything that punches in together: rays, badge and the tap target.")]
        [SerializeField] private RectTransform badgeGroup;
        [Tooltip("Shine rays that turn slowly behind the badge.")]
        [SerializeField] private RectTransform shine;
        [SerializeField] private Button restartButton;
        [SerializeField] private Button playschoolButton;

        [Header("Voice-over")]
        [Tooltip("voiceover_title from the VO sheet -- retry_screen on the lose panel, win_screen on the win panel. Blank for silence.")]
        [SerializeField] private string voiceKey = "retry_screen";

        [Header("Tuning")]
        [SerializeField] private float popInDuration = 0.45f;
        [Tooltip("Degrees per second the shine rays turn. Negative to spin the other way.")]
        [SerializeField] private float shineSpinSpeed = 10f;
        [Tooltip("Gentle breathing on the badge once it has landed.")]
        [SerializeField] private float idlePulseScale = 0.04f;
        [SerializeField] private float idlePulseDuration = 1.4f;

        private Action onRestart;
        private bool visible;
        private float previousTimeScale = 1f;

        /// <summary>True while this screen is up and waiting on the player.</summary>
        public bool IsShowing { get { return visible; } }

        private void Awake()
        {
            if (root != null) root.SetActive(false);
        }

        private void OnEnable()
        {
            if (restartButton != null) restartButton.onClick.AddListener(OnRestartPressed);
            if (playschoolButton != null) playschoolButton.onClick.AddListener(OnPlayschoolPressed);
        }

        private void OnDisable()
        {
            if (restartButton != null) restartButton.onClick.RemoveListener(OnRestartPressed);
            if (playschoolButton != null) playschoolButton.onClick.RemoveListener(OnPlayschoolPressed);
        }

        /// <summary>
        /// Shows the screen and freezes the game. <paramref name="onRestartCallback"/>
        /// runs if the player chooses to play again.
        /// </summary>
        public void Show(Action onRestartCallback)
        {
            if (visible) return;

            onRestart = onRestartCallback;
            visible = true;

            previousTimeScale = Time.timeScale > 0f ? Time.timeScale : 1f;
            Time.timeScale = 0f;

            if (root != null) root.SetActive(true);

            RocketRunVoice.Play(voiceKey);

            StopAllCoroutines();
            StartCoroutine(PopIn());
        }

        /// <summary>Badge drops in on unscaled time, then breathes gently while it waits.</summary>
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

        private void OnRestartPressed()
        {
            if (!visible) return;

            Hide();

            Action cb = onRestart;
            onRestart = null;
            if (cb != null) cb();
        }

        private void OnPlayschoolPressed()
        {
            if (!visible) return;

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

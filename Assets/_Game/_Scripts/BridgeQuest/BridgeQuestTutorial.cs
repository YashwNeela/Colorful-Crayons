using System;
using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TMKOC.BridgeQuest
{
    /// <summary>
    /// Two jobs, both borrowed from RocketRunTutorial's shape.
    ///
    /// 1. The opening lesson. Each instruction is shown with the world frozen and
    ///    waits for a tap. It sits still until the opening storyboard has finished,
    ///    so the two never speak over each other.
    ///
    /// 2. The stuck-nudge. If a question goes unanswered for a while, an animated
    ///    hand appears over the correct option. RocketRun did not need this -- a
    ///    runner always moves -- but a question card does not resolve itself, and a
    ///    three-year-old who cannot find the answer has no other way forward. The
    ///    GDD specifies no hint behaviour at all; this is the safety net.
    /// </summary>
    public class BridgeQuestTutorial : MonoBehaviour
    {
        [Header("Bubble")]
        [SerializeField] private GameObject root;
        [SerializeField] private RectTransform bubble;
        [SerializeField] private Image primaryIcon;
        [SerializeField] private TextMeshProUGUI messageText;
        [SerializeField] private TextMeshProUGUI tapHintText;

        [Header("Step icons")]
        [Tooltip("Ear or speaker art for the 'listen' step.")]
        [SerializeField] private Sprite listenIcon;

        [Tooltip("Pointing-finger art for the 'tap the answer' step.")]
        [SerializeField] private Sprite handIcon;

        [Tooltip("Plank art for the 'each answer builds the bridge' step.")]
        [SerializeField] private Sprite plankIcon;

        [Header("Stuck-nudge")]
        [Tooltip("Animated hand, reparented over the correct option when the child stalls.")]
        [SerializeField] private RectTransform hintHand;

        [Tooltip("Seconds of no tap before the hand appears. Long enough not to rob them of the answer.")]
        [SerializeField] private float hintDelay = 8f;

        [Tooltip("Seconds before the hand appears again, if they still have not answered.")]
        [SerializeField] private float hintRepeatDelay = 6f;

        [SerializeField] private float hintBobHeight = 18f;
        [SerializeField] private float hintBobDuration = 0.6f;

        [Header("Tuning")]
        [SerializeField] private float popInDuration = 0.3f;

        private class Step
        {
            public string message;
            public Sprite icon;
            public string voiceKey;
        }

        private Coroutine hintRoutine;

        private void Awake()
        {
            if (root != null) root.SetActive(false);
            if (hintHand != null) hintHand.gameObject.SetActive(false);
        }

        // ---- opening lesson -------------------------------------------------

        /// <summary>
        /// Runs the three-step opening lesson, then calls back. Waits out the opening
        /// storyboard first -- <see cref="BridgeStoryCutsceneUI.IsPlaying"/> is the
        /// handshake, exactly as RocketRunTutorial waits on StoryCutsceneUI.
        /// </summary>
        public void RunOpeningLesson(Action onComplete)
        {
            StartCoroutine(LessonRoutine(onComplete));
        }

        private IEnumerator LessonRoutine(Action onComplete)
        {
            // let every other Start() run before we touch anything
            yield return null;

            while (BridgeStoryCutsceneUI.IsPlaying) yield return null;

            BridgeQuestAudioMapper voice = BridgeQuestVoice.Mapper;

            yield return ShowInstruction(new Step
            {
                message = "Listen to the question.",
                icon = listenIcon,
                voiceKey = voice != null ? voice.TutorialListen : null
            });

            yield return ShowInstruction(new Step
            {
                message = "Tap the right answer!",
                icon = handIcon,
                voiceKey = voice != null ? voice.TutorialTap : null
            });

            yield return ShowInstruction(new Step
            {
                message = "Every right answer builds the bridge.",
                icon = plankIcon,
                voiceKey = voice != null ? voice.TutorialBridge : null
            });

            if (root != null) root.SetActive(false);
            Time.timeScale = 1f;

            if (onComplete != null) onComplete();
        }

        /// <summary>
        /// Freezes the world, pops the bubble in, waits for a tap, then hides it and
        /// unfreezes so the child can act on what they just heard.
        /// </summary>
        private IEnumerator ShowInstruction(Step step)
        {
            Time.timeScale = 0f;
            if (root != null) root.SetActive(true);

            if (messageText != null) messageText.text = step.message;
            if (tapHintText != null) tapHintText.gameObject.SetActive(true);

            if (primaryIcon != null)
            {
                primaryIcon.sprite = step.icon;
                primaryIcon.enabled = step.icon != null;
            }

            BridgeQuestVoice.Play(step.voiceKey);

            if (bubble != null)
            {
                float t = 0f;
                bubble.localScale = Vector3.zero;
                while (t < popInDuration)
                {
                    t += Time.unscaledDeltaTime;
                    float p = Mathf.Clamp01(t / popInDuration);
                    bubble.localScale = Vector3.one * Mathf.Sin(p * Mathf.PI * 0.5f);
                    yield return null;
                }
                bubble.localScale = Vector3.one;
            }

            // eat one frame so the tap that dismissed the previous bubble cannot
            // also immediately dismiss this one
            yield return null;

            while (!TapDetected()) yield return null;

            if (root != null) root.SetActive(false);
            Time.timeScale = 1f;
        }

        // ---- stuck-nudge ----------------------------------------------------

        /// <summary>Starts watching a presented question. Cancelled by <see cref="DisarmHint"/>.</summary>
        public void ArmHint(QuestionCardUI card)
        {
            DisarmHint();
            if (card == null || hintHand == null) return;

            hintRoutine = StartCoroutine(HintRoutine(card));
        }

        public void DisarmHint()
        {
            if (hintRoutine != null)
            {
                StopCoroutine(hintRoutine);
                hintRoutine = null;
            }

            if (hintHand != null)
            {
                hintHand.DOKill();
                hintHand.gameObject.SetActive(false);
            }
        }

        private IEnumerator HintRoutine(QuestionCardUI card)
        {
            yield return new WaitForSecondsRealtime(hintDelay);

            while (true)
            {
                RectTransform target = card.CorrectSlotRect;
                if (target == null) yield break;

                hintHand.SetParent(target, false);
                hintHand.anchoredPosition = Vector2.zero;
                hintHand.SetAsLastSibling();
                hintHand.gameObject.SetActive(true);

                hintHand.DOKill();
                hintHand
                    .DOAnchorPosY(hintBobHeight, hintBobDuration)
                    .SetEase(Ease.InOutSine)
                    .SetLoops(6, LoopType.Yoyo)
                    .SetUpdate(true);

                yield return new WaitForSecondsRealtime(hintBobDuration * 6f);

                hintHand.DOKill();
                hintHand.gameObject.SetActive(false);

                yield return new WaitForSecondsRealtime(hintRepeatDelay);
            }
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
            if (hintHand != null) hintHand.DOKill();
        }
    }
}

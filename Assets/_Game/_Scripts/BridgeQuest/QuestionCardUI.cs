using System;
using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TMKOC.BridgeQuest
{
    /// <summary>
    /// The question card: a prompt the child hears, and three options they tap.
    ///
    /// Bridge Quest is no-fail by design. A wrong tap shakes, plays a gentle retry
    /// line and re-arms -- it never costs anything and there is no attempt limit.
    /// This mirrors RocketRun's "wrong pickup is a free mistake" tier; RocketRun's
    /// other tier (hazard crash costs a life) has no equivalent here, which is why
    /// Bridge Quest has no lose screen at all.
    ///
    /// Presentation order is shuffled every time. Without that, a child learns the
    /// position rather than the letter -- the single most important thing this
    /// component does.
    /// </summary>
    public class QuestionCardUI : MonoBehaviour
    {
        /// <summary>One option slot in the scene. Art and label are mutually exclusive.</summary>
        [Serializable]
        public class OptionView
        {
            public Button button;
            public RectTransform rect;
            public Image art;
            public TextMeshProUGUI label;

            [Tooltip("Plain colour swatch, used by Colour questions.")]
            public Image swatch;
        }

        [Header("Refs")]
        [SerializeField] private GameObject root;
        [SerializeField] private RectTransform card;
        [SerializeField] private TextMeshProUGUI promptText;
        [SerializeField] private Image promptArt;

        [Tooltip("Re-speaks the question. Essential -- pre-readers cannot re-read the prompt.")]
        [SerializeField] private Button replayButton;

        [Tooltip("Exactly three, per the GDD.")]
        [SerializeField] private OptionView[] options = new OptionView[3];

        [Header("Feedback")]
        [SerializeField] private float popInDuration = 0.35f;
        [SerializeField] private float shakeDuration = 0.4f;
        [SerializeField] private float shakeStrength = 24f;

        [Tooltip("Taps ignored for this long after a wrong answer, so a mashing child cannot fire the retry line five times.")]
        [SerializeField] private float wrongLockout = 0.5f;

        [Tooltip("Beat between the correct tap and handing control back, so the praise line is not cut off by the plank card.")]
        [SerializeField] private float correctHold = 0.6f;

        [Header("Accessibility")]
        [Tooltip("Colour questions must not rely on hue alone -- red/green is the most common deficiency. Leave on to show the colour's name under the swatch.")]
        [SerializeField] private bool labelColourOptions = true;

        private QuestionData current;
        private int[] order;          // presentation slot -> authored option index
        private Action onAnswered;
        private bool accepting;
        private bool answered;

        private void Awake()
        {
            if (root != null) root.SetActive(false);
        }

        private void OnEnable()
        {
            if (replayButton != null) replayButton.onClick.AddListener(RepeatPrompt);
            HookOptions(true);
        }

        private void OnDisable()
        {
            if (replayButton != null) replayButton.onClick.RemoveListener(RepeatPrompt);
            HookOptions(false);
        }

        private void HookOptions(bool add)
        {
            if (options == null) return;

            for (int i = 0; i < options.Length; i++)
            {
                if (options[i] == null || options[i].button == null) continue;

                int slot = i; // capture
                if (add) options[i].button.onClick.AddListener(delegate { OnOptionTapped(slot); });
                else options[i].button.onClick.RemoveAllListeners();
            }
        }

        /// <summary>
        /// Shows a question and waits for the child to find the answer.
        /// <paramref name="onCorrect"/> fires once, on the correct tap -- wrong taps
        /// never end the question.
        /// </summary>
        public void Present(QuestionData question, Action onCorrect)
        {
            if (question == null || !question.IsValid)
            {
                Debug.LogWarning("[BridgeQuest] Asked to present an invalid question -- skipping it.", this);
                if (onCorrect != null) onCorrect();
                return;
            }

            current = question;
            onAnswered = onCorrect;
            answered = false;

            BuildOrder(question.options.Length);
            Paint();

            if (root != null) root.SetActive(true);

            StopAllCoroutines();
            StartCoroutine(PopInThenAccept());
        }

        public void Hide()
        {
            StopAllCoroutines();
            accepting = false;
            if (card != null) card.DOKill();
            if (root != null) root.SetActive(false);
        }

        /// <summary>Fisher-Yates. Re-rolled per presentation, so a replayed mission never repeats the layout.</summary>
        private void BuildOrder(int count)
        {
            order = new int[count];
            for (int i = 0; i < count; i++) order[i] = i;

            for (int i = count - 1; i > 0; i--)
            {
                int j = UnityEngine.Random.Range(0, i + 1);
                int tmp = order[i];
                order[i] = order[j];
                order[j] = tmp;
            }
        }

        private void Paint()
        {
            if (promptText != null) promptText.text = current.prompt;

            if (promptArt != null)
            {
                promptArt.sprite = current.promptArt;
                promptArt.enabled = current.promptArt != null;
            }

            int slots = options != null ? options.Length : 0;

            for (int slot = 0; slot < slots; slot++)
            {
                OptionView view = options[slot];
                if (view == null) continue;

                bool used = order != null && slot < order.Length;
                if (view.button != null) view.button.gameObject.SetActive(used);
                if (!used) continue;

                AnswerOption opt = current.options[order[slot]];

                if (view.rect != null)
                {
                    view.rect.DOKill();
                    view.rect.localRotation = Quaternion.identity;
                    view.rect.localScale = Vector3.one;
                }

                bool isColour = opt.useColour;

                if (view.swatch != null)
                {
                    view.swatch.gameObject.SetActive(isColour);
                    if (isColour) view.swatch.color = opt.colour;
                }

                if (view.art != null)
                {
                    view.art.sprite = opt.art;
                    view.art.enabled = !isColour && opt.art != null;
                }

                if (view.label != null)
                {
                    // a colour swatch still gets its name underneath unless that is
                    // switched off -- hue alone is not an accessible cue
                    bool showLabel = !string.IsNullOrEmpty(opt.label)
                        && (!isColour || labelColourOptions)
                        && (isColour || opt.art == null);

                    view.label.gameObject.SetActive(showLabel);
                    if (showLabel) view.label.text = opt.label;
                }

                if (view.button != null) view.button.interactable = true;
            }
        }

        private IEnumerator PopInThenAccept()
        {
            accepting = false;

            if (card != null)
            {
                card.DOKill();
                card.localScale = Vector3.zero;
                Tween t = card.DOScale(1f, popInDuration).SetEase(Ease.OutBack, 1.5f).SetUpdate(true);
                yield return t.WaitForCompletion();
                card.localScale = Vector3.one;
            }

            // the prompt is only spoken once the card has landed -- a line that starts
            // under a moving card reads as belonging to whatever came before it
            BridgeQuestVoice.Play(current.promptVoiceKey);

            accepting = true;
        }

        private Coroutine repeatRoutine;

        private void RepeatPrompt()
        {
            if (current == null || answered) return;

            BridgeQuestSfx.Tap();

            if (repeatRoutine != null) StopCoroutine(repeatRoutine);
            repeatRoutine = StartCoroutine(RepeatRoutine());
        }

        /// <summary>
        /// Speaks 'Listen again.' and then the question itself. The two cannot be
        /// fired back to back: RuntimeAudioLoader plays every line through one shared
        /// AudioSource and calls Stop() first, so the second call would swallow the
        /// first. Realtime waits, because the card is up while Time.timeScale is 0.
        /// </summary>
        private IEnumerator RepeatRoutine()
        {
            BridgeQuestAudioMapper voice = BridgeQuestVoice.Mapper;
            float lead = voice != null ? BridgeQuestVoice.PlayAndGetLength(voice.RepeatQuestion) : 0f;
            if (lead > 0f) yield return new WaitForSecondsRealtime(lead);

            // the child may have answered while 'Listen again.' was still speaking
            if (current == null || answered) { repeatRoutine = null; yield break; }

            BridgeQuestVoice.Play(current.promptVoiceKey);
            repeatRoutine = null;
        }

        private void OnOptionTapped(int slot)
        {
            if (!accepting || answered || current == null) return;
            if (order == null || slot >= order.Length) return;

            int authored = order[slot];
            AnswerOption opt = current.options[authored];

            if (authored == current.correctIndex)
            {
                answered = true;
                accepting = false;
                StartCoroutine(CorrectRoutine(slot, opt));
                return;
            }

            StartCoroutine(WrongRoutine(slot, opt));
        }

        private IEnumerator CorrectRoutine(int slot, AnswerOption opt)
        {
            LockAll();

            OptionView view = options[slot];
            if (view != null && view.rect != null)
            {
                view.rect.DOKill();
                view.rect.DOPunchScale(Vector3.one * 0.25f, 0.35f, 8, 0.8f).SetUpdate(true);
            }

            BridgeQuestAudioMapper voice = BridgeQuestVoice.Mapper;
            if (voice != null) BridgeQuestVoice.Play(voice.GetRandomCorrect());

            BridgeQuestGameManager.RaiseCorrectAnswer(current.type);

            yield return new WaitForSecondsRealtime(correctHold);

            Hide();

            Action cb = onAnswered;
            onAnswered = null;
            if (cb != null) cb();
        }

        private IEnumerator WrongRoutine(int slot, AnswerOption opt)
        {
            accepting = false;

            OptionView view = options[slot];
            if (view != null && view.rect != null)
            {
                view.rect.DOKill();
                view.rect.localRotation = Quaternion.identity;
                view.rect
                    .DOShakeAnchorPos(shakeDuration, new Vector2(shakeStrength, 0f), 12, 90f, false, true)
                    .SetUpdate(true);
            }

            BridgeQuestAudioMapper voice = BridgeQuestVoice.Mapper;
            if (voice != null) BridgeQuestVoice.Play(voice.GetRandomWrong());

            BridgeQuestGameManager.RaiseWrongAnswer(current.type);

            yield return new WaitForSecondsRealtime(wrongLockout);

            // no penalty, no attempt counter, nothing removed from the board --
            // the child simply gets to try again
            if (!answered) accepting = true;
        }

        private void LockAll()
        {
            if (options == null) return;
            for (int i = 0; i < options.Length; i++)
            {
                if (options[i] != null && options[i].button != null)
                {
                    options[i].button.interactable = false;
                }
            }
        }

        /// <summary>The slot the correct answer landed in this shuffle. Used by the tutorial to point at it.</summary>
        public RectTransform CorrectSlotRect
        {
            get
            {
                if (current == null || order == null || options == null) return null;

                for (int slot = 0; slot < order.Length && slot < options.Length; slot++)
                {
                    if (order[slot] == current.correctIndex && options[slot] != null)
                    {
                        return options[slot].rect;
                    }
                }
                return null;
            }
        }
    }
}

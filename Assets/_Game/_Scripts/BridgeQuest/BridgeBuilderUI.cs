using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace TMKOC.BridgeQuest
{
    /// <summary>
    /// The bridge itself -- and, more importantly, the progress bar.
    ///
    /// This is the best idea in the GDD: a pre-reader cannot parse "3/5" but can see
    /// a gap closing. So the plank IS the progress indicator; there is no separate
    /// counter, and there should not be one.
    ///
    /// Planks are pre-placed in the scene along the span and start hidden. A correct
    /// answer drops the next one in, in order, left to right.
    /// </summary>
    public class BridgeBuilderUI : MonoBehaviour
    {
        [Header("Refs")]
        [Tooltip("One per question, laid out along the gap left to right. Hidden at mission start.")]
        [SerializeField] private Image[] plankSlots;

        [Tooltip("Optional -- the character walks across this once the span is closed.")]
        [SerializeField] private RectTransform walker;

        [SerializeField] private RectTransform walkStart;
        [SerializeField] private RectTransform walkEnd;

        [Header("Drop-in")]
        [SerializeField] private float dropDuration = 0.5f;
        [SerializeField] private float dropHeight = 320f;
        [SerializeField] private float dropRotation = 25f;

        [Header("Crossing")]
        [SerializeField] private float walkDuration = 2.2f;

        private int placed;

        /// <summary>How many planks are down.</summary>
        public int Placed { get { return placed; } }

        /// <summary>How many the span needs.</summary>
        public int Total { get { return plankSlots != null ? plankSlots.Length : 0; } }

        public bool IsComplete { get { return Total > 0 && placed >= Total; } }

        /// <summary>
        /// Empties the span, ready for a mission. <paramref name="plankSprite"/> lets
        /// each mission use its own plank art (a wooden plank, a stone, a log) without
        /// a separate prefab per mission.
        /// </summary>
        public void ResetBridge(Sprite plankSprite)
        {
            placed = 0;

            if (plankSlots != null)
            {
                for (int i = 0; i < plankSlots.Length; i++)
                {
                    Image slot = plankSlots[i];
                    if (slot == null) continue;

                    slot.rectTransform.DOKill();
                    if (plankSprite != null) slot.sprite = plankSprite;
                    slot.gameObject.SetActive(false);
                }
            }

            if (walker != null && walkStart != null)
            {
                walker.DOKill();
                walker.anchoredPosition = walkStart.anchoredPosition;
            }
        }

        /// <summary>
        /// Drops the next plank into the span. Runs on unscaled time because the
        /// caller may well have the world frozen behind a card.
        /// </summary>
        public void PlaceNextPlank(Action onLanded)
        {
            if (plankSlots == null || placed >= plankSlots.Length)
            {
                if (onLanded != null) onLanded();
                return;
            }

            Image slot = plankSlots[placed];
            placed++;

            BridgeQuestGameManager.RaisePlankPlaced(placed, Total);

            BridgeQuestAudioMapper voice = BridgeQuestVoice.Mapper;
            if (voice != null) BridgeQuestVoice.Play(voice.GetRandomPlank());

            if (slot == null)
            {
                if (onLanded != null) onLanded();
                return;
            }

            RectTransform rt = slot.rectTransform;
            Vector2 rest = rt.anchoredPosition;

            slot.gameObject.SetActive(true);
            rt.DOKill();
            rt.anchoredPosition = rest + new Vector2(0f, dropHeight);
            rt.localRotation = Quaternion.Euler(0f, 0f, dropRotation);

            Sequence seq = DOTween.Sequence().SetUpdate(true);
            seq.Append(rt.DOAnchorPos(rest, dropDuration).SetEase(Ease.OutBounce));
            seq.Join(rt.DOLocalRotate(Vector3.zero, dropDuration).SetEase(Ease.OutBack));
            seq.OnComplete(delegate
            {
                rt.anchoredPosition = rest;
                rt.localRotation = Quaternion.identity;

                if (IsComplete)
                {
                    BridgeQuestGameManager.RaiseBridgeComplete();
                    if (voice != null) BridgeQuestVoice.Play(voice.BridgeComplete);
                }

                if (onLanded != null) onLanded();
            });
        }

        /// <summary>
        /// Walks the character across the finished span. The payoff the whole mission
        /// has been building to -- give it room before the closing storyboard starts.
        /// </summary>
        public void PlayCrossing(Action onArrived)
        {
            if (walker == null || walkEnd == null)
            {
                if (onArrived != null) onArrived();
                return;
            }

            walker.DOKill();
            walker
                .DOAnchorPos(walkEnd.anchoredPosition, walkDuration)
                .SetEase(Ease.Linear)
                .SetUpdate(true)
                .OnComplete(delegate
                {
                    if (onArrived != null) onArrived();
                });
        }

        private void OnDestroy()
        {
            if (plankSlots != null)
            {
                for (int i = 0; i < plankSlots.Length; i++)
                {
                    if (plankSlots[i] != null) plankSlots[i].rectTransform.DOKill();
                }
            }
            if (walker != null) walker.DOKill();
        }
    }
}

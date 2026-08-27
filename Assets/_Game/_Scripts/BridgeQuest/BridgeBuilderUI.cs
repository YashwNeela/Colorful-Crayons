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

        [Tooltip("Optional -- the character rig displayed in the walker. Idle while the bridge is\n" +
                 "being built, walk for the crossing. Left empty, the walker just slides.")]
        [SerializeField] private BridgeQuestPlayerView playerView;

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
            ResetBridge(plankSprite, null);
        }

        /// <summary>
        /// Empties the span, ready for a mission.
        ///
        /// <paramref name="perSlotSprites"/> lets a mission draw its planks in
        /// perspective -- one sprite per slot, left to right across the span. Any
        /// slot the array does not cover (or leaves null) falls back to
        /// <paramref name="plankSprite"/>, so a mission that only wants a single
        /// themed plank (a stone, a log) still works by passing that alone.
        ///
        /// If both are null the slot keeps whatever sprite the scene gave it.
        /// </summary>
        public void ResetBridge(Sprite plankSprite, Sprite[] perSlotSprites)
        {
            placed = 0;

            if (plankSlots != null)
            {
                for (int i = 0; i < plankSlots.Length; i++)
                {
                    Image slot = plankSlots[i];
                    if (slot == null) continue;

                    slot.rectTransform.DOKill();

                    Sprite art = null;
                    if (perSlotSprites != null && i < perSlotSprites.Length) art = perSlotSprites[i];
                    if (art == null) art = plankSprite;
                    if (art != null) slot.sprite = art;

                    slot.gameObject.SetActive(false);
                }
            }

            if (walker != null && walkStart != null)
            {
                walker.DOKill();
                walker.anchoredPosition = walkStart.anchoredPosition;
            }

            // standing at the near bank while the questions are answered
            if (playerView != null) playerView.PlayIdle();
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

            // feet move for exactly as long as the slot slides
            if (playerView != null) playerView.PlayWalk();

            walker
                .DOAnchorPos(walkEnd.anchoredPosition, walkDuration)
                .SetEase(Ease.Linear)
                .SetUpdate(true)
                .OnComplete(delegate
                {
                    // arrived -- stand still again before the closing storyboard
                    if (playerView != null) playerView.PlayIdle();

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

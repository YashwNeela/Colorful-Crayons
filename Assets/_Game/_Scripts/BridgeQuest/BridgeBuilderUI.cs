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
                [Tooltip("Full-span crossing time, near bank to far bank. When the character has already stepped part of the way across, only the remaining fraction of this is spent -- see scaleCrossingByRemainingDistance.")]
        [SerializeField] private float walkDuration = 2.2f;

        [Tooltip("Scales the crossing to the distance still to cover. With per-plank stepping on, the last leg is only the final plank to the far bank, and spending the whole walkDuration on it would be a crawl. Off, the crossing always takes walkDuration whatever the start point.")]
        [SerializeField] private bool scaleCrossingByRemainingDistance = true;

        [Tooltip("Floor for the scaled crossing, so the last hop is never instant.")]
        [SerializeField] private float minCrossingDuration = 0.45f;

        [Header("Stepping")]
        [Tooltip("Time for one step onto a freshly placed plank. Fixed per step by default, so every plank reads the same however uneven the spacing is.")]
        [SerializeField] private float stepDuration = 0.8f;

        [Tooltip("On, stepDuration becomes the time for an AVERAGE step and each one is scaled by how far it actually is -- honest, but it makes the uneven plank spacing visible.")]
        [SerializeField] private bool scaleStepByDistance = false;

        [Tooltip("Nudges where the character stands relative to the plank's centre. X shifts along the span, Y off the walk line.")]
        [SerializeField] private Vector2 walkerPlankOffset = Vector2.zero;

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

            // standing at the near bank while the questions are answered -- and facing
            // sideways again, in case the last run ended mid-celebration
            if (playerView != null)
            {
                playerView.StopCelebration();
                playerView.PlayIdle();
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
        /// Walks the character onto the plank at <paramref name="slotIndex"/> and leaves
        /// them standing there. This is the per-question beat: answer, the plank lands,
        /// the child sees the character actually gain ground, then the next question.
        ///
        /// Steps are NOT reset between calls -- each one starts from wherever the last
        /// ended, so the walk accumulates across the mission and PlayCrossing is left
        /// with only the final plank to the far bank.
        ///
        /// Unscaled, like everything else here: the caller usually has the world frozen.
        /// </summary>
        public void StepToPlank(int slotIndex, Action onArrived)
        {
            if (walker == null || plankSlots == null
                || slotIndex < 0 || slotIndex >= plankSlots.Length
                || plankSlots[slotIndex] == null)
            {
                if (onArrived != null) onArrived();
                return;
            }

            Vector2 target = WalkPositionForSlot(slotIndex);
            float distance = Vector2.Distance(walker.anchoredPosition, target);

            // already standing there -- do not play a walk cycle on the spot
            if (distance < 1f)
            {
                if (onArrived != null) onArrived();
                return;
            }

            float duration = stepDuration;
            if (scaleStepByDistance)
            {
                float average = AverageStepDistance();
                if (average > 0.01f) duration = stepDuration * (distance / average);
            }
            duration = Mathf.Max(0.05f, duration);

            walker.DOKill();
            if (playerView != null) playerView.PlayWalk();

            walker
                .DOAnchorPos(target, duration)
                .SetEase(Ease.Linear)
                .SetUpdate(true)
                .OnComplete(delegate
                {
                    walker.anchoredPosition = target;
                    if (playerView != null) playerView.PlayIdle();
                    if (onArrived != null) onArrived();
                });
        }

        /// <summary>Steps onto the plank <see cref="PlaceNextPlank"/> has just dropped.</summary>
        public void StepToLastPlacedPlank(Action onArrived)
        {
            StepToPlank(placed - 1, onArrived);
        }

        /// <summary>
        /// Where the character stands to be on slot <paramref name="slotIndex"/>.
        ///
        /// X comes from the plank, converted into the walker's own parent space rather
        /// than read straight off anchoredPosition -- the planks sit under their own
        /// container, and that container is free to move.
        ///
        /// Y does NOT come from the plank. The walker is a 208x288 slot whose centre
        /// rides well above the deck, and its line is defined by WalkStart/WalkEnd, so
        /// the height is interpolated between those two and the plank only decides how
        /// far along the span the character is.
        /// </summary>
        private Vector2 WalkPositionForSlot(int slotIndex)
        {
            RectTransform slot = plankSlots[slotIndex].rectTransform;
            RectTransform parent = walker.parent as RectTransform;

            float x = parent != null
                ? parent.InverseTransformPoint(slot.position).x
                : slot.anchoredPosition.x;

            float y = walker.anchoredPosition.y;
            if (walkStart != null && walkEnd != null)
            {
                int last = plankSlots.Length - 1;
                float t = last > 0 ? (float)slotIndex / last : 1f;
                y = Mathf.Lerp(walkStart.anchoredPosition.y, walkEnd.anchoredPosition.y, t);
            }

            return new Vector2(x, y) + walkerPlankOffset;
        }

        /// <summary>Mean gap between consecutive walk positions. Only scaleStepByDistance reads it.</summary>
        private float AverageStepDistance()
        {
            if (walkStart == null || walkEnd == null || plankSlots == null || plankSlots.Length == 0) return 0f;

            float span = Vector2.Distance(walkStart.anchoredPosition, walkEnd.anchoredPosition);
            return span / (plankSlots.Length + 1);
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

            Vector2 target = walkEnd.anchoredPosition;

            // already on the far bank -- nothing left to walk
            if (Vector2.Distance(walker.anchoredPosition, target) < 1f)
            {
                if (playerView != null) playerView.PlayIdle();
                if (onArrived != null) onArrived();
                return;
            }

            float duration = walkDuration;

            // With per-plank stepping the character is already standing on the last
            // plank, so this leg is a short hop onto the bank. Spend the same fraction
            // of walkDuration as the distance left is of the whole span, so the pace
            // matches the steps instead of crawling.
            if (scaleCrossingByRemainingDistance && walkStart != null)
            {
                float span = Vector2.Distance(walkStart.anchoredPosition, target);
                float remaining = Vector2.Distance(walker.anchoredPosition, target);

                if (span > 0.01f)
                {
                    duration = Mathf.Max(minCrossingDuration, walkDuration * (remaining / span));
                }
            }

            // feet move for exactly as long as the slot slides
            if (playerView != null) playerView.PlayWalk();

            walker
                .DOAnchorPos(target, duration)
                .SetEase(Ease.Linear)
                .SetUpdate(true)
                .OnComplete(delegate
                {
                    walker.anchoredPosition = target;

                    // arrived -- stand still again before the closing storyboard
                    if (playerView != null) playerView.PlayIdle();

                    if (onArrived != null) onArrived();
                });
        }

        /// <summary>True when the view has a celebration rig to swap in.</summary>
        public bool HasCelebration { get { return playerView != null && playerView.HasCelebration; } }

        /// <summary>
        /// The little dance at the far bank. Loops until something else moves the
        /// story on, so the caller decides how long it runs -- see
        /// BridgeQuestFlow.celebrationDuration.
        /// </summary>
        public void PlayCelebration()
        {
            if (playerView != null) playerView.PlayCelebration();
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

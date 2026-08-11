using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace TMKOC.FruitAndVeggiRun
{

    /// <summary>
    /// The target-item indicator at the top of the screen. Shows the produce's own
    /// icon: a dim "outline" copy always visible so the player knows what they're
    /// hunting, with a full-colour radial fill sweeping in smoothly as they collect
    /// the right item.
    ///
    /// Later difficulty bands put more than one fruit in play at once, so the icon
    /// authored in the scene acts as a template: extra slots are cloned from it at
    /// runtime and laid out beside it, each with its own fill and its own progress.
    /// </summary>
    public class TargetWordUI : MonoBehaviour
    {
        [SerializeField] private RectTransform iconContainer;
        [SerializeField] private Image outlineIcon;
        [SerializeField] private Image fillIcon;
        [SerializeField] private float fillTweenDuration = 0.35f;

        [Header("Extra Slots")]
        [Tooltip("Where each cloned icon sits relative to the one before it. Ignored when the parent drives layout itself.")]
        [SerializeField] private Vector2 extraSlotOffset = new Vector2(150f, 0f);

        /// <summary>One fruit's worth of HUD: the outline, the radial fill, and its own tween.</summary>
        private class Slot
        {
            public RectTransform root;
            public Image outline;
            public Image fill;
            public int total = 1;
            public Tween tween;
        }

        private readonly List<Slot> slots = new List<Slot>();

        // where the outline / fill sit among the template's Images, so the same two
        // can be picked out of a clone without relying on names
        private int outlineChildIndex = -1;
        private int fillChildIndex = -1;

        private int usedSlots = 1;
        private bool containerVisible = true;
        private bool fillVisible = true;
        private bool built;

        private void Awake()
        {
            Build();
        }

        /// <summary>Registers the scene-authored icon as slot 0. Safe to call repeatedly.</summary>
        private void Build()
        {
            if (built) return;
            built = true;

            if (iconContainer == null) return;

            Image[] images = iconContainer.GetComponentsInChildren<Image>(true);
            for (int i = 0; i < images.Length; i++)
            {
                if (images[i] == outlineIcon) outlineChildIndex = i;
                if (images[i] == fillIcon) fillChildIndex = i;
            }

            Slot s = new Slot();
            s.root = iconContainer;
            s.outline = outlineIcon;
            s.fill = fillIcon;
            slots.Add(s);
        }

        /// <summary>Clones the template until there are at least <paramref name="n"/> slots.</summary>
        private void EnsureSlots(int n)
        {
            Build();
            if (slots.Count == 0) return;

            RectTransform template = slots[0].root;
            if (template == null) return;

            // a layout group already knows how to space children; only place clones by
            // hand when nothing else is going to
            bool parentDrivesLayout = template.parent != null
                && template.parent.GetComponent<LayoutGroup>() != null;

            while (slots.Count < n)
            {
                RectTransform previous = slots[slots.Count - 1].root;

                RectTransform clone = Instantiate(template, template.parent);
                clone.name = template.name + "_" + slots.Count;
                clone.localScale = Vector3.one;
                if (!parentDrivesLayout && previous != null)
                {
                    clone.anchoredPosition = previous.anchoredPosition + extraSlotOffset;
                }

                Image[] images = clone.GetComponentsInChildren<Image>(true);

                Slot s = new Slot();
                s.root = clone;
                s.outline = (outlineChildIndex >= 0 && outlineChildIndex < images.Length) ? images[outlineChildIndex] : null;
                s.fill = (fillChildIndex >= 0 && fillChildIndex < images.Length) ? images[fillChildIndex] : null;
                slots.Add(s);
            }
        }

        /// <summary>Shows one icon per fruit currently being hunted and resets every fill.</summary>
        public void SetTargets(IList<string> names, IList<int> counts, IList<Sprite> icons)
        {
            int n = names != null ? names.Count : 0;
            EnsureSlots(n);
            usedSlots = n;

            for (int i = 0; i < slots.Count; i++)
            {
                Slot s = slots[i];
                bool used = i < n;

                if (s.root != null) s.root.gameObject.SetActive(used && containerVisible);
                if (!used) continue;

                s.total = Mathf.Max(1, counts != null && i < counts.Count ? counts[i] : 1);

                Sprite icon = (icons != null && i < icons.Count) ? icons[i] : null;
                if (s.outline != null) s.outline.sprite = icon;

                if (s.fill != null)
                {
                    s.fill.sprite = icon;
                    if (s.tween != null && s.tween.IsActive()) s.tween.Kill();
                    s.fill.fillAmount = 0f;
                    s.fill.enabled = fillVisible;
                }
            }
        }

        /// <summary>Single-fruit convenience wrapper, kept for the earlier one-target flow.</summary>
        public void SetWord(string newWord, int targetCount, Sprite icon)
        {
            SetTargets(new string[] { newWord }, new int[] { targetCount }, new Sprite[] { icon });
        }

        public void SetProgress(int collected)
        {
            SetSlotProgress(0, collected);
        }

        public void SetSlotProgress(int slotIndex, int collected)
        {
            Build();
            if (slotIndex < 0 || slotIndex >= slots.Count) return;

            Slot s = slots[slotIndex];
            if (s.fill == null) return;

            float pct = Mathf.Clamp01((float)collected / s.total);

            if (s.tween != null && s.tween.IsActive()) s.tween.Kill();
            // unscaled time so the fill still visibly completes even if something (e.g. the
            // item-complete popup) pauses the game the instant the final pickup lands
            s.tween = s.fill.DOFillAmount(pct, fillTweenDuration).SetEase(Ease.OutQuad).SetUpdate(true);
        }

        public void Celebrate()
        {
            CelebrateSlot(0);
        }

        public void CelebrateSlot(int slotIndex)
        {
            Build();
            if (slotIndex < 0 || slotIndex >= slots.Count) return;

            RectTransform root = slots[slotIndex].root;
            if (root == null) return;

            root.DOKill();
            root.localScale = Vector3.one;
            root.DOPunchScale(Vector3.one * 0.35f, 0.45f, 6, 0.6f).SetUpdate(true);
        }

        public void SetFillVisible(bool visible)
        {
            fillVisible = visible;
            Build();
            for (int i = 0; i < slots.Count; i++)
            {
                if (slots[i].fill != null) slots[i].fill.enabled = visible;
            }
        }

        public void SetIconContainerVisible(bool visible)
        {
            containerVisible = visible;
            Build();
            for (int i = 0; i < slots.Count; i++)
            {
                if (slots[i].root != null) slots[i].root.gameObject.SetActive(visible && i < usedSlots);
            }
        }
    }
}

using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

/// <summary>
/// The target-item indicator at the top of the screen. Shows the produce's own
/// icon: a dim "outline" copy always visible so the player knows what they're
/// hunting, with a full-colour radial fill sweeping in smoothly as they collect
/// the right item.
/// </summary>
public class TargetWordUI : MonoBehaviour
{
    [SerializeField] private RectTransform iconContainer;
    [SerializeField] private Image outlineIcon;
    [SerializeField] private Image fillIcon;
    [SerializeField] private float fillTweenDuration = 0.35f;

    private int total = 1;
    private Tween fillTween;

    public void SetWord(string newWord, int targetCount, Sprite icon)
    {
        total = Mathf.Max(1, targetCount);

        if (outlineIcon != null) outlineIcon.sprite = icon;
        if (fillIcon != null) fillIcon.sprite = icon;

        if (fillTween != null && fillTween.IsActive()) fillTween.Kill();
        if (fillIcon != null) fillIcon.fillAmount = 0f;
    }

    public void SetProgress(int collected)
    {
        if (fillIcon == null) return;

        float pct = Mathf.Clamp01((float)collected / total);

        if (fillTween != null && fillTween.IsActive()) fillTween.Kill();
        // unscaled time so the fill still visibly completes even if something (e.g. the
        // item-complete popup) pauses the game the instant the final pickup lands
        fillTween = fillIcon.DOFillAmount(pct, fillTweenDuration).SetEase(Ease.OutQuad).SetUpdate(true);
    }

    public void Celebrate()
    {
        if (iconContainer == null) return;
        iconContainer.DOKill();
        iconContainer.localScale = Vector3.one;
        iconContainer.DOPunchScale(Vector3.one * 0.35f, 0.45f, 6, 0.6f).SetUpdate(true);
    }

public void SetFillVisible(bool visible)
    {
        if (fillIcon != null) fillIcon.enabled = visible;
    }

public void SetIconContainerVisible(bool visible)
    {
        if (iconContainer != null) iconContainer.gameObject.SetActive(visible);
    }


}

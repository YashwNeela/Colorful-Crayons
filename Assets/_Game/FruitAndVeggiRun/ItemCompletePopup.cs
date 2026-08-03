using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Full-screen "you got it!" card shown when a shopping-list item is fully
/// collected. Freezes the world (Time.timeScale = 0) and waits for a tap
/// anywhere on screen before resuming play and invoking the completion callback.
/// </summary>
public class ItemCompletePopup : MonoBehaviour
{
    [SerializeField] private GameObject root;       // whole popup, inactive by default
    [SerializeField] private RectTransform bubble;   // the round card, scaled in on show
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI label;

    [Header("Timing")]
    [Tooltip("Pop-in duration and minimum time before a tap can dismiss the card, in unscaled seconds.")]
    [SerializeField] private float popInDuration = 0.3f;

    private Action onDismissed;

    private void Awake()
    {
        if (root != null) root.SetActive(false);
    }

    public void Show(string itemName, Sprite iconSprite, Action onDismissedCallback)
    {
        onDismissed = onDismissedCallback;

        if (icon != null) icon.sprite = iconSprite;
        if (label != null)
        {
            label.text = itemName;
            label.color = GameDefs.ColorOf(itemName);
        }

        if (root != null) root.SetActive(true);
        Time.timeScale = 0f;

        StopAllCoroutines();
        StartCoroutine(PlayInThenWaitForTap());
    }

    private IEnumerator PlayInThenWaitForTap()
    {
        // Unscaled-time pop-in so it still animates while the world is paused.
        float t = 0f;
        if (bubble != null) bubble.localScale = Vector3.zero;
        while (t < popInDuration)
        {
            t += Time.unscaledDeltaTime;
            float k = Mathf.Sin(Mathf.Clamp01(t / popInDuration) * Mathf.PI * 0.5f);
            if (bubble != null) bubble.localScale = Vector3.one * k;
            yield return null;
        }
        if (bubble != null) bubble.localScale = Vector3.one;

        while (!TapDetected())
        {
            yield return null;
        }

        Hide();
    }

    private bool TapDetected()
    {
        if (Input.GetMouseButtonDown(0)) return true;
        for (int i = 0; i < Input.touchCount; i++)
            if (Input.GetTouch(i).phase == TouchPhase.Began) return true;
        return false;
    }

    private void Hide()
    {
        if (root != null) root.SetActive(false);
        Time.timeScale = 1f;

        Action cb = onDismissed;
        onDismissed = null;
        cb?.Invoke();
    }
}

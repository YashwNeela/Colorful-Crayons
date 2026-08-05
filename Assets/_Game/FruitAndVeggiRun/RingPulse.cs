using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Expanding ring that draws the eye to a UI element -- switch the GameObject on
/// to start it, off to stop it. Runs on unscaled time so it keeps pulsing while
/// the tutorial holds the world at Time.timeScale = 0.
///
/// While it is running it can also lift its target out of whatever is dimming the
/// screen (liftCanvas) and push a graphic up to full opacity (brighten), so the
/// thing it is pointing at reads clearly instead of sitting under the tutorial
/// backdrop. Both are restored when it switches off.
/// </summary>
[RequireComponent(typeof(RectTransform))]
public class RingPulse : MonoBehaviour
{
    [Header("Pulse")]
    [SerializeField] private float startScale = 0.7f;
    [SerializeField] private float endScale = 1.9f;
    [SerializeField] private float peakAlpha = 0.9f;
    [Tooltip("Seconds for one ring to travel out and fade away.")]
    [SerializeField] private float pulseDuration = 0.9f;
    [Tooltip("Seconds of quiet between rings.")]
    [SerializeField] private float gapDuration = 0.3f;

    [Header("While Running")]
    [Tooltip("Canvas lifted above the tutorial dim while the ring is running. Optional.")]
    [SerializeField] private Canvas liftCanvas;
    [SerializeField] private int liftSortingOrder = 6;
    [Tooltip("Graphic pushed to full opacity while the ring is running. Optional.")]
    [SerializeField] private Graphic brighten;
    [SerializeField] private float brightenAlpha = 1f;

    private RectTransform rt;
    private Graphic ring;
    private float t;
    private float restoreAlpha = -1f;

    private void Awake()
    {
        rt = GetComponent<RectTransform>();
        ring = GetComponent<Graphic>();
    }

    private void OnEnable()
    {
        t = 0f;

        if (liftCanvas != null)
        {
            liftCanvas.overrideSorting = true;
            liftCanvas.sortingOrder = liftSortingOrder;
        }

        if (brighten != null)
        {
            if (restoreAlpha < 0f) restoreAlpha = brighten.color.a;
            Color b = brighten.color;
            b.a = brightenAlpha;
            brighten.color = b;
        }

        Apply(0f);
    }

    private void OnDisable()
    {
        if (liftCanvas != null) liftCanvas.overrideSorting = false;

        if (brighten != null && restoreAlpha >= 0f)
        {
            Color b = brighten.color;
            b.a = restoreAlpha;
            brighten.color = b;
            restoreAlpha = -1f;
        }

        SetRingAlpha(0f);
    }

    private void Update()
    {
        float cycle = Mathf.Max(0.05f, pulseDuration + gapDuration);
        t += Time.unscaledDeltaTime;
        while (t >= cycle) t -= cycle;
        Apply(t);
    }

    /// <summary>Places the ring for one point in the pulse cycle.</summary>
    private void Apply(float time)
    {
        if (pulseDuration <= 0f || time > pulseDuration)
        {
            if (rt != null) rt.localScale = Vector3.one * startScale;
            SetRingAlpha(0f);
            return;
        }

        float p = Mathf.Clamp01(time / pulseDuration);
        if (rt != null) rt.localScale = Vector3.one * Mathf.Lerp(startScale, endScale, p);

        // full strength for the first sliver of the travel, then fade out
        float fade = 1f - Mathf.Clamp01((p - 0.15f) / 0.85f);
        SetRingAlpha(peakAlpha * fade);
    }

    private void SetRingAlpha(float a)
    {
        if (ring == null) return;
        Color c = ring.color;
        c.a = a;
        ring.color = c;
    }
}

using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Looping hand hint that shows the player what to do with the screen.
/// Everything runs on unscaled time so it keeps animating while the tutorial is
/// holding the world at Time.timeScale = 0.
///
///   Tap  -- the hand sinks, springs straight back and a ripple spreads out.
///   Hold -- the hand sinks and stays down for a beat with the ring sitting
///           under it and breathing, so it reads as "press and keep pressing".
/// </summary>
[RequireComponent(typeof(RectTransform))]
public class TapHandHint : MonoBehaviour
{
    public enum HintMode
    {
        Tap,
        Hold
    }

    [Header("Mode")]
    [Tooltip("Tap = quick press and release. Hold = press and stay down.")]
    [SerializeField] private HintMode mode = HintMode.Tap;

    [Header("Cycle (unscaled seconds)")]
    [SerializeField] private float pressDuration = 0.16f;
    [Tooltip("Hold mode only: how long the finger stays pressed before letting go.")]
    [SerializeField] private float holdDuration = 1.5f;
    [SerializeField] private float releaseDuration = 0.24f;
    [Tooltip("Pause before the next press.")]
    [SerializeField] private float restDuration = 0.55f;

    [Header("Hand")]
    [Tooltip("How small the hand gets at the bottom of the press.")]
    [SerializeField] private float pressScale = 0.82f;
    [Tooltip("Pixels the hand sinks on the press.")]
    [SerializeField] private float pressDrop = 24f;

    [Header("Ring")]
    [Tooltip("Circle under the fingertip. Optional.")]
    [SerializeField] private RectTransform ripple;
    [Tooltip("Tap mode: the ring starts here and spreads to the end scale.")]
    [SerializeField] private float rippleStartScale = 0.3f;
    [SerializeField] private float rippleEndScale = 1.55f;
    [SerializeField] private float rippleAlpha = 0.75f;
    [Tooltip("Hold mode: the ring sits at this scale and breathes while the finger is down.")]
    [SerializeField] private float holdRingScale = 1.05f;
    [Tooltip("Hold mode: how much the ring swells and shrinks as it breathes.")]
    [SerializeField] private float holdRingBreath = 0.08f;
    [Tooltip("Hold mode: breaths per second while the finger is down.")]
    [SerializeField] private float holdRingBreathRate = 1.6f;

    private RectTransform rt;
    private Graphic rippleGraphic;
    private Vector2 restPos;
    private Vector3 restScale;
    private float t;

    private void Awake()
    {
        rt = GetComponent<RectTransform>();
        restPos = rt.anchoredPosition;
        restScale = rt.localScale;
        if (ripple != null) rippleGraphic = ripple.GetComponent<Graphic>();
    }

    private void OnEnable()
    {
        t = 0f;
        Apply(0f);
    }

    private void OnDisable()
    {
        if (rt == null) return;
        rt.anchoredPosition = restPos;
        rt.localScale = restScale;
    }

    private void Update()
    {
        float cycle = Mathf.Max(0.05f, CycleLength());
        t += Time.unscaledDeltaTime;
        while (t >= cycle) t -= cycle;
        Apply(t);
    }

    private float CycleLength()
    {
        float hold = mode == HintMode.Hold ? holdDuration : 0f;
        return pressDuration + hold + releaseDuration + restDuration;
    }

    /// <summary>Places the hand and ring for one point in the cycle.</summary>
    private void Apply(float time)
    {
        float hold = mode == HintMode.Hold ? holdDuration : 0f;

        // k: 0 = hand at rest, 1 = fully pressed
        float k;
        float pressProgress = -1f;    // 0..1 while sinking
        float holdProgress = -1f;     // 0..1 while held down
        float releaseProgress = -1f;  // 0..1 while springing back

        if (time < pressDuration)
        {
            pressProgress = pressDuration <= 0f ? 1f : Mathf.Clamp01(time / pressDuration);
            k = Mathf.Sin(pressProgress * Mathf.PI * 0.5f);
        }
        else if (time < pressDuration + hold)
        {
            holdProgress = hold <= 0f ? 1f : Mathf.Clamp01((time - pressDuration) / hold);
            k = 1f;
        }
        else if (time < pressDuration + hold + releaseDuration)
        {
            releaseProgress = releaseDuration <= 0f ? 1f : Mathf.Clamp01((time - pressDuration - hold) / releaseDuration);
            k = 1f - Mathf.Sin(releaseProgress * Mathf.PI * 0.5f);
        }
        else
        {
            k = 0f;
        }

        rt.localScale = restScale * Mathf.Lerp(1f, pressScale, k);
        rt.anchoredPosition = restPos + new Vector2(0f, -pressDrop * k);

        if (ripple == null) return;

        if (mode == HintMode.Hold)
        {
            ApplyHoldRing(pressProgress, holdProgress, releaseProgress);
        }
        else
        {
            ApplyTapRing(releaseProgress);
        }
    }

    /// <summary>Tap: the ring spreads out and fades as the finger lifts.</summary>
    private void ApplyTapRing(float releaseProgress)
    {
        if (releaseProgress < 0f)
        {
            ripple.localScale = Vector3.one * rippleStartScale;
            SetRippleAlpha(0f);
            return;
        }

        ripple.localScale = Vector3.one * Mathf.Lerp(rippleStartScale, rippleEndScale, releaseProgress);
        SetRippleAlpha(rippleAlpha * (1f - releaseProgress));
    }

    /// <summary>Hold: the ring closes in under the finger, then sits there breathing.</summary>
    private void ApplyHoldRing(float pressProgress, float holdProgress, float releaseProgress)
    {
        if (pressProgress >= 0f)
        {
            // ring contracts onto the fingertip as the hand comes down
            ripple.localScale = Vector3.one * Mathf.Lerp(rippleEndScale, holdRingScale, pressProgress);
            SetRippleAlpha(rippleAlpha * pressProgress);
            return;
        }

        if (holdProgress >= 0f)
        {
            float breath = Mathf.Sin(holdProgress * holdDuration * holdRingBreathRate * Mathf.PI * 2f);
            ripple.localScale = Vector3.one * (holdRingScale + holdRingBreath * breath);
            SetRippleAlpha(rippleAlpha * (0.72f + 0.28f * breath));
            return;
        }

        if (releaseProgress >= 0f)
        {
            ripple.localScale = Vector3.one * Mathf.Lerp(holdRingScale, rippleEndScale, releaseProgress);
            SetRippleAlpha(rippleAlpha * (1f - releaseProgress));
            return;
        }

        ripple.localScale = Vector3.one * rippleStartScale;
        SetRippleAlpha(0f);
    }

    private void SetRippleAlpha(float a)
    {
        if (rippleGraphic == null) return;
        Color c = rippleGraphic.color;
        c.a = a;
        rippleGraphic.color = c;
    }
}

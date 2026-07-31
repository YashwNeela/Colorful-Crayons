using UnityEngine;
using DG.Tweening;

/// <summary>
/// Tap / hold to thrust upward, release to fall. The kid rides a rocket and
/// arcs through the level leaving a coloured trail behind.
/// </summary>
[RequireComponent(typeof(CircleCollider2D))]
public class RocketPlayer : MonoBehaviour
{
    [Header("Flight")]
    [SerializeField] private float forwardSpeed = 7.5f;
    [SerializeField] private float thrustAccel = 42f;
    [SerializeField] private float gravityAccel = 30f;
    [SerializeField] private float maxRiseSpeed = 10f;
    [SerializeField] private float maxFallSpeed = -14f;
    [SerializeField] private float ceilingY = 6.6f;

    [Header("Look")]
    [SerializeField] private Transform visual;
    [SerializeField] private Transform flame;
    [SerializeField] private TrailRenderer trail;
    [SerializeField] private float maxTiltDegrees = 42f;

    [Header("Ground Bounce")]
    [Tooltip("How high the little hop rises, in world units.")]
    [SerializeField] private float bounceHeight = 0.3f;
    [Tooltip("Total time for one up-down hop, in seconds.")]
    [SerializeField] private float bounceDuration = 0.25f;

    [Header("Refs")]
    [SerializeField] private GameFlow flow;
    [Tooltip("Optional. Only needed so the rocket can land on flat ground/grass, not just floating platforms. Auto-found if left empty.")]
    [SerializeField] private LevelBuilder level;

    [Header("Landing")]
    [SerializeField] private float bodyRadius = 0.42f;
    [SerializeField] private float snapTolerance = 0.35f;

    private float verticalSpeed;
    private bool thrusting;
    private bool alive = true;
    private float flameBaseScale = 1f;
    private bool grounded;

    private Vector3 logicalPos;

    private float bounceOffsetY;
    private Tween bounceTween;

    private readonly Collider2D[] overlapBuffer = new Collider2D[12];

    public float ForwardSpeed { get { return forwardSpeed; } }
    public bool Alive { get { return alive; } }
    public bool Grounded { get { return grounded; } }

    private void Awake()
    {
        if (flow == null) flow = FindObjectOfType<GameFlow>();
        if (level == null) level = FindObjectOfType<LevelBuilder>();
        if (flame != null) flameBaseScale = flame.localScale.x;
        logicalPos = transform.position;
    }

    private void Update()
    {
        if (!alive) return;

        bool wasGrounded = grounded;

        thrusting = ReadInput();

        verticalSpeed += (thrusting ? thrustAccel : -gravityAccel) * Time.deltaTime;
        verticalSpeed = Mathf.Clamp(verticalSpeed, maxFallSpeed, maxRiseSpeed);

        Vector3 pos = logicalPos;
        float previousFoot = pos.y - bodyRadius;
        pos.x += forwardSpeed * Time.deltaTime;
        pos.y += verticalSpeed * Time.deltaTime;

        if (pos.y > ceilingY)
        {
            pos.y = ceilingY;
            if (verticalSpeed > 0f) verticalSpeed = 0f;
        }

        TryLand(ref pos, previousFoot);

        logicalPos = pos;

        if (grounded && !wasGrounded)
        {
            PlayBounce();
        }

        Vector3 visualPos = pos;
        visualPos.y += bounceOffsetY;
        transform.position = visualPos;

        UpdateVisual();
    }

    private void TryLand(ref Vector3 pos, float previousFoot)
    {
        grounded = false;
        if (verticalSpeed > 0f) return;

        int count = Physics2D.OverlapCircleNonAlloc(pos, bodyRadius, overlapBuffer);
        for (int i = 0; i < count; i++)
        {
            Collider2D c = overlapBuffer[i];
            if (c == null) continue;

            Platform plat = c.GetComponent<Platform>();
            if (plat == null) continue;

            float top = plat.TopY;
            float foot = pos.y - bodyRadius;

            if (previousFoot < top - snapTolerance) continue;
            if (foot > top) continue;

            pos.y = top + bodyRadius;
            verticalSpeed = 0f;
            grounded = true;
            return;
        }

        if (level != null && !level.IsWaterAt(pos.x))
        {
            float top = level.GroundTopY;
            float foot = pos.y - bodyRadius;

            if (previousFoot >= top - snapTolerance && foot <= top)
            {
                pos.y = top + bodyRadius;
                verticalSpeed = 0f;
                grounded = true;
            }
        }
    }

    private void PlayBounce()
    {
        if (bounceTween != null && bounceTween.IsActive()) bounceTween.Kill();

        bounceOffsetY = 0f;
        Sequence seq = DOTween.Sequence();
        seq.Append(DOTween.To(() => bounceOffsetY, v => bounceOffsetY = v, bounceHeight, bounceDuration * 0.5f).SetEase(Ease.OutQuad));
        seq.Append(DOTween.To(() => bounceOffsetY, v => bounceOffsetY = v, 0f, bounceDuration * 0.5f).SetEase(Ease.InQuad));
        bounceTween = seq;
    }

    private bool ReadInput()
    {
        if (Input.GetMouseButton(0)) return true;
        if (Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W)) return true;
        for (int i = 0; i < Input.touchCount; i++)
        {
            TouchPhase p = Input.GetTouch(i).phase;
            if (p != TouchPhase.Ended && p != TouchPhase.Canceled) return true;
        }
        return false;
    }

    private void UpdateVisual()
    {
        if (visual != null)
        {
            float tilt = Mathf.Clamp(Mathf.Atan2(verticalSpeed, forwardSpeed) * Mathf.Rad2Deg, -maxTiltDegrees, maxTiltDegrees);
            visual.rotation = Quaternion.Lerp(visual.rotation, Quaternion.Euler(0f, 0f, tilt), 14f * Time.deltaTime);
        }

        if (flame != null)
        {
            float target = thrusting ? flameBaseScale * (1f + 0.25f * Mathf.Sin(Time.time * 40f)) : 0f;
            Vector3 s = flame.localScale;
            s.x = Mathf.Lerp(s.x, target, 22f * Time.deltaTime);
            flame.localScale = s;
        }
    }

    public void SetTrailColor(Color c)
    {
        if (trail == null) return;
        var grad = new Gradient();
        grad.SetKeys(
            new GradientColorKey[] { new GradientColorKey(c, 0f), new GradientColorKey(c, 1f) },
            new GradientAlphaKey[] { new GradientAlphaKey(1f, 0f), new GradientAlphaKey(0f, 1f) });
        trail.colorGradient = grad;
    }

    public void Crash()
    {
        if (!alive) return;
        alive = false;
        if (trail != null) trail.emitting = false;
        if (bounceTween != null && bounceTween.IsActive()) bounceTween.Kill();
        if (flow != null) flow.OnPlayerCrashed(transform.position);
    }

    public void Respawn(Vector3 pos)
    {
        logicalPos = pos;
        transform.position = pos;
        verticalSpeed = 0f;
        alive = true;
        grounded = false;
        bounceOffsetY = 0f;
        if (bounceTween != null && bounceTween.IsActive()) bounceTween.Kill();
        if (trail != null)
        {
            trail.Clear();
            trail.emitting = true;
        }
        if (visual != null) visual.rotation = Quaternion.identity;
    }
}

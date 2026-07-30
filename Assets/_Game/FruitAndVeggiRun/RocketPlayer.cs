using UnityEngine;

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

    [Header("Refs")]
    [SerializeField] private GameFlow flow;

    [Header("Landing")]
    [SerializeField] private float bodyRadius = 0.42f;
    [SerializeField] private float snapTolerance = 0.35f;

    private float verticalSpeed;
    private bool thrusting;
    private bool alive = true;
    private float flameBaseScale = 1f;
    private bool grounded;
    private readonly Collider2D[] overlapBuffer = new Collider2D[12];

    public float ForwardSpeed { get { return forwardSpeed; } }
    public bool Alive { get { return alive; } }
    public bool Grounded { get { return grounded; } }

    private void Awake()
    {
        if (flow == null) flow = FindObjectOfType<GameFlow>();
        if (flame != null) flameBaseScale = flame.localScale.x;
    }

    private void Update()
    {
        if (!alive) return;

        thrusting = ReadInput();

        verticalSpeed += (thrusting ? thrustAccel : -gravityAccel) * Time.deltaTime;
        verticalSpeed = Mathf.Clamp(verticalSpeed, maxFallSpeed, maxRiseSpeed);

        Vector3 pos = transform.position;
        float previousFoot = pos.y - bodyRadius;
        pos.x += forwardSpeed * Time.deltaTime;
        pos.y += verticalSpeed * Time.deltaTime;

        if (pos.y > ceilingY)
        {
            pos.y = ceilingY;
            if (verticalSpeed > 0f) verticalSpeed = 0f;
        }

        TryLandOnPlatform(ref pos, previousFoot);

        transform.position = pos;

        UpdateVisual();
    }

    /// <summary>
    /// One-way platform landing: only catches the player when they were above
    /// the surface last frame, so you can still fly up through a ledge.
    /// </summary>
    private void TryLandOnPlatform(ref Vector3 pos, float previousFoot)
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

            // must have been at or above the surface a frame ago
            if (previousFoot < top - snapTolerance) continue;
            if (foot > top) continue;

            pos.y = top + bodyRadius;
            verticalSpeed = 0f;
            grounded = true;
            return;
        }
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

    /// <summary>Called when the player flies into a hazard or off the bottom.</summary>
    public void Crash()
    {
        if (!alive) return;
        alive = false;
        if (trail != null) trail.emitting = false;
        if (flow != null) flow.OnPlayerCrashed(transform.position);
    }

    /// <summary>Put the player back in the air after a crash.</summary>
    public void Respawn(Vector3 pos)
    {
        transform.position = pos;
        verticalSpeed = 0f;
        alive = true;
        if (trail != null)
        {
            trail.Clear();
            trail.emitting = true;
        }
        if (visual != null) visual.rotation = Quaternion.identity;
    }
}

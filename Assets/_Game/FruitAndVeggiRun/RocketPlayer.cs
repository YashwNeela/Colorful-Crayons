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

    private float verticalSpeed;
    private bool thrusting;
    private bool alive = true;
    private float flameBaseScale = 1f;

    public float ForwardSpeed { get { return forwardSpeed; } }
    public bool Alive { get { return alive; } }

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
        pos.x += forwardSpeed * Time.deltaTime;
        pos.y += verticalSpeed * Time.deltaTime;

        if (pos.y > ceilingY)
        {
            pos.y = ceilingY;
            if (verticalSpeed > 0f) verticalSpeed = 0f;
        }

        transform.position = pos;

        UpdateVisual();
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

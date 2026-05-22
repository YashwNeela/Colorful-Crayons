using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace TMKOC.StarLink
{
    public class Comet : MonoBehaviour
    {

        private StarLinkLevel cachedLevel;
        private float cometRadius = 0f;

        [Header("Alignment Check")]
        [SerializeField] private float alignmentTolerance = 0.05f;  // small forgiveness
        [Header("Settings")]
        public float launchSpeed = 10f;
        public Star currentStar;

        private Rigidbody2D rb;
        private bool isOrbiting = false;
        private float currentAngle = 0f; // Angle in degrees

        public TrailRenderer trailRenderer;

        [Header("Launch Forgiveness")]
        [SerializeField] private bool useAimAssist = true;
        [SerializeField] private float forgivenessAngle = 15f;

        [Range(0f, 1f)]
[SerializeField] private float greenZoneFraction = 1f;
// 1.0 = green covers the entire forgiveness band (green = "will hit, period")
// 0.7 = green covers inner 70%, outer 30% is silent assist
// 0.5 = green is strict, assist quietly catches the rest


        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            if (rb == null)
            {
                rb = gameObject.AddComponent<Rigidbody2D>();
            }
            rb.isKinematic = true; // We control movement manually

            CircleCollider2D myCol = GetComponent<CircleCollider2D>();
            if (myCol != null)
                cometRadius = myCol.radius * Mathf.Max(transform.lossyScale.x, transform.lossyScale.y);
        }


        private void Start()
        {

            cachedLevel = FindObjectOfType<StarLinkLevel>();
            if (currentStar != null)
            {
                AttachToStar(currentStar);
            }
        }

        private void Update()
        {
            if (isOrbiting)
            {
                HandleOrbiting();
                CheckLaunchAlignment();
                // Tap to launch
                if (Input.GetMouseButtonDown(0) && StarLinkGameManager.Instance.CurrentGameState == GameState.Playing)
                {
                    Launch();
                }
            }
        }

       private void CheckLaunchAlignment()
{
    if (cachedLevel == null)
    {
        cachedLevel = FindObjectOfType<StarLinkLevel>();
        if (cachedLevel == null) return;
    }

    Star target = cachedLevel.CurrentTargetStar;
    if (target == null)
    {
        LineDrawer.Instance.SetDottedLineAligned(false);
        return;
    }

    float rad = currentAngle * Mathf.Deg2Rad;
    Vector2 tangent = new Vector2(-Mathf.Sin(rad), Mathf.Cos(rad)).normalized;

    Vector2 toTarget = (Vector2)target.transform.position - (Vector2)transform.position;

    // Target must be in front of the launch direction
    if (Vector2.Dot(tangent, toTarget) <= 0f)
    {
        LineDrawer.Instance.SetDottedLineAligned(false);
        return;
    }

    float angleOff = Vector2.Angle(tangent, toTarget.normalized);
    bool aligned = angleOff <= forgivenessAngle * greenZoneFraction;

    LineDrawer.Instance.SetDottedLineAligned(aligned);
}

        public void AttachToStar(Star star)
        {
            StartCoroutine(StaticCoroutine.Co_GenericCoroutine(0.5f, () =>
            {
                trailRenderer.emitting = true;
            }));


            currentStar = star;
            isOrbiting = true;
            rb.linearVelocity = Vector2.zero; // Stop any existing movement

            // Calculate initial angle based on current position relative to star, or just default to 0
            Vector2 dir = transform.position - star.transform.position;
            if (dir.sqrMagnitude > 0.01f)
            {
                currentAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            }
            else
            {
                currentAngle = 0f;
            }

            UpdatePositionFromAngle();
        }

        private void HandleOrbiting()
        {
            if (currentStar == null) return;

            // Increase angle based on star's orbit speed
            currentAngle += currentStar.orbitSpeed * Time.deltaTime;
            currentAngle %= 360f;

            UpdatePositionFromAngle();
        }

        private void UpdatePositionFromAngle()
        {
            float rad = currentAngle * Mathf.Deg2Rad;
            float x = Mathf.Cos(rad) * currentStar.orbitRadius;
            float y = Mathf.Sin(rad) * currentStar.orbitRadius;

            transform.position = currentStar.transform.position + new Vector3(x, y, 0);

            // Calculate tangential direction for visual rotation (optional)
            Vector2 tangent = new Vector2(-Mathf.Sin(rad), Mathf.Cos(rad));
            transform.up = tangent;
        }

        private void Launch()
        {
            isOrbiting = false;

            float rad = currentAngle * Mathf.Deg2Rad;
            Vector2 tangent = new Vector2(-Mathf.Sin(rad), Mathf.Cos(rad)).normalized;

            Vector2 launchDirection = tangent;

            // --- Aim assist: nudge near-misses into guaranteed hits ---
            if (useAimAssist && cachedLevel != null)
            {
                Star target = cachedLevel.CurrentTargetStar;
                if (target != null)
                {
                    Vector2 toTarget = (Vector2)target.transform.position - (Vector2)transform.position;
                    float dist = toTarget.magnitude;

                    if (dist > 0.01f)
                    {
                        Vector2 toTargetDir = toTarget / dist;

                        // Only assist if target is generally in front of the launch direction
                        if (Vector2.Dot(tangent, toTargetDir) > 0f)
                        {
                            float angleOff = Vector2.Angle(tangent, toTargetDir);
                            if (angleOff <= forgivenessAngle)
                            {
                                launchDirection = toTargetDir;
                            }
                        }
                    }
                }
            }

            rb.linearVelocity = launchDirection * launchSpeed;

            LineDrawer.Instance.SetDottedLineAligned(false);
        }

        /// <summary>
        /// Sent when another object enters a trigger collider attached to this
        /// object (2D physics only).
        /// </summary>
        /// <param name="other">The other Collider2D involved in this collision.</param>
        void OnTriggerEnter2D(Collider2D other)
        {


            if (!isOrbiting) // Only detect hits while flying
            {
                Star hitStar = other.GetComponent<Star>();
                if (hitStar != null)
                {
                    // Check with Level Manager if this is the correct target
                    StarLinkLevel currentLevel = FindObjectOfType<StarLinkLevel>();
                    if (currentLevel != null)
                    {
                        if (hitStar.IsTarget())
                        {
                            Debug.Log("Attaching to star");
                            // Correct star!
                            AttachToStar(hitStar);
                            currentLevel.OnStarHit(hitStar);
                        }
                        else
                        {
                            // Wrong star hit or hit already active star
                            // Might want to bounce off or ignore
                        }
                    }
                }
            }
        }


        private void OnBecameInvisible()
        {
            if (!isOrbiting)
            {
                // Comet flew off screen
                trailRenderer.emitting = false;
                StarLinkLevel currentLevel = FindObjectOfType<StarLinkLevel>();
                if (currentLevel != null)
                {
                    currentLevel.OnCometMissed();
                }

                // Reset to current active star
                if (currentStar != null)
                {
                    StartCoroutine(StaticCoroutine.Co_GenericCoroutine(1, () =>
                    {
                        AttachToStar(currentStar);
                    }));
                }
            }
        }
    }
}

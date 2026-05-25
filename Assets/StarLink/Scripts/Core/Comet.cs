using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

namespace TMKOC.StarLink
{
    public class Comet : MonoBehaviour
    {
        [Header("Settings")]
        public float launchSpeed = 10f;
        public Star currentStar;
        public TrailRenderer trailRenderer;

        public bool canLaunch = true;

        [Header("Launch Forgiveness")]
        [SerializeField] private bool useAimAssist = true;
        [SerializeField] private float forgivenessAngle = 15f;
        [Range(0f, 1f)]
        [SerializeField] private float greenZoneFraction = 1f;
        // 1.0 = green covers the entire forgiveness band (green = "will hit, period")
        // 0.7 = green covers inner 70%, outer 30% is silent assist
        // 0.5 = green is strict, assist quietly catches the rest

        [Header("Sweet Spot Pause")]
        [SerializeField] private bool pauseAtSweetSpot = false;

        [Header("Easy Mode Settings")]
        [SerializeField] private float requiredRotationsInEasyMode = 2f;

        [Header("Inactivity Hint")]
        [SerializeField] private float inactivityThreshold = 5f;
        [SerializeField] private UnityEvent onPlayerInactive;
        [SerializeField] private UnityEvent onPlayerResumed;

        [Header("Auto-Pause After Misses")]
        [SerializeField] private int missesBeforeAutoPause = 5;

        // --- Internal state ---
        private Rigidbody2D rb;
        private bool isOrbiting = false;
        private float currentAngle = 0f;

        private StarLinkLevel cachedLevel;
        private bool isAlignedCached = false;

        private bool consumePauseOnLaunch = false;
        private bool waitingForUnalignFirst = false;
        private bool easyModeActive = false;
        private float angleTraveledThisOrbit = 0f;

        private float timeSinceOrbitStart = 0f;
        private bool inactivityFired = false;

        private float originalTrailTime = 0f;
        private bool trailFrozen = false;
        private bool wasPaused = false;

        private int consecutiveMisses = 0;

        // ============================================================
        //  Lifecycle
        // ============================================================

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            if (rb == null)
                rb = gameObject.AddComponent<Rigidbody2D>();
            rb.isKinematic = true;

            if (trailRenderer != null)
                originalTrailTime = trailRenderer.time;
        }

        private void Start()
        {
            cachedLevel = FindObjectOfType<StarLinkLevel>();

            if (currentStar != null)
                AttachToStar(currentStar);
        }

        private void Update()
        {
            if (!isOrbiting) return;

            // 1. Alignment check drives both the green line and the pause
            isAlignedCached = ComputeAlignment();
            LineDrawer.Instance.SetDottedLineAligned(isAlignedCached);

            // 2. Inactivity tracking — fire once if player hasn't launched in N seconds
            timeSinceOrbitStart += Time.deltaTime;
            if (!inactivityFired && timeSinceOrbitStart >= inactivityThreshold)
            {
                inactivityFired = true;
                OnPlayerInactive();
            }

            // 3. Once the comet has left the alignment window, arm the real pause
            if (waitingForUnalignFirst && !isAlignedCached)
                waitingForUnalignFirst = false;

            // 4. In easy mode, require N full rotations before allowing pause
            bool meetsRotationRequirement =
                !easyModeActive ||
                angleTraveledThisOrbit >= requiredRotationsInEasyMode * 360f;

            bool shouldPause =
                pauseAtSweetSpot &&
                isAlignedCached &&
                !waitingForUnalignFirst &&
                meetsRotationRequirement;

            // Freeze/unfreeze the trail on pause transitions
            if (shouldPause && !wasPaused) FreezeTrail();
            else if (!shouldPause && wasPaused) UnfreezeTrail();
            wasPaused = shouldPause;

            if (shouldPause)
                UpdatePositionFromAngle();
            else
                HandleOrbiting();

            // 5. Tap handling
            if (Input.GetMouseButtonDown(0) &&
                StarLinkGameManager.Instance.CurrentGameState == GameState.Playing)
            {
                // Any tap resets the inactivity timer (player is engaged)
                timeSinceOrbitStart = 0f;

                // If the inactive event had fired, fire the resume event now
                if (inactivityFired)
                {
                    inactivityFired = false;
                    OnPlayerResumed();
                }

                // In pause mode, ignore taps unless comet is at the sweet spot
                if (pauseAtSweetSpot && !isAlignedCached)
                {
                    // Optional feedback: small shake or "not yet" sound
                    // transform.DOShakePosition(0.2f, 0.05f, 20, 90, false, true);
                    return;
                }

                Launch();
            }
        }

        // ============================================================
        //  Orbit math
        // ============================================================

        private void HandleOrbiting()
        {
            if (currentStar == null) return;

            float delta = currentStar.orbitSpeed * Time.deltaTime;
            currentAngle += delta;
            angleTraveledThisOrbit += Mathf.Abs(delta);
            currentAngle %= 360f;

            UpdatePositionFromAngle();
        }

        private void UpdatePositionFromAngle()
        {
            float rad = currentAngle * Mathf.Deg2Rad;
            float x = Mathf.Cos(rad) * currentStar.orbitRadius;
            float y = Mathf.Sin(rad) * currentStar.orbitRadius;

            transform.position = currentStar.transform.position + new Vector3(x, y, 0);

            Vector2 tangent = new Vector2(-Mathf.Sin(rad), Mathf.Cos(rad));
            transform.up = tangent;
        }

        private bool ComputeAlignment()
        {
            if (cachedLevel == null)
            {
                cachedLevel = FindObjectOfType<StarLinkLevel>();
                if (cachedLevel == null) return false;
            }

            Star target = cachedLevel.CurrentTargetStar;
            if (target == null) return false;

            float rad = currentAngle * Mathf.Deg2Rad;
            Vector2 tangent = new Vector2(-Mathf.Sin(rad), Mathf.Cos(rad)).normalized;

            Vector2 toTarget = (Vector2)target.transform.position - (Vector2)transform.position;
            if (Vector2.Dot(tangent, toTarget) <= 0f) return false;

            float angleOff = Vector2.Angle(tangent, toTarget.normalized);
            return angleOff <= forgivenessAngle * greenZoneFraction;
        }

        // ============================================================
        //  Attach / Launch
        // ============================================================

        public void AttachToStar(Star star)
        {
            StartCoroutine(StaticCoroutine.Co_GenericCoroutine(0.5f, () =>
            {
                trailRenderer.emitting = true;
            }));

            currentStar = star;
            isOrbiting = true;
            rb.linearVelocity = Vector2.zero;

            Vector2 dir = transform.position - star.transform.position;
            if (dir.sqrMagnitude > 0.01f)
                currentAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            else
                currentAngle = 0f;

            UpdatePositionFromAngle();

            // Reset rotation counter for the new orbit
            angleTraveledThisOrbit = 0f;

            // Reset inactivity timer for the new orbit
            timeSinceOrbitStart = 0f;
            inactivityFired = false;
        }

        public void Launch()
        {
            UnfreezeTrail();
            isOrbiting = false;

            float rad = currentAngle * Mathf.Deg2Rad;
            Vector2 tangent = new Vector2(-Mathf.Sin(rad), Mathf.Cos(rad)).normalized;
            Vector2 launchDirection = tangent;

            // --- Aim assist: snap near-misses into guaranteed hits ---
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

                        if (Vector2.Dot(tangent, toTargetDir) > 0f)
                        {
                            float angleOff = Vector2.Angle(tangent, toTargetDir);
                            if (angleOff <= forgivenessAngle)
                                launchDirection = toTargetDir;
                        }
                    }
                }
            }

            rb.linearVelocity = launchDirection * launchSpeed;
            LineDrawer.Instance.SetDottedLineAligned(false);

            if (consumePauseOnLaunch)
            {
                pauseAtSweetSpot = false;
                consumePauseOnLaunch = false;
            }
        }

        // ============================================================
        //  Public API
        // ============================================================

        public void SetForgivenessAngle(float value)
        {
            forgivenessAngle = value;
        }

        /// <summary>
        /// Comet will stop at the next sweet spot and wait for a tap.
        /// Auto-disables after the player launches.
        /// </summary>
        public void PauseAtNextSweetSpot()
        {
            pauseAtSweetSpot = true;
            consumePauseOnLaunch = true;
            waitingForUnalignFirst = isAlignedCached;
        }

        /// <summary>
        /// Easy mode — comet always pauses at sweet spots until disabled,
        /// after orbiting N full rotations first. Taps outside the sweet
        /// spot are ignored.
        /// </summary>
        public void SetSweetSpotPauseMode(bool enabled)
        {
            pauseAtSweetSpot = enabled;
            consumePauseOnLaunch = false;
            waitingForUnalignFirst = enabled && isAlignedCached;
            easyModeActive = enabled;
            angleTraveledThisOrbit = 0f;
        }

        /// <summary>
        /// Force-resume the orbit immediately, even if paused at a sweet spot.
        /// </summary>
        public void ResumeOrbit()
        {
            pauseAtSweetSpot = false;
            consumePauseOnLaunch = false;
            waitingForUnalignFirst = false;
            easyModeActive = false;
        }

        /// <summary>
        /// Called once when the player hasn't launched within the inactivity threshold.
        /// </summary>
        protected virtual void OnPlayerInactive()
        {
            onPlayerInactive?.Invoke();
            if (StarLinkGameManager.Instance.CurrentGameState == GameState.Playing)
                StarlinkUI.Instance.fingerObj.SetActive(true);
        }

        /// <summary>
        /// Called when the player taps after the inactivity event had fired.
        /// </summary>
        protected virtual void OnPlayerResumed()
        {
            onPlayerResumed?.Invoke();
            StarlinkUI.Instance.fingerObj.SetActive(false);
        }

        // ============================================================
        //  Trail helpers
        // ============================================================

        private void FreezeTrail()
        {
            if (trailRenderer == null || trailFrozen) return;
            trailRenderer.time = Mathf.Infinity;
            trailRenderer.emitting = false;
            trailFrozen = true;
        }

        private void UnfreezeTrail()
        {
            if (trailRenderer == null || !trailFrozen) return;
            trailRenderer.time = originalTrailTime;
            trailRenderer.emitting = true;
            trailFrozen = false;
        }

        // ============================================================
        //  Collision / Off-screen
        // ============================================================

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (isOrbiting) return; // Only detect hits while flying

            Star hitStar = other.GetComponent<Star>();
            if (hitStar == null) return;

            StarLinkLevel currentLevel = FindObjectOfType<StarLinkLevel>();
            if (currentLevel == null) return;

            if (hitStar.IsTarget())
            {
                consecutiveMisses = 0;   // Reset on successful hit
                Debug.Log("Attaching to star");
                AttachToStar(hitStar);
                currentLevel.OnStarHit(hitStar);
            }
            // else: wrong star — ignore for now (could bounce off)
        }

        private void OnBecameInvisible()
        {
            if (isOrbiting) return;

            // Comet flew off screen
            trailRenderer.emitting = false;

            // Count this miss and auto-pause at sweet spot if threshold reached
            consecutiveMisses++;
            if (consecutiveMisses >= missesBeforeAutoPause)
            {
                PauseAtNextSweetSpot();
                SetSweetSpotPauseMode(true);
                consecutiveMisses = 0;   // reset so it doesn't fire every miss after
                Debug.Log("Triggered sweet-spot pause after repeated misses");
            }

            StarLinkLevel currentLevel = FindObjectOfType<StarLinkLevel>();
            if (currentLevel != null)
                currentLevel.OnCometMissed();

            // Snap back to current active star after a delay
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
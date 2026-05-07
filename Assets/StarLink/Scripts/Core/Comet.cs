using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace TMKOC.StarLink
{
    public class Comet : MonoBehaviour
    {
        [Header("Settings")]
        public float launchSpeed = 10f;
        public Star currentStar;

        private Rigidbody2D rb;
        private bool isOrbiting = false;
        private float currentAngle = 0f; // Angle in degrees

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            if (rb == null)
            {
                rb = gameObject.AddComponent<Rigidbody2D>();
            }
            rb.isKinematic = true; // We control movement manually
        }

        private void Start()
        {
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

                // Tap to launch
                if (Input.GetMouseButtonDown(0))
                {
                    Launch();
                }
            }
        }

        public void AttachToStar(Star star)
        {
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
            
            // Tangential direction is perpendicular to radius
            float rad = currentAngle * Mathf.Deg2Rad;
            Vector2 tangent = new Vector2(-Mathf.Sin(rad), Mathf.Cos(rad)).normalized;

            // Apply velocity
            rb.linearVelocity = tangent * launchSpeed;
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
                StarLinkLevel currentLevel = FindObjectOfType<StarLinkLevel>();
                if (currentLevel != null)
                {
                    currentLevel.OnCometMissed();
                }
                
                // Reset to current active star
                if (currentStar != null)
                {
                    AttachToStar(currentStar);
                }
            }
        }
    }
}

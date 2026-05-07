using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMKOC;

namespace TMKOC.StarLink
{
    public class StarLinkLevel : Level
    {
        [Header("Star-Link Settings")]
        public List<Star> starSequence; // The sequence of stars the comet needs to hit
        public SpriteRenderer constellationArt; // The artwork to reveal at the end
        
        private int currentTargetIndex = 1; // 0 is the starting star, 1 is the first target

        protected override void Awake()
        {
            base.Awake();
        }

        protected override void OnGameStart()
        {
            base.OnGameStart();
            InitializeLevel();
        }

        public virtual void InitializeLevel()
        {
            currentTargetIndex = 1;
            
            if (constellationArt != null)
            {
                // Hide constellation art initially
                Color c = constellationArt.color;
                c.a = 0f;
                constellationArt.color = c;
            }

            if (starSequence != null && starSequence.Count > 0)
            {
                // Setup the initial state
                starSequence[0].SetAsActive(true);
                
                if (starSequence.Count > 1)
                {
                    starSequence[1].SetAsTarget(true);
                }
            }
        }

        public virtual void OnStarHit(Star hitStar)
        {
            if (currentTargetIndex >= starSequence.Count) return;

            // Check if the hit star is the target star
            if (hitStar == starSequence[currentTargetIndex])
            {
                // Correct star hit!
                hitStar.SetAsTarget(false);
                hitStar.SetAsActive(true);
                
                // Previous star is no longer active
                starSequence[currentTargetIndex - 1].SetAsActive(false);

                currentTargetIndex++;

                if (currentTargetIndex < starSequence.Count)
                {
                    // Set next target
                    starSequence[currentTargetIndex].SetAsTarget(true);
                }
                else
                {
                    // Level Completed! All stars hit.
                    OnConstellationComplete();
                }
            }
            else
            {
                // Wrong star hit (optional behavior: maybe ignore or reset)
            }
        }

        public virtual void OnCometMissed()
        {
            // Reset comet to the current active star
            int activeIndex = currentTargetIndex - 1;
            if (activeIndex >= 0 && activeIndex < starSequence.Count)
            {
                Star activeStar = starSequence[activeIndex];
                // TODO: Tell comet to snap back to activeStar
            }
        }

        protected virtual void OnConstellationComplete()
        {
            // Reveal the art and win the game
            if (constellationArt != null)
            {
                StartCoroutine(RevealArtCoroutine());
            }
            else
            {
                GameManager.Instance.GameWin();
            }
        }

        private IEnumerator RevealArtCoroutine()
        {
            // Simple fade in
            float duration = 1.5f;
            float elapsed = 0f;
            Color c = constellationArt.color;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                c.a = Mathf.Lerp(0f, 1f, elapsed / duration);
                constellationArt.color = c;
                yield return null;
            }

            c.a = 1f;
            constellationArt.color = c;

            yield return new WaitForSeconds(1f); // Wait a bit before showing win screen

            GameManager.Instance.GameWin();
        }
    }
}

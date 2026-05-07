using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMKOC;
using Sirenix.OdinInspector;

namespace TMKOC.StarLink
{
    public class StarLinkLevel : Level
    {
        [Header("Star-Link Settings")]
        public List<Star> starSequence; // The sequence of stars the comet needs to hit

        
        private int currentTargetIndex = 1; // 0 is the starting star, 1 is the first target

        public Sprite constellationImage;

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
            
            

            if (starSequence != null && starSequence.Count > 0)
            {
                LineDrawer.Instance.DrawDottedLine(starSequence[0].transform.position, starSequence[1].transform.position);
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

    if (hitStar == starSequence[currentTargetIndex])
    {
        hitStar.SetAsTarget(false);
        hitStar.SetAsActive(true);

        int previousIndex = currentTargetIndex - 1;

        if (previousIndex >= 0)
        {
            starSequence[previousIndex].SetAsActive(false);

            LineDrawer.Instance.DrawHighlightedLine(
                starSequence[previousIndex].transform.position,
                starSequence[currentTargetIndex].transform.position
            );
        }

        currentTargetIndex++;

        if (currentTargetIndex < starSequence.Count)
        {
            LineDrawer.Instance.DrawDottedLine(
                starSequence[currentTargetIndex - 1].transform.position,
                starSequence[currentTargetIndex].transform.position
            );

            starSequence[currentTargetIndex].SetAsTarget(true);
        }
        else
        {
            OnConstellationComplete();
        }
    }
    else
    {
        // Wrong star hit
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

        [Button]
        protected virtual void OnConstellationComplete()
        {
            Debug.Log("On Constellation Completed");
            // Reveal the art and win the game
            if (constellationImage != null)
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
           
            


     

            yield return new WaitForSeconds(1f); // Wait a bit before showing win screen

            StarlinkUI.Instance.Show(constellationImage);

            GameManager.Instance.GameWin();
        }
    }
}

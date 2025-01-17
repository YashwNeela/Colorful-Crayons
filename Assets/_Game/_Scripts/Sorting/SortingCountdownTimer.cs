using TMPro;
using UnityEngine;
using DG.Tweening;
using TMKOC.Sorting;

namespace TMKOC.Sorting
{
    public class SortingCountdownTimer : CountDownTimer
    {
       
        [SerializeField] private DOTweenAnimation dotweenAnimation;         // Reference to the DOTween animation
        [SerializeField] private HUDUI hudUI;

        

       protected override void Start()
        {
            base.Start();
            // Initialize countdown time and color
            
             // Ensure the animation is initially paused (won't play right away)
            if (dotweenAnimation != null)
            {
                dotweenAnimation.DOPause();
            }
           
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            SortingGameManager.OnGameStart += StartCountdown;
        }


        protected override void OnDisable()
        {
            base.OnDisable();
            SortingGameManager.OnGameStart -= StartCountdown;
        }

        protected override void Update()
        {
            base.Update();
            if (isCountingDown && SortingGameManager.Instance.CurrentGameState == GameState.Playing)
            {
                if (currentTime <= thresholdLimit && dotweenAnimation != null)
                {
                    dotweenAnimation.DOPlay();  // Start the animation
                }

                 if (currentTime <= 0f)
                {
                    
                    dotweenAnimation?.DOPause();  // Pause the animation when time is up
        
                }
            }

        }

        // Call this method to start the countdown
        public void StartCountdown()
        {
            countdownDuration = (SortingLevelManager.Instance.GetCurrentLevel() as SortingLevel).LevelTimer;

            currentTime =countdownDuration;
          //  currentTime = countdownDuration;
            isCountingDown = true;
        }

        // Call this method to stop the countdown
        public void StopCountdown()
        {
            isCountingDown = false;
            dotweenAnimation?.DOPause();  // Pause the animation when stopping the countdown
        }

        // Event triggered when the countdown ends
        protected override void TimerEnded()
        {
            base.TimerEnded();
            hudUI.LevelCompleteCheck();
            // Additional code for when the countdown reaches zero
        }
    }
}

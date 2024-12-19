using TMPro;
using UnityEngine;
using DG.Tweening;
using TMKOC.Sorting;  // Import DOTween

namespace TMKOC.Sorting
{
    public class CountdownTimer : SerializedSingleton<CountdownTimer>
    {
        [SerializeField] private TextMeshProUGUI countdownText;
        [SerializeField] private float countdownDuration = 10f;  // Total countdown duration
        [SerializeField] private float thresholdLimit = 3f;      // When to trigger the animation
        [SerializeField] private DOTweenAnimation dotweenAnimation;         // Reference to the DOTween animation
        [SerializeField] private HUDUI hudUI;

        private float currentTime;
        private bool isCountingDown = false;

        void Start()
        {
            // Initialize countdown time and color
            

            // Ensure the animation is initially paused (won't play right away)
            if (dotweenAnimation != null)
            {
                dotweenAnimation.DOPause();
            }
        }

        private void OnEnable()
        {
            SortingGameManager.OnGameStart += StartCountdown;
        }


        private void OnDisable()
        {
            SortingGameManager.OnGameStart -= StartCountdown;
        }
        void Update()
        {
            if (isCountingDown && SortingGameManager.Instance.CurrentGameState == GameState.Playing)
            {
                // Decrease the time
                currentTime -= Time.deltaTime;

                // Update the UI text to show remaining whole seconds
                if (countdownText != null)
                {
                    countdownText.text = Mathf.CeilToInt(Mathf.Clamp(currentTime, 0, countdownDuration)).ToString();

                    // Lerp text color as the timer counts down
                    float t = Mathf.InverseLerp(0, countdownDuration, currentTime);
                }

                // Trigger animation if the time falls below the threshold limit
                if (currentTime <= thresholdLimit && dotweenAnimation != null)
                {
                    dotweenAnimation.DOPlay();  // Start the animation
                }

                // Stop counting down if the timer reaches zero
                if (currentTime <= 0f)
                {
                    currentTime = 0f;
                    isCountingDown = false;
                    dotweenAnimation?.DOPause();  // Pause the animation when time is up
                    TimerEnded();
                }
            }
        }

        // Call this method to start the countdown
        public void StartCountdown()
        {
            countdownDuration = LevelManager.Instance.GetCurrentLevel().LevelTimer;

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
        void TimerEnded()
        {
            Debug.Log("Timer has ended!");
            hudUI.LevelCompleteCheck();
            // Additional code for when the countdown reaches zero
        }
    }
}

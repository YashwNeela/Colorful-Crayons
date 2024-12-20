using System.Collections;
using System.Collections.Generic;
using TMKOC.Sorting;
using TMPro;
using UnityEngine;
using UnityEngine.Accessibility;

namespace TMKOC
{
    public class CountDownTimer : SerializedSingleton<CountDownTimer>
    {
        [SerializeField] protected TextMeshProUGUI countdownText;
        [SerializeField] protected float countdownDuration = 10f;  // Total countdown duration
        [SerializeField] protected float thresholdLimit = 3f;

        protected float currentTime;
        protected bool isCountingDown = false;      // When to trigger the animation

        protected virtual void Start()
        {

        }

        protected virtual void OnEnable()
        {

        }

        protected virtual void OnDisable()
        {

        }

        protected virtual void Update()
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


                // Stop counting down if the timer reaches zero
                if (currentTime <= 0f)
                {
                    currentTime = 0f;
                    isCountingDown = false;

                    TimerEnded();
                }
            }
        }

        protected virtual void TimerEnded()
        {
            Debug.Log("Timer has ended!");

        }
    }
}

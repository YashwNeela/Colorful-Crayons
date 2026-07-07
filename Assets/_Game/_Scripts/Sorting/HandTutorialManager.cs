using DG.Tweening;
using System.Collections;
using UnityEngine;

namespace TMKOC.Sorting
{
    public class HandTutorialManager : MonoBehaviour
    {
        [SerializeField] private Transform[] _items;
        [SerializeField, Tooltip("Only for dragging")] private Transform _endPosition;

        [SerializeField] private int tapCount = 2;          // Number of taps
        [SerializeField] private float tapDistance = 0.2f;  // Distance the hand moves (Y-axis)
        [SerializeField] private float tapDuration = 0.5f;  // Duration of one tap (up or down)
        [SerializeField] private float scaleFactor = 1.1f;  // How much to scale (1.1 = 10% larger)

        [Header("Hand Visual")]
        [SerializeField] private SpriteRenderer _handRenderer;

        [Header("Idle Animation")]
        [SerializeField] private float idleDelay = 5f; // seconds of no tap before idle plays
        [SerializeField] private float idleBobDistance = 0.1f;
        [SerializeField] private float idleBobDuration = 0.6f;

        [SerializeField, Tooltip("Reference position for the center of the screen")]
        private Transform _centerPosition;
        [SerializeField] private float moveToCenterDuration = 0.3f;

        private bool _isPlaying = false;

        private Tween _idleMoveTween;
        private Tween _idleScaleTween;
        private Tween _idleCenterTween;
        private bool _isIdlePlaying = false;
        private bool _canPlayIdel = false;

        // ---------------- Screen Tap Check ----------------
        private float _lastTapTime;

        private void OnEnable()
        {
            _handRenderer = GetComponent<SpriteRenderer>();
            // Start the countdown fresh whenever this becomes active
            _lastTapTime = Time.time;
            HideHand();
        }

        private void Update()
        {
            CheckForUserTap();
            CheckForIdleTrigger();
        }

        private void CheckForUserTap()
        {
#if UNITY_EDITOR || UNITY_STANDALONE
            if (Input.GetMouseButtonDown(0))
            {
                OnUserTapped();
            }
#else
            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            {
                OnUserTapped();
            }
#endif
        }

        private void OnUserTapped()
        {
            _lastTapTime = Time.time; // reset the 5-second countdown

            // If idle animation is currently playing, stop it immediately —
            // user is active again
            if (_isIdlePlaying)
            {
                StopIdleAnimation();
            }
        }

        private void CheckForIdleTrigger()
        {
            // Don't start idle while a tutorial tap/drag action is actively playing,
            // or if idle is already playing
            if (_isPlaying || _isIdlePlaying || !_canPlayIdel)
                return;

            if (Time.time - _lastTapTime >= idleDelay)
            {
                PlayIdleAnimation();
            }
        }

        // ---------------- Hand Visibility ----------------

        private void ShowHand()
        {
            if (_handRenderer != null)
                _handRenderer.enabled = true;
        }

        private void HideHand()
        {
            if (_handRenderer != null)
                _handRenderer.enabled = false;
        }

        // one animation for tapping
        private void TappingAction(Vector2 movePosition)
        {
            if (!_isPlaying)
            {
                StopIdleAnimation();
                ShowHand();
                _isPlaying = true;

                transform.DOScale(1f, 0.25f);
                // move to fruit position
                transform.DOMove(movePosition, 0.25f).SetDelay(0.35f).OnComplete(() =>
                {
                    //_handTutor.GetComponent<DOTweenAnimation>().DOPlay();
                    // play dotween animation for tapping
                    transform.DOLocalMoveY(transform.position.y + tapDistance, tapDuration)
                        .SetLoops(tapCount * 2, LoopType.Yoyo)  // Yoyo makes it go up and down
                        .SetEase(Ease.InOutSine)
                        .OnComplete(() =>
                        {
                            _isPlaying = false;
                        });

                    //Scaling effect
                    transform.DOScale(scaleFactor, tapDuration)
                             .SetLoops(tapCount * 2, LoopType.Yoyo)
                             .SetEase(Ease.InOutSine);
                });
            }
        }

        private void DraggingAction(Vector2 startPosition)
        {
            if (!_isPlaying)
            {
                StopIdleAnimation();
                ShowHand();
                _isPlaying = true; // Block further actions until this one completes

                // Set the initial position
                transform.position = startPosition;

                // Step 1: Scale down to 0 quickly
                transform.DOScale(1f, 0.25f).OnComplete(() =>
                {
                    // Step 2: Set the position back to the start and scale back up while moving to the end
                    transform.position = startPosition;
                    transform.DOScale(1f, 0.5f).OnStart(() =>
                    {
                        // Move to the end position while scaling up
                        transform.DOMove(_endPosition.position, 0.75f);
                    }).OnComplete(() =>
                    {
                        // Reset the flag to allow new actions after animation completes
                        //Scaling effect
                        transform.DOScale(scaleFactor, tapDuration)
                                 .SetLoops(3, LoopType.Yoyo)
                                 .SetEase(Ease.InOutSine);
                        _isPlaying = false;
                    });
                });
            }
        }

        private IEnumerator PlayTappingActionCoroutine()
        {
            WaitForSeconds waitTime = new(1.25f + tapDuration);

            for (int i = 0; i < _items.Length; i++)
            {
                // play one dotween at first item
                TappingAction(_items[i].transform.position);
                yield return waitTime;
            }

            transform.DOScale(0f, 0.25f)
                .OnComplete(() =>
                {
                    HideHand(); // not visible while waiting for idle countdown
                    _lastTapTime = Time.time; // start the 5-sec countdown from now
                    _canPlayIdel = true;
                });
        }

        private IEnumerator PlayDraggingActionCoroutine()
        {
            WaitForSeconds waitTime = new(1f * 2 + tapDuration);

            for (int i = 0; i < _items.Length; i++)
            {
                // play one dotween at first item
                DraggingAction(_items[i].transform.position);
                yield return waitTime;
            }

            transform.DOScale(0f, 0.25f)
                .OnComplete(
                () =>
                {
                    HideHand(); // not visible while waiting for idle countdown
                    _lastTapTime = Time.time; // start the 5-sec countdown from now
                    _canPlayIdel = true;
                });
        }

        // ---------------- Idle Animation ----------------

        private void PlayIdleAnimation()
        {
            _isIdlePlaying = true;
            ShowHand();

            // Make sure the hand is centered before starting the idle loop
            transform.localScale = Vector3.one;
            _idleCenterTween = transform.DOMove(_centerPosition.position, moveToCenterDuration)
                .SetEase(Ease.OutQuad)
                .OnComplete(() =>
                {
                    // Re-check in case state changed while moving to center
                    if (_isPlaying || Time.time - _lastTapTime < idleDelay)
                    {
                        _isIdlePlaying = false;
                        HideHand();
                        return;
                    }

                    // Gentle bobbing loop to draw attention back to the hand
                    _idleMoveTween = transform.DOLocalMoveY(transform.position.y + idleBobDistance, idleBobDuration)
                        .SetLoops(-1, LoopType.Yoyo)
                        .SetEase(Ease.InOutSine);

                    // Optional subtle scale pulse alongside the bob
                    _idleScaleTween = transform.DOScale(scaleFactor * 0.95f, idleBobDuration)
                             .SetLoops(-1, LoopType.Yoyo)
                             .SetEase(Ease.InOutSine);
                });
        }

        public void StopIdleAnimation()
        {
            if (_isIdlePlaying)
            {
                _idleCenterTween?.Kill();
                _idleMoveTween?.Kill();
                _idleScaleTween?.Kill();
                transform.DOScale(1f, 0.2f); // reset scale cleanly
                _isIdlePlaying = false;
                HideHand();
            }
        }

        // ---------------- Public Triggers ----------------

        public void PlayHandTutorial_Tapping()
        {
            StopIdleAnimation();
            _lastTapTime = Time.time;
            StartCoroutine(PlayTappingActionCoroutine());
        }

        public void PlayHandTutorial_Dragging()
        {
            StopIdleAnimation();
            _lastTapTime = Time.time;
            StartCoroutine(PlayDraggingActionCoroutine());
        }
    }
}
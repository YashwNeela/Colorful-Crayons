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

        private bool _isPlaying = false;

        // one animation for tapping
        private void TappingAction(Vector2 movePosition)
        {
            if (!_isPlaying)
            {
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
                _isPlaying = true; // Block further actions until this one completes

                // Set the initial position
                transform.position = startPosition;

                // Step 1: Scale down to 0 quickly
                transform.DOScale(0f, 0.25f).OnComplete(() =>
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

            transform.DOScale(0f, 0.25f);
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

            transform.DOScale(0f, 0.25f);
        }
        public void PlayHandTutorial_Tapping()
        {
            StartCoroutine(PlayTappingActionCoroutine());
        }

        public void PlayHandTutorial_Dragging()
        {
            StartCoroutine(PlayDraggingActionCoroutine());
        }
    }
}

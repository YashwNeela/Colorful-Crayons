using UnityEngine;
using System;

public class InputManager : MonoBehaviour
{
    [SerializeField] private float swipeThreshold = 50f;
    
    private Vector2 touchStartPos;
    private bool isSwiping = false;
    
    public event Action OnSwipeUp;
    public event Action OnSwipeDown;
    
    private void Update()
    {
        HandleInput();
    }
    
    private void HandleInput()
    {
        #if UNITY_EDITOR || UNITY_STANDALONE
        HandleMouseInput();
        #elif UNITY_ANDROID || UNITY_IOS
        HandleTouchInput();
        #endif
    }
    
    private void HandleMouseInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            touchStartPos = Input.mousePosition;
            isSwiping = true;
        }
        
        if (Input.GetMouseButtonUp(0) && isSwiping)
        {
            isSwiping = false;
            DetectSwipe(Input.mousePosition);
        }
    }
    
    private void HandleTouchInput()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            
            if (touch.phase == TouchPhase.Began)
            {
                touchStartPos = touch.position;
                isSwiping = true;
            }
            else if (touch.phase == TouchPhase.Ended && isSwiping)
            {
                isSwiping = false;
                DetectSwipe(touch.position);
            }
        }
    }
    
    private void DetectSwipe(Vector2 touchEndPos)
    {
        Vector2 swipeDirection = touchEndPos - touchStartPos;
        
        if (swipeDirection.magnitude < swipeThreshold)
            return;
        
        // Check if swipe is more vertical than horizontal
        if (Mathf.Abs(swipeDirection.y) > Mathf.Abs(swipeDirection.x))
        {
            if (swipeDirection.y > 0)
            {
                OnSwipeUp?.Invoke();
                Debug.Log("Swipe Up Detected");
            }
            else
            {
                OnSwipeDown?.Invoke();
                Debug.Log("Swipe Down Detected");
            }
        }
    }
}

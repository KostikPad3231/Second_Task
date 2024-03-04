using System;
using UnityEngine;

public class Swiping : MonoBehaviour
{
    public delegate void OnSwipeInput(Vector2 direction);
    public event OnSwipeInput SwipeEvent;

    private Vector2 startTouchPosition;
    private Vector2 swipeDelta;

    private float minSwipeDelta = 50;

    private bool isSwiping;
    private bool isMobile;

    private void Start()
    {
        isMobile = Application.isMobilePlatform;
    }

    private void Update()
    {
        if (!isMobile)
        {
            if (Input.GetMouseButtonDown(0))
            {
                isSwiping = true;
                startTouchPosition = Input.mousePosition;
            }
            else if(Input.GetMouseButtonUp(0))
            {
                ResetSwipe();
            }
        }
        else
        {
            if (Input.touchCount > 0)
            {
                if (Input.GetTouch(0).phase == TouchPhase.Began)
                {
                    isSwiping = true;
                    startTouchPosition = Input.GetTouch(0).position;
                }
                else if (Input.GetTouch(0).phase == TouchPhase.Canceled || Input.GetTouch(0).phase == TouchPhase.Ended)
                {
                    ResetSwipe();
                }
            }
        }
        
        CheckSwipe();
    }

    private void CheckSwipe()
    {
        swipeDelta = Vector2.zero;

        if (isSwiping)
        {
            if (!isMobile && Input.GetMouseButton(0))
            {
                swipeDelta = (Vector2)Input.mousePosition - startTouchPosition;
            }
            else if (Input.touchCount > 0)
            {
                swipeDelta = Input.GetTouch(0).position - startTouchPosition;
            }
        }

        if (swipeDelta.magnitude > minSwipeDelta)
        {
            if (Math.Abs(swipeDelta.x) > Math.Abs(swipeDelta.y))
            {
                SwipeEvent?.Invoke(swipeDelta.x > 0 ? Vector2.right : Vector2.left);
            }
            else
            {
                SwipeEvent?.Invoke(swipeDelta.y > 0 ? Vector2.up : Vector2.down);
            }
            
            ResetSwipe();
        }
    }

    private void ResetSwipe()
    {
        isSwiping = false;

        startTouchPosition = Vector2.zero;
        swipeDelta = Vector2.zero;
    }
}

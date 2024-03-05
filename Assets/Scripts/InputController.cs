using System;
using UnityEngine;

public class InputController : MonoBehaviour
{
    public delegate void OnMoveInput(Vector2 direction);

    public event OnMoveInput InputEvent;

    private Vector2 startTouchPosition;
    private Vector2 swipeDelta;

    private float minSwipeDelta = 50;

    private bool isSwiping;

    private void Update()
    {
#if UNITY_EDITOR || UNITY_STANDALONE
        if (Input.GetMouseButtonDown(0))
        {
            isSwiping = true;
            startTouchPosition = Input.mousePosition;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            ResetSwipe();
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.W))
            {
                InputEvent?.Invoke(Vector2.up);
            }

            if (Input.GetKeyDown(KeyCode.A))
            {
                InputEvent?.Invoke(Vector2.left);
            }

            if (Input.GetKeyDown(KeyCode.S))
            {
                InputEvent?.Invoke(Vector2.down);
            }

            if (Input.GetKeyDown(KeyCode.D))
            {
                InputEvent?.Invoke(Vector2.right);
            }
        }

#elif UNITY_ANDROID
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
#endif

        CheckSwipe();
    }

    private void CheckSwipe()
    {
        swipeDelta = Vector2.zero;

        if (isSwiping)
        {
#if UNITY_STANDALONE || UNITY_EDITOR
            if(Input.GetMouseButton(0))
            {
                swipeDelta = (Vector2)Input.mousePosition - startTouchPosition;
            }
#elif UNITY_ANDROID
            if (Input.touchCount > 0)
            {
                swipeDelta = Input.GetTouch(0).position - startTouchPosition;
            }
#endif
        }

        if (swipeDelta.magnitude > minSwipeDelta)
        {
            if (Math.Abs(swipeDelta.x) > Math.Abs(swipeDelta.y))
            {
                InputEvent?.Invoke(swipeDelta.x > 0 ? Vector2.right : Vector2.left);
            }
            else
            {
                InputEvent?.Invoke(swipeDelta.y > 0 ? Vector2.up : Vector2.down);
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
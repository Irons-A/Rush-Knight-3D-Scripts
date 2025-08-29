using System;
using UnityEngine;

namespace Scripts.Input
{
    public class PlayerInput : MonoBehaviour
    {
        [Header("Mobile Settings")]
        [SerializeField] private float _swipeThreshold = 50f;

        private Vector2 _touchStartPosition;

        public event Action OnLeft;
        public event Action OnRight;
        public event Action OnUp;
        public event Action OnDown;
        public event Action OnPausePressed;

        public bool AreControlsEnabled { get; private set; } = true;

        private void Update()
        {
            if (AreControlsEnabled == false)
            {
                return;
            }

            HandlePCInput();
            HandleMobileInput();
        }

        public void SetControlsEnabled(bool value)
        {
            AreControlsEnabled = value;
        }

        private void HandlePCInput()
        {
            if (UnityEngine.Input.GetKeyDown(KeyCode.A) || UnityEngine.Input.GetKeyDown(KeyCode.LeftArrow))
            {
                OnLeft?.Invoke();
            }

            if (UnityEngine.Input.GetKeyDown(KeyCode.D) || UnityEngine.Input.GetKeyDown(KeyCode.RightArrow))
            {
                OnRight?.Invoke();
            }

            if (UnityEngine.Input.GetKeyDown(KeyCode.W) || UnityEngine.Input.GetKeyDown(KeyCode.UpArrow))
            {
                OnUp?.Invoke();
            }

            if (UnityEngine.Input.GetKeyDown(KeyCode.S) || UnityEngine.Input.GetKeyDown(KeyCode.DownArrow))
            {
                OnDown?.Invoke();
            }

            if (UnityEngine.Input.GetKeyDown(KeyCode.Escape))
            {
                OnPausePressed?.Invoke();
            }
        }

        private void HandleMobileInput()
        {
            if (UnityEngine.Input.touchCount == 0)
            {
                return;
            }

            Touch touch = UnityEngine.Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                _touchStartPosition = touch.position;
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                Vector2 touchEndPosition = touch.position;
                ProcessSwipe(touchEndPosition);
            }
        }

        private void ProcessSwipe(Vector2 endPosition)
        {
            Vector2 swipeDelta = endPosition - _touchStartPosition;

            if (swipeDelta.magnitude < _swipeThreshold) return;

            bool isHorizontalSwipe = Mathf.Abs(swipeDelta.x) > Mathf.Abs(swipeDelta.y);

            if (isHorizontalSwipe)
            {
                if (swipeDelta.x > 0) OnRight?.Invoke();
                else OnLeft?.Invoke();
            }
            else
            {
                if (swipeDelta.y > 0) OnUp?.Invoke();
                else OnDown?.Invoke();
            }
        }
    }
}

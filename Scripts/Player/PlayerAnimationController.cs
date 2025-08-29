using UnityEngine;

namespace Scripts.Player
{
    public class PlayerAnimationController : MonoBehaviour
    {
        private Animator _animator;
        private bool _isJumping;
        private bool _gameoverAnimationPlayed = false;

        private int _runHash = Animator.StringToHash("Run");
        private int _jumpHash = Animator.StringToHash("Jump");
        private int _slideHash = Animator.StringToHash("Slide");
        private int _slipHash = Animator.StringToHash("Slip");
        private int _stumbleHash = Animator.StringToHash("Stumble");

        public void GetAnimatorInChildren()
        {
            _animator = GetComponentInChildren<Animator>();
        }

        public void SetRunAnimation()
        {
            _animator.SetTrigger(_runHash);
        }

        public void SetJumpAnimation()
        {
            _animator.SetTrigger(_jumpHash);
        }

        public void SetSlideAnimation()
        {
            _animator.SetTrigger(_slideHash);
        }

        public void SetJumpingState(bool isJumping)
        {
            _isJumping = isJumping;
        }

        public void SetGameoverAnimation()
        {
            if (_gameoverAnimationPlayed)
            {
                return;
            }

            if (_isJumping)
            {
                _animator.SetTrigger(_slipHash);
            }
            else
            {
                _animator.SetTrigger(_stumbleHash);
            }

            _gameoverAnimationPlayed = true;
        }

        public void ReenableGameoverAnimation()
        {
            _gameoverAnimationPlayed = false;
        }
    }
}

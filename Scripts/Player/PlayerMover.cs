using Scripts.Enums;
using Scripts.Input;
using Scripts.Systems.Audio;
using System.Collections;
using UnityEngine;

namespace Scripts.Player
{
    public class PlayerMover : MonoBehaviour
    {
        private readonly float[] _tracks = { -2f, 0f, 2f };

        [Header("Movement Settings")]
        [SerializeField] private float _baseMovementSpeed = 9f;
        [SerializeField] private float _strafeSpeed = 5f;
        [SerializeField] private float _jumpHeight = 5f;
        [SerializeField] private float _jumpDuration = 1f;
        [SerializeField] private float _slideEffectDuration = 1f;
        [SerializeField] private float _additionalSlideReactivationDelay = 0.5f;
        [SerializeField] private float _colliderShrinkMultiplyer = 0.5f;

        [Header("References")]
        [SerializeField] private PlayerInput _input;
        [SerializeField] private Rigidbody _rigidbody;
        [SerializeField] private CapsuleCollider _playerCollider;
        [SerializeField] private PlayerAnimationController _animationController;
        [SerializeField] private PlayerCollisionsHandler _collisionsHandler;
        [SerializeField] private ParticleSystem _playerDust;

        [Header("Pushback Settings")]
        [SerializeField] private float _pushDistance = 3f;
        [SerializeField] private float _pushDuration = 0.5f;
        [SerializeField]
        private AnimationCurve _decelerationCurve =
            new AnimationCurve(new Keyframe(0, 1), new Keyframe(1, 0));

        private int _currentTrack = 1;
        private int _desiredTrack = 1;
        private bool _isJumping = false;
        private bool _isSliding = false;
        private bool _canStrafe = true;
        private WaitForSeconds _jumpDelay;
        private WaitForSeconds _slideDelay;
        private WaitForSeconds _slideReactivationDelay;

        private float _gravityModifier = 2f;

        private float _originalColliderHeight;
        private float _originalColliderRadius;
        private Vector3 _originalColliderCenter;

        private ParticleSystem.EmissionModule _playerDustEmission;

        public bool CanMove { get; set; } = true;

        private void Awake()
        {
            _originalColliderHeight = _playerCollider.height;
            _originalColliderRadius = _playerCollider.radius;
            _originalColliderCenter = _playerCollider.center;

            _jumpDelay = new WaitForSeconds(_jumpDuration);
            _slideDelay = new WaitForSeconds(_slideEffectDuration);
            _slideReactivationDelay = new WaitForSeconds(_additionalSlideReactivationDelay);

            _playerDustEmission = _playerDust.emission;
        }

        private void OnEnable()
        {
            _input.OnLeft += MoveLeft;
            _input.OnRight += MoveRight;
            _input.OnUp += Jump;
            _input.OnDown += Slide;
        }

        private void OnDisable()
        {
            _input.OnLeft -= MoveLeft;
            _input.OnRight -= MoveRight;
            _input.OnUp -= Jump;
            _input.OnDown -= Slide;
        }

        private void Update()
        {
            if (CanMove && _isJumping == false && _isSliding == false)
            {
                if (AudioManager.Instance != null)
                {
                    AudioManager.Instance.SetShouldPlayFootsteps(true);
                }
            }
            else
            {
                if (AudioManager.Instance != null)
                {
                    AudioManager.Instance.SetShouldPlayFootsteps(false);
                }
            }

            _playerDustEmission.enabled = CanMove && _isJumping == false;

            if (Time.timeScale == 0 || CanMove == false) return;

            transform.Translate(Vector3.forward * _baseMovementSpeed * Time.deltaTime);

            if (_canStrafe)
            {
                Vector3 targetPosition = new Vector3(_tracks[_desiredTrack], transform.position.y, transform.position.z);
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, _strafeSpeed * Time.deltaTime);

                if (Mathf.Approximately(transform.position.x, _tracks[_desiredTrack]))
                {
                    _currentTrack = _desiredTrack;
                }
            }
        }

        public void ReenableRun()
        {
            _animationController.SetRunAnimation();
            SetCanMove(true);
        }

        public void SetCanMove(bool value)
        {
            CanMove = value;
        }

        public void ApplyPushback()
        {
            StartCoroutine(PushbackRoutine());
        }

        private IEnumerator PushbackRoutine()
        {
            Vector3 pushDirection = -transform.forward;
            float elapsedTime = 0f;

            float initialVelocity = _pushDistance / _pushDuration;
            Vector3 currentPushVelocity = pushDirection * initialVelocity;

            while (elapsedTime < _pushDuration)
            {
                float velocityMultiplier = _decelerationCurve.Evaluate(elapsedTime / _pushDuration);
                Vector3 modifiedVelocity = currentPushVelocity * velocityMultiplier;

                modifiedVelocity.y = _rigidbody.velocity.y;
                _rigidbody.velocity = modifiedVelocity;

                elapsedTime += Time.deltaTime;

                yield return null;
            }
        }

        private void MoveLeft()
        {
            if (Time.timeScale == 0 || CanMove == false) return;

            _desiredTrack = Mathf.Clamp(_desiredTrack - 1, 0, _tracks.Length - 1);
            if (_desiredTrack != _currentTrack) _canStrafe = true;
        }

        private void MoveRight()
        {
            if (Time.timeScale == 0 || CanMove == false) return;

            _desiredTrack = Mathf.Clamp(_desiredTrack + 1, 0, _tracks.Length - 1);
            if (_desiredTrack != _currentTrack) _canStrafe = true;
        }

        private void Jump()
        {
            if (Time.timeScale == 0 || _isJumping || CanMove == false) return;

            if (_isSliding)
            {
                StopCoroutine(PerformSlide());
                ResetCollider();
                _isSliding = false;
            }

            StartCoroutine(PerformJump());
        }

        private void Slide()
        {
            if (Time.timeScale == 0 || _isJumping || _isSliding || CanMove == false) return;

            StartCoroutine(PerformSlide());
        }

        private IEnumerator PerformJump()
        {
            _isJumping = true;
            _rigidbody.velocity = new Vector3(_rigidbody.velocity.x, CalculateJumpVelocity(), _rigidbody.velocity.z);

            _animationController.SetJumpAnimation();
            _animationController.SetJumpingState(true);

            AudioManager.Instance.PlaySound(SoundType.Jump, true);

            yield return _jumpDelay;

            _isJumping = false;
            _animationController.SetJumpingState(false);
        }

        private IEnumerator PerformSlide()
        {
            _isSliding = true;
            _collisionsHandler.SetSlideState(true);

            float newHeight = _originalColliderHeight * _colliderShrinkMultiplyer;
            float heightDifference = _originalColliderHeight - newHeight;

            _playerCollider.height = newHeight;
            _playerCollider.center = new Vector3(
                _originalColliderCenter.x,
                _originalColliderCenter.y - heightDifference * _colliderShrinkMultiplyer,
                _originalColliderCenter.z);

            _animationController.SetSlideAnimation();

            AudioManager.Instance.PlaySound(SoundType.Slide, true);

            yield return _slideDelay;

            ResetCollider();
            _collisionsHandler.SetSlideState(false);

            yield return _slideReactivationDelay;

            _isSliding = false;
        }

        private void ResetCollider()
        {
            _playerCollider.height = _originalColliderHeight;
            _playerCollider.radius = _originalColliderRadius;
            _playerCollider.center = _originalColliderCenter;
        }

        private float CalculateJumpVelocity()
        {
            return Mathf.Sqrt(_gravityModifier * Mathf.Abs(Physics.gravity.y) * _jumpHeight);
        }
    }
}

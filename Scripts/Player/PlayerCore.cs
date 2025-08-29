using Scripts.Input;
using Scripts.Systems;
using System;
using UnityEngine;

namespace Scripts.Player
{
    [RequireComponent(typeof(PlayerInput))]
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(CapsuleCollider))]
    [RequireComponent(typeof(PlayerAnimationController))]
    [RequireComponent(typeof(PlayerMover))]
    [RequireComponent(typeof(PlayerCollisionsHandler))]
    [RequireComponent(typeof(PlayerFlash))]
    [RequireComponent(typeof(PlayerMagnet))]
    public class PlayerCore : MonoBehaviour
    {
        [SerializeField] private GameObject[] _characters = new GameObject[5];

        private PlayerAnimationController _animationController;
        private PlayerMover _mover;
        private PlayerCollisionsHandler _collisionsHandler;
        private PlayerFlash _flasher;

        public event Action OnGameover;

        public bool IsRunning { get; private set; } = true;
        public bool IsReviveUsed { get; private set; } = false;

        private void Awake()
        {
            _mover = GetComponent<PlayerMover>();
            _collisionsHandler = GetComponent<PlayerCollisionsHandler>();
            _animationController = GetComponent<PlayerAnimationController>();
            _flasher = GetComponent<PlayerFlash>();

            DisplaySelectedCharacter();
        }

        private void OnEnable()
        {
            _collisionsHandler.GotHit += CommandGameover;
            _flasher.FlashEnded += SetInvulnurability;
        }

        private void OnDisable()
        {
            _collisionsHandler.GotHit -= CommandGameover;
            _flasher.FlashEnded -= SetInvulnurability;
        }

        public void CommandGameover()
        {
            _mover.SetCanMove(false);
            _mover.ApplyPushback();
            IsRunning = false;
            OnGameover?.Invoke();
        }

        public void Revive()
        {
            IsReviveUsed = true;
            _mover.ReenableRun();
            IsRunning = true;
            SetInvulnurability(true);
            _animationController.ReenableGameoverAnimation();

            _flasher.StartFlashEffect();
        }

        private void DisplaySelectedCharacter()
        {
            int selectedCharacterIndex = GlobalData.Instance.SelectedCharacter;

            foreach (GameObject character in _characters)
            {
                character.SetActive(false);
            }

            _characters[selectedCharacterIndex].SetActive(true);

            _animationController.GetAnimatorInChildren();
        }

        private void SetInvulnurability(bool state)
        {
            _collisionsHandler.SetInvulnurable(state);
        }
    }
}


using Scripts.Camera;
using Scripts.Enums;
using Scripts.Interctables;
using Scripts.Systems.Audio;
using System;
using UnityEngine;

namespace Scripts.Player
{
    [RequireComponent(typeof(PlayerAnimationController))]
    [RequireComponent(typeof(PlayerMagnet))]
    public class PlayerCollisionsHandler : MonoBehaviour
    {
        [SerializeField] private CameraShaker _cameraShaker;

        private PlayerAnimationController _animationController;
        private PlayerMagnet _magnet;

        private bool _isSlidingInActivePhase = false;
        private bool _isInvulnurable = false;

        public event Action GotHit;

        private void Awake()
        {
            _animationController = GetComponent<PlayerAnimationController>();
            _magnet = GetComponent<PlayerMagnet>();
        }

        private void OnTriggerEnter(Collider collider)
        {
            if (collider.TryGetComponent(out Obstacle _) && _isInvulnurable == false)
            {
                SetGameover();
            }
            else if (collider.TryGetComponent(out Enemy enemy))
            {
                if (_isSlidingInActivePhase)
                {
                    enemy.GetDefeated();
                    AudioManager.Instance.PlaySound(SoundType.EnemyKick, true);
                }
                else if (_isInvulnurable == false)
                {
                    enemy.Attack();
                    AudioManager.Instance.PlaySound(SoundType.EnemyHit, true);
                    SetGameover();
                }
            }
            else if (collider.TryGetComponent(out Gem gem))
            {
                gem.GetCollected();
                AudioManager.Instance.PlayCrystalCollectSound();
            }
            else if (collider.TryGetComponent(out MagnetPowerup magnet))
            {
                magnet.PlayPickupEffects();
                _magnet.ActivateMagnet();
                AudioManager.Instance.PlaySound(SoundType.Magnetpickup);
            }
        }

        public void SetSlideState(bool state)
        {
            _isSlidingInActivePhase = state;
        }

        private void SetGameover()
        {
            GotHit?.Invoke();
            AudioManager.Instance.PlaySound(SoundType.ObstacleHit);

            _animationController.SetGameoverAnimation();

            if (_cameraShaker != null)
            {
                _cameraShaker.TriggerShake();
            }
        }

        public void SetInvulnurable(bool state)
        {
            _isInvulnurable = state;
        }
    }
}

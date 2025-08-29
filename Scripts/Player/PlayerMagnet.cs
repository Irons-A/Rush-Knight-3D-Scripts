using Scripts.Interctables;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Scripts.Player
{
    public class PlayerMagnet : MonoBehaviour
    {
        [SerializeField] private float _duration = 12f;
        [SerializeField] private float _attractRadius = 15f;
        [SerializeField] private float _attractSpeed = 8f;
        [SerializeField] private float _gemVerticalOffset = 0.5f;
        [SerializeField] private float _magnetIconFlashDuration = 2f;
        [SerializeField] private float _magnetIconFlashInterval = 0.2f;
        [SerializeField] private Image _magnetIcon;

        private Coroutine _magnetCoroutine;
        private Coroutine _magnetFlashCoroutine;
        private List<Gem> _activeGems = new List<Gem>();

        private WaitForSeconds _timeBeforeFlashing;
        private WaitForSeconds _flashInterval;

        private void Awake()
        {
            _timeBeforeFlashing = new WaitForSeconds(_duration - _magnetIconFlashDuration);
            _flashInterval = new WaitForSeconds(_magnetIconFlashInterval);
        }

        public void ActivateMagnet()
        {
            if (_magnetCoroutine != null)
            {
                StopCoroutine(_magnetCoroutine);
            }

            _magnetCoroutine = StartCoroutine(MagnetRoutine());

            if (_magnetFlashCoroutine != null)
            {
                StopCoroutine(_magnetFlashCoroutine);
            }

            _magnetFlashCoroutine = StartCoroutine(MagnetFlashRoutine());

            _magnetIcon.enabled = true;
        }

        private IEnumerator MagnetRoutine()
        {
            float endTime = Time.time + _duration;

            while (Time.time < endTime)
            {
                FindNearbyGems();
                MoveGemsTowardsPlayer();

                yield return null;
            }

            _activeGems.Clear();
        }

        private IEnumerator MagnetFlashRoutine()
        {
            yield return _timeBeforeFlashing;

            float endTime = Time.time + _magnetIconFlashDuration;
            bool visibleState = true;

            while (Time.time < endTime)
            {
                _magnetIcon.enabled = visibleState;

                visibleState = !visibleState;

                yield return _flashInterval;
            }

            _magnetIcon.enabled = false;
        }

        private void FindNearbyGems()
        {
            Collider[] hitColliders = Physics.OverlapSphere(
                transform.position,
                _attractRadius,
                LayerMask.GetMask("Gems"));

            foreach (Collider collider in hitColliders)
            {
                Gem gem = collider.GetComponent<Gem>();

                if (gem != null && !_activeGems.Contains(gem))
                {
                    _activeGems.Add(gem);
                    gem.StartAttraction();
                }
            }
        }

        private void MoveGemsTowardsPlayer()
        {
            for (int i = _activeGems.Count - 1; i >= 0; i--)
            {
                Gem gem = _activeGems[i];
                if (gem == null)
                {
                    _activeGems.RemoveAt(i);
                    continue;
                }

                Vector3 direction = (transform.position - gem.transform.position).normalized;

                gem.transform.position += direction * _attractSpeed * Time.deltaTime;

                gem.transform.position += Vector3.up * _gemVerticalOffset * Time.deltaTime;
            }
        }
    }
}
using System;
using System.Collections;
using UnityEngine;

namespace Scripts.Player
{
    public class PlayerFlash : MonoBehaviour
    {
        [Header("Flash Settings")]
        [SerializeField] private float _flashDuration = 3f;
        [SerializeField] private float _flashInterval = 0.2f;

        private Renderer[] _currentModelRenderers;
        private Coroutine _flashRoutine;
        private WaitForSeconds _flashDelay;

        public event Action<bool> FlashEnded;

        private void Awake()
        {
            _flashDelay = new WaitForSeconds(_flashInterval);
        }

        public void StartFlashEffect()
        {
            if (_flashRoutine != null)
            {
                StopCoroutine(_flashRoutine);
            }

            _currentModelRenderers = GetActiveModelRenderers();

            _flashRoutine = StartCoroutine(FlashRoutine());
        }

        private IEnumerator FlashRoutine()
        {
            float endTime = Time.time + _flashDuration;
            bool visibleState = true;

            while (Time.time < endTime)
            {
                foreach (Renderer renderer in _currentModelRenderers)
                {
                    renderer.enabled = visibleState;
                }

                visibleState = !visibleState;

                yield return _flashDelay;
            }

            foreach (Renderer renderer in _currentModelRenderers)
            {
                renderer.enabled = true;
            }

            FlashEnded?.Invoke(false);
        }

        private Renderer[] GetActiveModelRenderers()
        {
            foreach (Transform child in transform)
            {
                if (child.gameObject.activeSelf)
                {
                    return child.GetComponentsInChildren<Renderer>(true);
                }
            }

            return new Renderer[0];
        }
    }
}

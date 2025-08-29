using UnityEngine;

namespace Scripts.Camera
{
    public class CameraShaker : MonoBehaviour
    {
        [SerializeField] private float _shakeDuration = 1f;
        [SerializeField] private float _initialMagnitude = 0.33f;

        private float _currentShakeDuration;
        private float _currentMagnitude;
        private Vector3 _shakeOffset;

        private void LateUpdate()
        {
            if (_currentShakeDuration > 0)
            {
                Vector2 randomPoint = Random.insideUnitCircle * _currentMagnitude;
                _shakeOffset = new Vector3(randomPoint.x, randomPoint.y, 0);

                transform.position += _shakeOffset;

                _currentShakeDuration -= Time.deltaTime;
                _currentMagnitude = Mathf.Lerp(0, _initialMagnitude, _currentShakeDuration / _shakeDuration);
            }
            else
            {
                _shakeOffset = Vector3.zero;
            }
        }

        public void TriggerShake()
        {
            if (_currentShakeDuration <= 0)
            {
                _currentShakeDuration = _shakeDuration;
                _currentMagnitude = _initialMagnitude;
            }
        }
    }
}
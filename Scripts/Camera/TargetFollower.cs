using UnityEngine;

namespace Scripts.Camera
{
    public class TargetFollower : MonoBehaviour
    {
        [SerializeField] private Transform _target;
        [SerializeField] private float _followSpeed = 5f;
        [SerializeField] private float _zOffset = 10f;
        [SerializeField] private float _yOffset = 0f;
        [SerializeField] private float lookAheadFactor = 0f;

        private void LateUpdate()
        {
            if (_target == null) return;

            Vector3 targetPosition = new Vector3(
                _target.position.x + (_target.forward.x * lookAheadFactor),
                _target.position.y + _yOffset,
                _target.position.z - _zOffset);

            Vector3 newPosition = Vector3.Lerp(transform.position, targetPosition, _followSpeed * Time.deltaTime);

            newPosition.z = _target.position.z - _zOffset;

            transform.position = newPosition;
        }
    }
}

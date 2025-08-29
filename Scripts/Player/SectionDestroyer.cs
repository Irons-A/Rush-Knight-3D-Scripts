using Scripts.Interctables;
using UnityEngine;

namespace Scripts.Player
{
    [RequireComponent(typeof(Rigidbody))]
    public class SectionDestroyer : MonoBehaviour
    {
        [SerializeField] private Transform _target;
        [SerializeField] private float _zOffset = -80;

        private void Update()
        {
            transform.position = new Vector3(_target.position.x, _target.position.y, _target.position.z + _zOffset);
        }

        private void OnTriggerEnter(Collider collider)
        {
            if (collider.TryGetComponent(out Gem gem))
            {
                gem.ReturnToPool();
            }
            else
            {
                Destroy(collider.gameObject);
            }
        }
    }
}

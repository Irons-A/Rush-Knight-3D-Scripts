using UnityEngine;

namespace Scripts.Interctables
{
    public class MagnetPowerup : MonoBehaviour
    {
        [SerializeField] private float _rotationSpeed = 90f;

        private void Update()
        {
            transform.Rotate(Vector3.up, _rotationSpeed * Time.deltaTime);
        }

        public void PlayPickupEffects()
        {
            Destroy(gameObject);
        }
    }
}

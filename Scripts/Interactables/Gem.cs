using Config.Gems;
using Scripts.Systems;
using UnityEngine;

namespace Scripts.Interctables
{
    [RequireComponent(typeof(MeshRenderer))]
    public class Gem : MonoBehaviour
    {
        [SerializeField] private MeshRenderer _renderer;

        [Header("Movement Settings")]
        [SerializeField] private float _rotationSpeed = 90f;
        [SerializeField] private float _bounceHeight = 0.2f;
        [SerializeField] private float _bounceSpeed = 1.5f;
        [SerializeField] private float _maxSpawnYOffset = 0.1f;

        private Vector3 _basePosition;
        private float _randomPhase;
        private bool _isBeingAttracted;

        [field: SerializeField] public GemType Type { get; set; }

        private void OnEnable()
        {
            _isBeingAttracted = false;
        }

        private void Update()
        {
            if (gameObject.activeSelf == false)
            {
                return;
            }

            if (_isBeingAttracted == false)
            {
                Rotate();
                Bounce();
            }
        }

        public void Initialize(GemType type)
        {
            Type = type;
            _renderer.material = type.material;

            Vector3 pos = transform.position;
            pos.y += Random.Range(-_maxSpawnYOffset, _maxSpawnYOffset);
            transform.position = pos;
            _basePosition = pos;

            _randomPhase = Random.Range(0f, 2f * Mathf.PI);
        }

        public void GetCollected()
        {
            ScoreCounter.Instance.AddScore(Type.scoreValue);
            ScoreCounter.Instance.EmphasizeScoreText();

            PlayCollectEffects();

            GemPool.Instance.ReturnToPool(this);
        }

        public void ReturnToPool()
        {
            GemPool.Instance.ReturnToPool(this);
        }

        public void StartAttraction()
        {
            _isBeingAttracted = true;
            _basePosition = transform.position;
        }

        private void Rotate()
        {
            transform.Rotate(Vector3.forward, _rotationSpeed * Time.deltaTime);
        }

        private void Bounce()
        {
            float newY = _basePosition.y + Mathf.Sin(Time.time * _bounceSpeed + _randomPhase) * _bounceHeight;
            transform.position = new Vector3(_basePosition.x, newY, _basePosition.z);
        }

        private void PlayCollectEffects()
        {
            if (Type.collectEffect != null)
            {
                ParticlePool.Instance.PlayParticleEffect(Type.collectEffect, transform.position);
            }
        }
    }
}

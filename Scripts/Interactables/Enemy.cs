using Scripts.Systems;
using UnityEngine;

namespace Scripts.Interctables
{
    [RequireComponent(typeof(Animator))]
    public class Enemy : MonoBehaviour
    {
        [SerializeField] private bool _isTutorialEnemy = false;
        [SerializeField] private ParticleSystem _kickEffect;
        [SerializeField] private int _score = 100;

        [SerializeField] private Animator _animator;

        private int _attackHash = Animator.StringToHash("attack");
        private int _dieHash = Animator.StringToHash("die");

        private void Awake()
        {
            if (_animator == null)
            {
                _animator = GetComponent<Animator>();
            }
        }

        public void Attack()
        {
            _animator.SetTrigger(_attackHash);
        }

        public void GetDefeated()
        {
            _animator.SetTrigger(_dieHash);

            if (_isTutorialEnemy == false)
            {
                ScoreCounter.Instance.AddScore(_score);
                ScoreCounter.Instance.EmphasizeScoreText();
            }

            if (_kickEffect != null)
            {
                ParticlePool.Instance.PlayParticleEffect(_kickEffect, transform.position);
            }
        }
    }
}

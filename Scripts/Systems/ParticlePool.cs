using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scripts.Systems
{
    public class ParticlePool : MonoBehaviour
    {
        public static ParticlePool Instance { get; private set; }

        [SerializeField] private List<ParticleEffect> _particleEffects = new List<ParticleEffect>();
        private Dictionary<ParticleSystem, Queue<ParticleSystem>> _poolDictionary =
            new Dictionary<ParticleSystem, Queue<ParticleSystem>>();

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;

            InitializePools();
        }

        private void InitializePools()
        {
            foreach (var effect in _particleEffects)
            {
                if (effect.Prefab == null) continue;

                var queue = new Queue<ParticleSystem>();

                for (int i = 0; i < effect.InitialPoolSize; i++)
                {
                    ParticleSystem ps = CreateParticleSystem(effect.Prefab);
                    queue.Enqueue(ps);
                }

                _poolDictionary[effect.Prefab] = queue;
            }
        }

        public void PlayParticleEffect(ParticleSystem prefab, Vector3 position)
        {
            if (prefab == null) return;

            if (!_poolDictionary.TryGetValue(prefab, out Queue<ParticleSystem> queue))
            {
                queue = new Queue<ParticleSystem>();
                _poolDictionary[prefab] = queue;
            }

            ParticleSystem ps = GetAvailableParticleSystem(queue, prefab);
            if (ps == null) return;

            SetupParticleSystem(ps, position);
            StartCoroutine(ReturnToPoolAfterPlay(ps, prefab, queue));
        }

        private ParticleSystem GetAvailableParticleSystem(Queue<ParticleSystem> queue, ParticleSystem prefab)
        {
            while (queue.Count > 0 && queue.Peek() == null)
            {
                queue.Dequeue();
            }

            if (queue.Count > 0) return queue.Dequeue();

            return CreateParticleSystem(prefab);
        }

        private void SetupParticleSystem(ParticleSystem ps, Vector3 position)
        {
            ps.transform.position = position;
            ps.gameObject.SetActive(true);
            ps.Play();
        }

        private IEnumerator ReturnToPoolAfterPlay(ParticleSystem ps, ParticleSystem prefab, Queue<ParticleSystem> queue)
        {
            while (ps != null && ps.isPlaying)
            {
                yield return null;
            }

            if (ps != null)
            {
                ps.gameObject.SetActive(false);
                queue.Enqueue(ps);
            }
        }

        private ParticleSystem CreateParticleSystem(ParticleSystem prefab)
        {
            ParticleSystem ps = Instantiate(prefab);

            ps.gameObject.SetActive(false);
            DontDestroyOnLoad(ps.gameObject);
            return ps;
        }

        [System.Serializable]
        public class ParticleEffect
        {
            public ParticleSystem Prefab;
            public int InitialPoolSize = 5;
        }
    }
}
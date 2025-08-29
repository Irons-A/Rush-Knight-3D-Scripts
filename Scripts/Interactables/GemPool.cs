using Config.Gems;
using System.Collections.Generic;
using UnityEngine;

namespace Scripts.Interctables
{
    public class GemPool : MonoBehaviour
    {
        public static GemPool Instance { get; private set; }

        [Header("Gem Settings")]
        [SerializeField] private List<GemTypePool> _gemPools = new List<GemTypePool>();

        private Dictionary<GemType, Queue<Gem>> _poolDictionary =
            new Dictionary<GemType, Queue<Gem>>();

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);

                return;
            }

            Instance = this;

            InitializePools();
        }

        private void InitializePools()
        {
            foreach (var gemPool in _gemPools)
            {
                if (gemPool.type == null || gemPool.prefab == null)
                {
                    continue;
                }

                Queue<Gem> objectPool = new Queue<Gem>();

                for (int i = 0; i < gemPool.initialPoolSize; i++)
                {
                    Gem gem = CreateGem(gemPool.prefab, gemPool.type);
                    gem.gameObject.SetActive(false);
                    objectPool.Enqueue(gem);
                }

                _poolDictionary[gemPool.type] = objectPool;
            }
        }

        public Gem GetGem(GemType type, Vector3 position)
        {
            if (!_poolDictionary.ContainsKey(type))
            {
                return null;
            }

            Gem gem = null;

            if (_poolDictionary[type].Count > 0)
            {
                gem = _poolDictionary[type].Dequeue();
            }
            else
            {
                gem = CreateNewGem(type);
            }

            if (gem != null)
            {
                gem.transform.position = position;
                gem.gameObject.SetActive(true);
                gem.Initialize(type);
            }

            return gem;
        }

        public void ReturnToPool(Gem gem)
        {
            if (gem == null) return;

            gem.gameObject.SetActive(false);

            if (_poolDictionary.ContainsKey(gem.Type))
            {
                _poolDictionary[gem.Type].Enqueue(gem);
            }
            else
            {
                Destroy(gem.gameObject);
            }
        }

        private Gem CreateNewGem(GemType type)
        {
            foreach (var pool in _gemPools)
            {
                if (pool.type == type)
                {
                    return CreateGem(pool.prefab, type);
                }
            }

            return null;
        }

        private Gem CreateGem(Gem prefab, GemType type)
        {
            Gem gem = Instantiate(prefab);
            gem.Initialize(type);

            return gem;
        }

        [System.Serializable]
        public class GemTypePool
        {
            public GemType type;
            public Gem prefab;
            public int initialPoolSize = 10;
        }
    }
}
using Config.Gems;
using Scripts.Interctables;
using System.Collections.Generic;
using UnityEngine;

namespace Scripts.Spawners
{
    public class GemSpawner : MonoBehaviour
    {
        [SerializeField]
        private List<GemSpawnChance> _gemSpawnChances = new List<GemSpawnChance>
    {
        new GemSpawnChance { GemType = null, SpawnChance = 70f },
        new GemSpawnChance { GemType = null, SpawnChance = 20f },
        new GemSpawnChance { GemType = null, SpawnChance = 10f },
    };

        [SerializeField] private float _spawnHeight = 1.5f;

        private void Start()
        {
            Vector3 spawnPosition = new Vector3(transform.position.x, _spawnHeight, transform.position.z);
            GemType gemToSpawn = GetRandomGemType();

            if (gemToSpawn != null)
            {
                GemPool.Instance.GetGem(gemToSpawn, spawnPosition);
            }
        }

        private GemType GetRandomGemType()
        {
            float totalChance = 0f;

            foreach (var chance in _gemSpawnChances)
            {
                totalChance += chance.SpawnChance;
            }

            float randomPoint = Random.Range(0f, totalChance);
            float cumulativeChance = 0f;

            foreach (var chance in _gemSpawnChances)
            {
                cumulativeChance += chance.SpawnChance;

                if (randomPoint <= cumulativeChance)
                {
                    return chance.GemType;
                }
            }

            return null;
        }

        [System.Serializable]
        public class GemSpawnChance
        {
            public GemType GemType;
            [Range(0, 100)] public float SpawnChance = 10f;
        }
    }
}

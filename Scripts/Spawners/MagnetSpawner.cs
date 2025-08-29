using Scripts.Interctables;
using Scripts.Levels;
using UnityEngine;

namespace Scripts.Spawners
{
    public class MagnetSpawner : MonoBehaviour
    {
        [SerializeField] private MagnetPowerup _magnetPrefab;
        [SerializeField] private float _spawnHeight = 1f;

        private int _spawnOnceInNumberOfSections;

        private void Start()
        {
            _spawnOnceInNumberOfSections = LevelGenerator.GetMagnetSpawnFrequency();

            int sectionsGenerated = LevelGenerator.GetSectionsGenerated();
            bool shouldSpawn = ShouldSpawnMagnet(sectionsGenerated);

            if (shouldSpawn)
            {
                Vector3 spawnPosition = new Vector3(transform.position.x, _spawnHeight, transform.position.z);
                Instantiate(_magnetPrefab, spawnPosition, Quaternion.identity);

                LevelGenerator.Instance.MarkMagnetSpawned();
            }
        }

        private bool ShouldSpawnMagnet(int sectionsGenerated)
        {
            bool isCorrectSection = (sectionsGenerated > 0) &&
                                  (sectionsGenerated % _spawnOnceInNumberOfSections == 0);

            bool isFirstInCycle = !LevelGenerator.Instance.MagnetSpawnedInCurrentCycle;

            return isCorrectSection && isFirstInCycle;
        }
    }
}

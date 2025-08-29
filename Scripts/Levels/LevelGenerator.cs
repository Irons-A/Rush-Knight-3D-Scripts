using Scripts.Player;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Scripts.Levels
{
    public class LevelGenerator : MonoBehaviour
    {
        private static int _sectionsGenerated = 0;
        private static int _spawnMagnetOnceInNumberOfSections = 6;

        [Header("Location Sections")]
        [SerializeField] private GameObject _location1Terrain;

        [Header("Action Sections")]
        [SerializeField] private List<GameObject> _location1ContentSections = new List<GameObject>();

        [Header("Generation Settings")]
        [SerializeField] private int _sectionsBeforeRepeat = 3;
        [SerializeField] private int _initialSections = 4;
        [SerializeField] private float _SectionSpawnDelay = 5.5f;
        [SerializeField] private int _generationStepOnZ = 60;

        [Header("References")]
        [SerializeField] private PlayerCore _player;

        [Header("Fog Settings")]
        [SerializeField] private bool _enableFog = true;
        [SerializeField] private float _fogStart = 60f;
        [SerializeField] private float _fogEnd = 90f;

        private Vector3 _nextSpawnPosition = new Vector3(0, 0, 60);
        private Queue<GameObject> _recentContentSections = new Queue<GameObject>();
        private bool _isGenerating = true;
        private int _flipSextionXScaleOnceIn = 2;
        private WaitForSeconds _currentSpawnDelay;
        private int _currentMagnetSpawnCycleSectionCount;

        public static LevelGenerator Instance { get; private set; }
        public bool MagnetSpawnedInCurrentCycle { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
            }
            else
            {
                Instance = this;
            }

            if (_player == null)
            {
                _player = FindObjectOfType<PlayerCore>();
            }

            _currentSpawnDelay = new WaitForSeconds(_SectionSpawnDelay);
        }

        private void Start()
        {
            for (int i = 0; i < _initialSections; i++)
            {
                GenerateFromLocation();
            }

            StartCoroutine(ContinuousGeneration());
            HandleFogSettings();
        }

        public static int GetSectionsGenerated() => _sectionsGenerated;

        public static int GetMagnetSpawnFrequency() => _spawnMagnetOnceInNumberOfSections;

        public void MarkMagnetSpawned()
        {
            MagnetSpawnedInCurrentCycle = true;
        }

        private void HandleFogSettings()
        {
            if (_enableFog)
            {
                RenderSettings.fog = true;
                RenderSettings.fogMode = FogMode.Linear;
                RenderSettings.fogStartDistance = _fogStart;
                RenderSettings.fogEndDistance = _fogEnd;
                RenderSettings.fogColor = Color.gray;
            }
        }

        private IEnumerator ContinuousGeneration()
        {
            while (_isGenerating)
            {
                while (!_player.IsRunning && enabled)
                {
                    yield return null;
                }

                yield return _currentSpawnDelay;

                GenerateFromLocation();
            }
        }

        private void GenerateFromLocation()
        {
            GameObject newTerrainSection = Instantiate(_location1Terrain, _nextSpawnPosition, Quaternion.identity);

            GameObject newContentSection = GetRandomContentSection(_location1ContentSections);
            Instantiate(newContentSection, _nextSpawnPosition, Quaternion.identity);

            if (_sectionsGenerated % _flipSextionXScaleOnceIn == 0)
            {
                Transform firstChild = newTerrainSection.transform.GetChild(0);

                Vector3 newScale = firstChild.localScale;
                newScale.x = -1;
                firstChild.localScale = newScale;
            }

            _nextSpawnPosition.z += _generationStepOnZ;

            _recentContentSections.Enqueue(newContentSection);

            if (_recentContentSections.Count > _sectionsBeforeRepeat)
            {
                _recentContentSections.Dequeue();
            }

            _sectionsGenerated++;

            _currentMagnetSpawnCycleSectionCount++;

            if (_currentMagnetSpawnCycleSectionCount >= _spawnMagnetOnceInNumberOfSections)
            {
                _currentMagnetSpawnCycleSectionCount = 0;
                MagnetSpawnedInCurrentCycle = false;
            }
        }

        private GameObject GetRandomContentSection(List<GameObject> sections)
        {
            List<GameObject> availableSections = sections
                .Where(s => !_recentContentSections.Contains(s))
                .ToList();

            if (availableSections.Count == 0)
            {
                availableSections = new List<GameObject>(sections);
            }

            return availableSections[Random.Range(0, availableSections.Count)];
        }
    }
}
using Scripts.Enums;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

namespace Scripts.Systems.Audio
{
    public class AudioManager : MonoBehaviour
    {
        public const string MusicVolume = "MusicVolume";
        public const string SoundVolume = "SFXVolume";

        [Header("Mixer")]
        [SerializeField] private AudioMixer _audioMixer;
        [SerializeField] private AudioMixerGroup _sfxGroup;
        [SerializeField] private AudioMixerGroup _musicGroup;
        [SerializeField] private AudioMixerSnapshot _normalMusicSnapshot;
        [SerializeField] private AudioMixerSnapshot _lowPitchMusicSnapshot;

        [Header("Music")]
        [SerializeField] private Music _menuMusicTrack;
        [SerializeField] private List<Music> _gameMusic;
        [SerializeField] private AudioSource _musicSource;
        [SerializeField] private float _musicSnapshotTransitionTime = 0.5f;
        [SerializeField] private bool _pauseWithTimescale = true;

        [Header("SFX")]
        [SerializeField] private List<Sound> _sounds = new List<Sound>();
        [SerializeField] private List<AudioClip> _stepSounds = new List<AudioClip>();
        [SerializeField] private float _footstepSoundFrequency = 0.3f;
        [SerializeField] private int _audioPoolSize = 10;
        [SerializeField] private float _lowerRandomPitch = 0.9f;
        [SerializeField] private float _upperRandomPitch = 1.1f;

        [Header("Crystal Collect")]
        [SerializeField] private float _crystalPitchIncrement = 0.02f;
        [SerializeField] private float _maxCrystalPitch = 1.4f;
        [SerializeField] private float _pitchResetDelay = 2f;
        [SerializeField] private float _defaultCrystalPitch = 1f;

        [Header("Scene Management")]
        [SerializeField]
        private List<string> _gameSceneNames = new List<string>() {
        SceneNames.Game.ToString(),
        SceneNames.Tutorial.ToString() };

        private string _currentSceneName;

        private List<AudioSource> _audioPool = new List<AudioSource>();
        private float _currentCrystalPitch = 1f;
        private int _volumeFormulaMultiplier = 20;
        private float _mutedVolumeValue = 0.001f;
        private float _maxVolumeValue = 1f;
        private Coroutine _pitchResetCoroutine;
        private Coroutine _footstepCoroutine;
        private WaitForSeconds _footstepSoundDelay;
        private bool _musicPausedByTimescale = false;
        private float _currentMusicPitch = 1f;
        private bool _shouldPlayFootsteps = false;

        public static AudioManager Instance { get; private set; }
        public List<Sound> Sounds => _sounds;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            InitializeAudioPool();
            LoadVolumeSettings();

            _footstepSoundDelay = new WaitForSeconds(_footstepSoundFrequency);
        }

        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void Update()
        {
            if (_pauseWithTimescale)
            {
                HandleMusicTimescalePause();
            }

            PlayFootsteps();
        }

        public void PlaySound(SoundType type, bool randomPitch = false, float customPitch = 1f, AudioClip clipOverride = null)
        {
            Sound sound = _sounds.Find(s => s.Type == type);

            if (sound == null) return;

            AudioSource source = GetAvailableSource();

            if (source == null) return;

            AudioClip clipToPlay = clipOverride ?? sound.Clip;

            source.volume = Mathf.Clamp(sound.BaseVolume * GlobalData.Instance.SoundVolume, _mutedVolumeValue, _maxVolumeValue);

            source.pitch = randomPitch ? Random.Range(_lowerRandomPitch, _upperRandomPitch) : customPitch;

            source.PlayOneShot(clipToPlay);
        }

        public void PlayCrystalCollectSound()
        {
            if (_pitchResetCoroutine != null)
            {
                StopCoroutine(_pitchResetCoroutine);
            }

            _currentCrystalPitch = Mathf.Min(_currentCrystalPitch + _crystalPitchIncrement, _maxCrystalPitch);

            PlaySound(SoundType.CrystalCollect, false, _currentCrystalPitch);

            _pitchResetCoroutine = StartCoroutine(ResetCrystalPitch());
        }

        public void PlayMenuMusic()
        {
            SetLowPitchMusic(false);
            StopGameMusic();
            _musicSource.clip = _menuMusicTrack.Clip;
            _musicSource.Play();
        }

        public void PlayGameMusic()
        {
            if (_musicSource.isPlaying && _gameMusic.Any(music => music.Clip == _musicSource.clip))
            {
                return;
            }

            StopMenuMusic();

            if (_gameMusic.Count > 0)
            {
                _musicSource.clip = _gameMusic[Random.Range(0, _gameMusic.Count)].Clip;
                _musicSource.Play();
            }
        }

        public void SetLowPitchMusic(bool enable)
        {
            if (enable)
            {
                _lowPitchMusicSnapshot.TransitionTo(_musicSnapshotTransitionTime);
            }
            else
            {
                _normalMusicSnapshot.TransitionTo(_musicSnapshotTransitionTime);
            }
        }

        public void SetSFXVolume(float volume)
        {
            float clampedVolume = Mathf.Clamp(volume, _mutedVolumeValue, _maxVolumeValue);
            float dB = Mathf.Log10(clampedVolume) * _volumeFormulaMultiplier;
            _audioMixer.SetFloat(SoundVolume, dB);
            GlobalData.Instance.SetSFXVolume(clampedVolume);
        }

        public void SetMusicVolume(float volume)
        {
            float clampedVolume = Mathf.Clamp(volume, _mutedVolumeValue, _maxVolumeValue);
            float dB = Mathf.Log10(clampedVolume) * _volumeFormulaMultiplier;
            _audioMixer.SetFloat(MusicVolume, dB);
            GlobalData.Instance.SetBGMVolume(clampedVolume);
        }

        public void SetShouldPlayFootsteps(bool value)
        {
            _shouldPlayFootsteps = value;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            _currentSceneName = scene.name;
            ResetFootsteps();
        }

        private void ResetFootsteps()
        {
            _shouldPlayFootsteps = false;

            if (_footstepCoroutine != null)
            {
                StopCoroutine(_footstepCoroutine);
                _footstepCoroutine = null;
            }
        }

        private void HandleMusicTimescalePause()
        {
            if (_musicSource == null) return;

            if (Mathf.Approximately(Time.timeScale, 0))
            {
                if (_musicSource.isPlaying)
                {
                    _musicSource.Pause();
                    _musicPausedByTimescale = true;
                }
            }
            else if (_musicPausedByTimescale)
            {
                _musicSource.UnPause();
                _musicPausedByTimescale = false;

                _musicSource.pitch = _currentMusicPitch;
            }
        }

        private void InitializeAudioPool()
        {
            for (int i = 0; i < _audioPoolSize; i++)
            {
                GameObject go = new GameObject($"SFX_Source_{i}");
                go.transform.SetParent(transform);
                AudioSource source = go.AddComponent<AudioSource>();
                source.outputAudioMixerGroup = _sfxGroup;
                source.playOnAwake = false;
                _audioPool.Add(source);
            }
        }

        private IEnumerator ResetCrystalPitch()
        {
            yield return new WaitForSeconds(_pitchResetDelay);

            _currentCrystalPitch = _defaultCrystalPitch;
        }

        private void PlayFootsteps()
        {
            if (_gameSceneNames.Contains(_currentSceneName) == false) return;

            if (_shouldPlayFootsteps && _footstepCoroutine == null)
            {
                _footstepCoroutine = StartCoroutine(FootstepRoutine());
            }
            else if (!_shouldPlayFootsteps && _footstepCoroutine != null)
            {
                StopCoroutine(_footstepCoroutine);
                _footstepCoroutine = null;
            }
        }

        private IEnumerator FootstepRoutine()
        {
            while (_shouldPlayFootsteps)
            {
                if (_stepSounds.Count > 0)
                {
                    AudioClip clip = _stepSounds[Random.Range(0, _stepSounds.Count)];
                    PlaySound(SoundType.Step, true, clipOverride: clip);
                }

                yield return _footstepSoundDelay;
            }

            _footstepCoroutine = null;
        }

        private void LoadVolumeSettings()
        {
            SetSFXVolume(GlobalData.Instance.SoundVolume);
            SetMusicVolume(GlobalData.Instance.MusicVolume);
        }

        private AudioSource GetAvailableSource()
        {
            foreach (AudioSource source in _audioPool)
            {
                if (!source.isPlaying)
                {
                    return source;
                }
            }

            return null;
        }

        private void StopMenuMusic() => _musicSource.Stop();
        private void StopGameMusic() => _musicSource.Stop();
    }
}
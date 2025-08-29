using Scripts.Systems.Audio;
using System.Collections;
using UnityEngine;
using UnityEngine.Localization.Settings;
using YG;

namespace YG
{
    public partial class SavesYG
    {
        public string GameData;
    }
}

namespace Scripts.Systems
{
    public class GlobalData : MonoBehaviour
    {
        public static GlobalData Instance { get; private set; }

        private GameData _currentData = new GameData();
        private bool _isLanguageSelectionProcessing = false;

        public int HighScore => _currentData.HighScore;
        public int SelectedCharacter => _currentData.SelectedCharacter;
        public bool[] CharactersUnlocked => _currentData.CharactersUnlocked;
        public int Char2Requirement => _currentData.Char2Requirement;
        public int Char3Progress => _currentData.Char3Progress;
        public int Char4Progress => _currentData.Char4Progress;
        public int Char5Requirement => _currentData.Char5Requirement;
        public float SoundVolume => _currentData.SoundVolume;
        public float MusicVolume => _currentData.MusicVolume;
        public bool TutorialCompleted => _currentData.TutorialCompleted;
        public int Char3Target => GameData.Char3Target;
        public int Char4Target => GameData.Char4Target;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);

                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            LoadData();
            HandleDailyProgress();
            TryUnlockingCharacters();
            SaveData();
        }

        private IEnumerator Start()
        {
            AudioManager.Instance.SetSFXVolume(SoundVolume);
            AudioManager.Instance.SetMusicVolume(MusicVolume);

            yield return LocalizationSettings.InitializationOperation;

            DefineAndSetLanguage();
        }

        public void TrySettingHighScore(int score)
        {
            if (score > _currentData.HighScore)
            {
                _currentData.SetHighScore(score);
                SaveData();
            }
        }

        public void SelectCharacter(int index)
        {
            if (index >= 0 && index < _currentData.CharactersUnlocked.Length
                && _currentData.CharactersUnlocked[index])
            {
                _currentData.SetSelectedCharacter(index);
                SaveData();
            }
        }

        public void UnlockCharacter(int index)
        {
            if (index >= 0 && index < _currentData.CharactersUnlocked.Length)
            {
                _currentData.SetCharacterUnlocked(index, true);
                SaveData();
            }
        }

        public void AddChar3Progress(int amount)
        {
            _currentData.SetChar3Progress(_currentData.Char3Progress + amount);
            SaveData();
        }

        public void SetSFXVolume(float volume)
        {
            _currentData.SetSoundVolume(volume);
            SaveData();
        }

        public void SetBGMVolume(float volume)
        {
            _currentData.SetMusicVolume(volume);
            SaveData();
        }

        public void SetLanguage(int localeID)
        {
            if (_isLanguageSelectionProcessing == false)
            {
                StartCoroutine(ChangeLanguageRoutine(localeID));
            }
        }

        public void CompleteTutorial()
        {
            _currentData.SetTutorialCompleted(true);
            SaveData();
        }

        public void TryUnlockingCharacters()
        {
            if (_currentData.HighScore >= _currentData.Char2Requirement && _currentData.CharactersUnlocked[1] == false)
            {
                UnlockCharacter(1);
                YG2.MetricaSend("Char2Unlocked");
            }
            else if (_currentData.Char3Progress >= _currentData.Char3Requirement
                && _currentData.CharactersUnlocked[2] == false)
            {
                UnlockCharacter(2);
                YG2.MetricaSend("Char3Unlocked");
            }
            else if (_currentData.Char4Progress >= _currentData.Char4Requirement
                && _currentData.CharactersUnlocked[3] == false)
            {
                UnlockCharacter(3);
                YG2.MetricaSend("Char4Unlocked");
            }
            else if (_currentData.HighScore >= _currentData.Char5Requirement && _currentData.CharactersUnlocked[4] == false)
            {
                UnlockCharacter(4);
                YG2.MetricaSend("Char5Unlocked");
            }
        }

        public void SaveData()
        {
            string json = JsonUtility.ToJson(_currentData);
            YG2.saves.GameData = json;

            YG2.SaveProgress();
        }

        private IEnumerator ChangeLanguageRoutine(int localeID)
        {
            _isLanguageSelectionProcessing = true;

            if (!LocalizationSettings.InitializationOperation.IsDone)
            {
                yield return LocalizationSettings.InitializationOperation;
            }

            LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[localeID];
            _isLanguageSelectionProcessing = false;
            SaveData();
        }

        private void LoadData()
        {
            if (YG2.saves.GameData == null || YG2.saves.GameData == string.Empty)
            {
                YG2.saves.GameData = JsonUtility.ToJson(new GameData());
            }

            string json = YG2.saves.GameData;
            _currentData = JsonUtility.FromJson<GameData>(json);

            InitializeDefaultData();
            SaveData();
        }

        private void InitializeDefaultData()
        {
            if (!_currentData.CharactersUnlocked[0])
            {
                _currentData.SetCharacterUnlocked(0, true);
                _currentData.SetSelectedCharacter(0);
            }
        }

        private void DefineAndSetLanguage()
        {
            string platformLanguage = YG2.envir.language;

            switch (platformLanguage)
            {
                case "ru":
                    SetLanguage(1);
                    break;
                case "tr":
                    SetLanguage(2);
                    break;
                default:
                    SetLanguage(0);
                    break;
            }
        }

        private void HandleDailyProgress()
        {
            long currentTimeMs = YG2.ServerTime();
            long lastPlayTimeMs = GetLastPlayTimeMs();

            long currentDays = currentTimeMs / (1000 * 60 * 60 * 24);
            long lastPlayDays = lastPlayTimeMs / (1000 * 60 * 60 * 24);

            if (currentDays > lastPlayDays)
            {
                _currentData.SetChar4Progress(_currentData.Char4Progress + 1);
                SaveData();
            }

            _currentData.SetLastPlayDate(currentTimeMs.ToString());
            SaveData();
        }

        private long GetLastPlayTimeMs()
        {
            if (string.IsNullOrEmpty(_currentData.LastPlayDate) ||
                    !long.TryParse(_currentData.LastPlayDate, out long lastTimeMs))
            {
                return 0;
            }

            return lastTimeMs;
        }
    }
}

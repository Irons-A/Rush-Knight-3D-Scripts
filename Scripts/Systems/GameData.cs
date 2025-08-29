using System;
using UnityEngine;
using YG;

namespace Scripts.Systems
{
    [Serializable]
    public class GameData
    {
        [SerializeField] private int _highScore = 0;
        [SerializeField] private int _selectedCharacter = 0;
        [SerializeField] private bool[] _charactersUnlocked = new bool[5];
        [SerializeField] private int _char3Progress = 0;
        [SerializeField] private int _char4Progress = 0;
        [SerializeField] private float _soundVolume = 0.8f;
        [SerializeField] private float _musicVolume = 0.8f;
        [SerializeField] private bool _tutorialCompleted = false;
        [SerializeField] private string _lastPlayDate;

        public const int Char2Target = 25000;
        public const int Char3Target = 15;
        public const int Char4Target = 3;
        public const int Char5Target = 50000;

        public int HighScore => _highScore;
        public int SelectedCharacter => _selectedCharacter;
        public bool[] CharactersUnlocked => (bool[])_charactersUnlocked.Clone();
        public int Char2Requirement => Char2Target;
        public int Char3Requirement => Char3Target;
        public int Char3Progress => _char3Progress;
        public int Char4Requirement => Char4Target;
        public int Char4Progress => _char4Progress;
        public int Char5Requirement => Char5Target;
        public float SoundVolume => _soundVolume;
        public float MusicVolume => _musicVolume;
        public bool TutorialCompleted => _tutorialCompleted;
        public string LastPlayDate => _lastPlayDate;

        public void SetHighScore(int newHighscore)
        {
            _highScore = newHighscore;
            YG2.SetLeaderboard("HighscoreLB", newHighscore);
        }

        public void SetSelectedCharacter(int index) => _selectedCharacter = index;
        public void SetCharacterUnlocked(int index, bool value)
        {
            if (index >= 0 && index < _charactersUnlocked.Length)
            {
                _charactersUnlocked[index] = value;
            }
        }

        public void SetChar3Progress(int value) => _char3Progress = Mathf.Clamp(value, 0, Char3Target);
        public void SetChar4Progress(int value) => _char4Progress = value;
        public void SetSoundVolume(float value) => _soundVolume = Mathf.Clamp01(value);
        public void SetMusicVolume(float value) => _musicVolume = Mathf.Clamp01(value);
        public void SetTutorialCompleted(bool value) => _tutorialCompleted = value;
        public void SetLastPlayDate(string date) => _lastPlayDate = date;
    }

}

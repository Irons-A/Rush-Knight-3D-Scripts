using System.Collections;
using DG.Tweening;
using Scripts.Enums;
using Scripts.Player;
using Scripts.Systems.Audio;
using Scripts.Systems;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;
using YG;

namespace Scripts.UI
{
    public class CharacterSelectionUI : MonoBehaviour
    {
        [SerializeField] private Button _previousButton;
        [SerializeField] private Button _nextButton;
        [SerializeField] private Button _selectButton;
        [SerializeField] private TextMeshProUGUI _characterNameText;
        [SerializeField] private TextMeshProUGUI _requirementText;
        [SerializeField] private TextMeshProUGUI _selectButtonText;
        [SerializeField] private Image _filmIcon;

        [Header("Localization")]
        [SerializeField] private LocalizedString _char1Name;
        [SerializeField] private LocalizedString _char2Name;
        [SerializeField] private LocalizedString _char3Name;
        [SerializeField] private LocalizedString _char4Name;
        [SerializeField] private LocalizedString _char5Name;
        [SerializeField] private LocalizedString _unlockedStart;
        [SerializeField] private LocalizedString _reachHighScore;
        [SerializeField] private LocalizedString _watchAdsFormat;
        [SerializeField] private LocalizedString _playDaysFormat;
        [SerializeField] private LocalizedString _selectedText;
        [SerializeField] private LocalizedString _selectText;
        [SerializeField] private LocalizedString _lockedText;
        [SerializeField] private LocalizedString _watchText;

        private int _currentCharacterIndex;
        private int _totalCharacters = 5;
        private MenuPlayerControl _menuPlayerControl;
        private GlobalData _globalData;
        private string _char3AdvRewardID = "char3progress";

        private void Awake()
        {
            _globalData = GlobalData.Instance;
            _menuPlayerControl = FindObjectOfType<MenuPlayerControl>();
        }

        private void OnEnable()
        {
            _previousButton.onClick.AddListener(() => CycleCharacter(-1));
            _nextButton.onClick.AddListener(() => CycleCharacter(1));
            _selectButton.onClick.AddListener(HandleCharacterSelection);
            _globalData.TryUnlockingCharacters();

            LocalizationSettings.SelectedLocaleChanged += OnLocaleChanged;
            UpdateUI();
        }

        private void OnDisable()
        {
            _previousButton.onClick.RemoveListener(() => CycleCharacter(-1));
            _nextButton.onClick.RemoveListener(() => CycleCharacter(1));
            _selectButton.onClick.RemoveListener(HandleCharacterSelection);

            LocalizationSettings.SelectedLocaleChanged -= OnLocaleChanged;
        }

        private void Start()
        {
            _currentCharacterIndex = _globalData.SelectedCharacter;

            _menuPlayerControl.SetPreviewIndex(_currentCharacterIndex);
            UpdateUI();
        }

        private void OnLocaleChanged(Locale locale)
        {
            StartCoroutine(UpdateLocalizedUI());
        }

        private void CycleCharacter(int direction)
        {
            _currentCharacterIndex = (_currentCharacterIndex + direction + _totalCharacters) % _totalCharacters;
            _menuPlayerControl.CycleCharacter(direction);
            UpdateUI();
            AudioManager.Instance.PlaySound(SoundType.UIclick);
        }

        private void UpdateUI()
        {
            if (_currentCharacterIndex == 2 && GlobalData.Instance.CharactersUnlocked[2] == false)
            {
                _filmIcon.enabled = true;
                _selectButtonText.enabled = false;
            }
            else
            {
                _filmIcon.enabled = false;
                _selectButtonText.enabled = true;
            }

            _globalData.TryUnlockingCharacters();
            StartCoroutine(UpdateLocalizedUI());
        }

        private IEnumerator UpdateLocalizedUI()
        {
            yield return LocalizationSettings.InitializationOperation;

            _characterNameText.text = string.Empty;
            _requirementText.text = string.Empty;
            _selectButtonText.text = string.Empty;

            var nameOp = GetLocalizedCharacterNames();
            yield return nameOp;
            _characterNameText.text = nameOp.Result;

            var requirementOp = GetLocalizedRequirementText();
            yield return requirementOp;
            _requirementText.text = requirementOp.Result;

            var buttonOp = GetLocalizedButtonText();
            yield return buttonOp;
            _selectButtonText.text = buttonOp.Result;
        }

        private AsyncOperationHandle<string> GetLocalizedCharacterNames()
        {
            return _currentCharacterIndex switch
            {
                0 => _char1Name.GetLocalizedStringAsync(),
                1 => _char2Name.GetLocalizedStringAsync(),
                2 => _char3Name.GetLocalizedStringAsync(),
                3 => _char4Name.GetLocalizedStringAsync(),
                4 => _char5Name.GetLocalizedStringAsync(),
                _ => new AsyncOperationHandle<string>()
            };
        }

        private AsyncOperationHandle<string> GetLocalizedRequirementText()
        {
            return _currentCharacterIndex switch
            {
                0 => _unlockedStart.GetLocalizedStringAsync(),
                1 => _reachHighScore.GetLocalizedStringAsync(_globalData.Char2Requirement),
                2 => _watchAdsFormat.GetLocalizedStringAsync(_globalData.Char3Target, _globalData.Char3Progress),
                3 => _playDaysFormat.GetLocalizedStringAsync(_globalData.Char4Target, _globalData.Char4Progress),
                4 => _reachHighScore.GetLocalizedStringAsync(_globalData.Char5Requirement),
                _ => new AsyncOperationHandle<string>()
            };
        }

        private AsyncOperationHandle<string> GetLocalizedButtonText()
        {
            if (_globalData.CharactersUnlocked[_currentCharacterIndex])
            {
                return _globalData.SelectedCharacter == _currentCharacterIndex ?
                    _selectedText.GetLocalizedStringAsync() :
                    _selectText.GetLocalizedStringAsync();
            }

            return _currentCharacterIndex switch
            {
                1 or 3 or 4 => _lockedText.GetLocalizedStringAsync(),
                2 => _watchText.GetLocalizedStringAsync(),
                _ => new AsyncOperationHandle<string>()
            };
        }

        private void HandleCharacterSelection()
        {
            if (_globalData.CharactersUnlocked[_currentCharacterIndex])
            {
                if (_globalData.SelectedCharacter != _currentCharacterIndex)
                {
                    _globalData.SelectCharacter(_currentCharacterIndex);
                    _selectButton.transform.DOPunchScale(Vector3.one * 0.1f, 0.2f);
                    AudioManager.Instance.PlaySound(SoundType.CharacterEquip);
                }
            }
            else if (_currentCharacterIndex == 2 && GlobalData.Instance.CharactersUnlocked[2] == false)
            {
                AudioManager.Instance.PlaySound(SoundType.UIclick);

                string rewardID = _char3AdvRewardID;

                YG2.RewardedAdvShow(rewardID, () =>
                {
                    if (rewardID == _char3AdvRewardID)
                    {
                        _globalData.AddChar3Progress(1);
                        UpdateUI();
                    }
                });
            }
            else
            {
                AudioManager.Instance.PlaySound(SoundType.UIclick);
            }

            UpdateUI();
        }
    }
}
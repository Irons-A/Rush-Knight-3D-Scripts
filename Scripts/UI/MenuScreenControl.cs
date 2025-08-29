using Scripts.Systems.Audio;
using Scripts.Systems;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.ResourceManagement.AsyncOperations;
using YG;

namespace Scripts.UI
{
    public class MenuScreenControl : MonoBehaviour
    {
        [SerializeField] private GameObject _menuCanvas;
        [SerializeField] private GameObject _charactersCanvas;
        [SerializeField] private GameObject _settingsCanvas;
        [SerializeField] private GameObject _leaderboardCanvas;
        [SerializeField] private GameObject _creditsCanvas;

        [SerializeField] private LocalizedString _highsoreLocalizedString;
        [SerializeField] private TextMeshProUGUI _highscoreText;

        private void Start()
        {
            AudioManager.Instance.PlayMenuMusic();
            StartCoroutine(InitializeLocalization());
            YG2.GameReadyAPI();
        }

        public void ShowMenuCanvas() => ToggleCanvas(_menuCanvas);
        public void ShowCharactersCanvas() => ToggleCanvas(_charactersCanvas);
        public void ShowSettingsCanvas() => ToggleCanvas(_settingsCanvas);
        public void ShowLeaderboardCanvas() => ToggleCanvas(_leaderboardCanvas);
        public void ShowCreditsCanvas() => ToggleCanvas(_creditsCanvas);

        private void ToggleCanvas(GameObject targetCanvas)
        {
            _menuCanvas.SetActive(false);
            _charactersCanvas.SetActive(false);
            _settingsCanvas.SetActive(false);
            _leaderboardCanvas.SetActive(false);
            _creditsCanvas.SetActive(false);
            targetCanvas.SetActive(true);

            UpdateLocalization();
        }

        private IEnumerator InitializeLocalization()
        {
            while (!YG2.isSDKEnabled || !LocalizationSettings.InitializationOperation.IsDone)
            {
                yield return null;
            }

            yield return UpdateLocalizedUI();

            LocalizationSettings.SelectedLocaleChanged += OnLocaleChanged;
        }

        private void OnLocaleChanged(Locale locale)
        {
            UpdateLocalization();
        }

        private void UpdateLocalization()
        {
            StartCoroutine(UpdateLocalizedUI());
        }

        private IEnumerator UpdateLocalizedUI()
        {
            var operation = _highsoreLocalizedString.GetLocalizedStringAsync(GlobalData.Instance.HighScore);

            yield return operation;

            if (operation.IsDone && operation.Status == AsyncOperationStatus.Succeeded)
            {
                _highscoreText.text = operation.Result;
            }
        }

        private void OnDestroy()
        {
            LocalizationSettings.SelectedLocaleChanged -= OnLocaleChanged;
        }
    }
}
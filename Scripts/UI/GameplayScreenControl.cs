using Scripts.Enums;
using Scripts.Input;
using Scripts.Systems.Audio;
using Scripts.Systems;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using YG;
using Scripts.Player;

namespace Scripts.UI
{
    public class GameplayScreenControl : MonoBehaviour
    {
        [Header("UI Canvases")]
        [SerializeField] private GameObject _gameplayUICanvas;
        [SerializeField] private GameObject _pauseCanvas;
        [SerializeField] private GameObject _gameoverCanvas;
        [SerializeField] private GameObject _resultCanvas;

        [Header("Countdown Settings")]
        [SerializeField] private TMP_Text _countdownText;
        [SerializeField] private float _countdownInterval = 0.5f;
        [SerializeField] private float _continueAfter = 2f;
        [SerializeField] private float _setGameoverScreenAfter = 2f;
        [SerializeField] private LocalizedString _countdownGo;

        [Header("References")]
        [SerializeField] private PlayerCore _player;
        [SerializeField] private PlayerInput _playerInput;

        [Header("Tutorial Settings")]
        [SerializeField] private bool _isTutorial = false;

        private bool _isPaused;
        private bool _isCountdownActive = false;
        private bool _canPause = true;
        private string[] _countdownSequence = new string[] { "3", "2", "1", "0" };
        private string _reviveAdvRewardID = "revive";
        private string _pendingSceneToLoad;
        private bool _isAdInProgress;

        private WaitForSeconds _countdownTotalDelay;
        private WaitForSeconds _gameoverScreenDelay;
        private WaitForSecondsRealtime _tutorialRestartDelay;
        private WaitForSecondsRealtime _countdownStepDelay;

        private void Awake()
        {
            _countdownTotalDelay = new WaitForSeconds(_continueAfter);
            _gameoverScreenDelay = new WaitForSeconds(_setGameoverScreenAfter);
            _countdownStepDelay = new WaitForSecondsRealtime(_countdownInterval);
            _tutorialRestartDelay = new WaitForSecondsRealtime(LoadingScreen.Instance.DefaultFadeDuration);
        }

        private void OnEnable()
        {
            _playerInput.OnPausePressed += PauseWithKeyboard;
            _player.OnGameover += SetGameover;
            YG2.onCloseInterAdv += OnInterstitialClosed;
            YG2.onErrorInterAdv += OnInterstitialClosed;
            YG2.onCloseInterAdvWasShow += HandleInterAdvError;
        }

        private void OnDisable()
        {
            _playerInput.OnPausePressed -= PauseWithKeyboard;
            _player.OnGameover -= SetGameover;
            YG2.onCloseInterAdv -= OnInterstitialClosed;
            YG2.onErrorInterAdv -= OnInterstitialClosed;
            YG2.onCloseInterAdvWasShow -= HandleInterAdvError;
        }

        private void Start()
        {
            StartCoroutine(PreloadLocalization());

            RestoreTimescale();

            if (_isTutorial == false)
            {
                SetAllCanvases(false);
                _gameplayUICanvas.SetActive(true);
                StartCountdown();
            }

            AudioManager.Instance.PlayGameMusic();
            AudioManager.Instance.SetLowPitchMusic(false);
        }

        public void OnPauseButtonClicked()
        {
            if (_canPause && _isCountdownActive == false)
            {
                AttemptToPuaseGame();
            }
        }

        public void OnContinueClicked()
        {
            if (_isCountdownActive == false)
            {
                StartCoroutine(ContinueGameRoutine());
            }
        }

        public void OnRestartClicked()
        {
            if (_isTutorial == false)
            {
                ScoreCounter.Instance.TrySettingHighscore();
                ShowAdAndQueueScene(SceneNames.Game.ToString());
            }
            else
            {
                LoadingScreen.Instance.LoadScene(SceneNames.Tutorial.ToString(), true);
            }
        }

        public void OnExitClicked()
        {
            if (_isTutorial == false)
            {
                ScoreCounter.Instance.TrySettingHighscore();
                ShowAdAndQueueScene(SceneNames.MainMenu.ToString());
            }
            else
            {
                LoadingScreen.Instance.LoadScene(SceneNames.MainMenu.ToString());
            }
        }

        public void SetGameover()
        {
            if (_isTutorial == false)
            {
                StartCoroutine(GameoverScreenRoutine());
            }
            else
            {
                StartCoroutine(RestartTutorialRoutine());
            }
        }

        public void OnReviveCancelClicked()
        {
            _gameoverCanvas.SetActive(false);
            _resultCanvas.SetActive(true);
            ScoreCounter.Instance.UpdateEndScoreDisplay();
        }

        public void OnReviveClicked()
        {
            if (_player.IsReviveUsed == false)
            {
                string rewardID = _reviveAdvRewardID;

                YG2.RewardedAdvShow(rewardID, () =>
                {
                    if (rewardID == _reviveAdvRewardID)
                    {
                        StartCoroutine(ReviveRoutine());
                    }
                });
            }
        }

        public void OnResultOKClicked()
        {
            ShowAdAndQueueScene(SceneNames.MainMenu.ToString());
        }

        public void GoToGame()
        {
            if (_isTutorial)
            {
                GlobalData.Instance.CompleteTutorial();

                LoadingScreen.Instance.LoadScene(SceneNames.Game.ToString());
            }
        }

        private void AttemptToPuaseGame()
        {
            if (_isTutorial == false)
            {
                if (!_gameoverCanvas.activeSelf && !_resultCanvas.activeSelf)
                {
                    SetPauseState(true);
                    _canPause = false;
                }
            }
            else
            {
                SetPauseState(true);
                _canPause = false;
            }
        }

        private void HandleInterAdvError(bool value)
        {
            if (value == false)
            {
                OnInterstitialClosed();
            }
        }

        private void ShowAdAndQueueScene(string sceneName)
        {
            if (_isAdInProgress == false)
            {
                _isAdInProgress = true;
                _pendingSceneToLoad = sceneName;
                YG2.InterstitialAdvShow();
            }
        }

        private void OnInterstitialClosed()
        {
            if (string.IsNullOrEmpty(_pendingSceneToLoad) == false)
            {
                _isAdInProgress = false;
                LoadingScreen.Instance.LoadScene(_pendingSceneToLoad, true);
                _pendingSceneToLoad = null;
            }
        }

        private IEnumerator PreloadLocalization()
        {
            var operation = _countdownGo.GetLocalizedStringAsync();
            yield return operation;
            _countdownSequence[_countdownSequence.Length - 1] = operation.Result;
        }

        private void StartCountdown()
        {
            StartCoroutine(CountdownRoutine());
        }

        private void PauseWithKeyboard()
        {
            if (_isPaused)
            {
                OnContinueClicked();
            }
            else
            {
                OnPauseButtonClicked();
            }
        }

        private IEnumerator GameoverScreenRoutine()
        {
            AudioManager.Instance.SetLowPitchMusic(true);

            yield return _gameoverScreenDelay;

            if (_player.IsReviveUsed)
            {
                _resultCanvas.SetActive(true);
                ScoreCounter.Instance.UpdateEndScoreDisplay();
            }
            else
            {
                _gameoverCanvas.SetActive(true);
            }

            _gameplayUICanvas.SetActive(false);
        }

        private IEnumerator ReviveRoutine()
        {
            _gameoverCanvas.SetActive(false);
            _gameplayUICanvas.SetActive(true);

            StartCountdown();

            Time.timeScale = 1f;
            _isPaused = false;
            AudioManager.Instance.SetLowPitchMusic(false);

            yield return _countdownTotalDelay;

            _player.Revive();
        }

        private void SetAllCanvases(bool state)
        {
            _gameplayUICanvas.SetActive(state);
            _pauseCanvas.SetActive(state);
            _gameoverCanvas.SetActive(state);
            _resultCanvas.SetActive(state);
        }

        private IEnumerator CountdownRoutine()
        {
            _countdownText.gameObject.SetActive(true);

            _isCountdownActive = true;

            foreach (string text in _countdownSequence)
            {
                _countdownText.text = text;
                AudioManager.Instance.PlaySound(SoundType.Countdown);

                yield return _countdownStepDelay;
            }

            _countdownText.gameObject.SetActive(false);
            _canPause = true;
            _isCountdownActive = false;
        }

        private IEnumerator ContinueGameRoutine()
        {
            _pauseCanvas.SetActive(false);
            StartCountdown();

            yield return new WaitForSecondsRealtime(_setGameoverScreenAfter);

            RestoreTimescale();
        }

        private IEnumerator RestartTutorialRoutine()
        {
            yield return _tutorialRestartDelay;

            LoadingScreen.Instance.LoadScene(SceneNames.Tutorial.ToString(), true);
        }

        private void RestoreTimescale()
        {
            Time.timeScale = 1f;
            _isPaused = false;
        }

        private void SetPauseState(bool paused)
        {
            _isPaused = paused;
            _pauseCanvas.SetActive(paused);
            Time.timeScale = paused ? 0f : 1f;
        }
    }
}
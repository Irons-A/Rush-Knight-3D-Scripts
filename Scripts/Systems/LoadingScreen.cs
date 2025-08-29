using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Scripts.Systems
{
    public class LoadingScreen : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private Canvas _rootCanvas;
        [SerializeField] private CanvasGroup _loadingCanvas;
        [SerializeField] private Slider _progressSlider;
        [SerializeField] private Image _blackScreen;

        [Header("Settings")]
        [SerializeField] private float _minimumLoadTime = 1f;
        [SerializeField] private bool _immediateFadeOnFirstLoad = true;

        private AsyncOperation _loadingOperation;
        private bool _isFirstLoad = true;
        private Tween _currentFadeTween;

        [field: SerializeField] public float DefaultFadeDuration { get; private set; } = 0.5f;
        public static LoadingScreen Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);

                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            InitializeUI();
        }

        private void InitializeUI()
        {
            _rootCanvas.enabled = true;

            _loadingCanvas.alpha = 0;
            _blackScreen.color = new Color(0, 0, 0, 0);
            _blackScreen.gameObject.SetActive(false);
        }

        public Tween FadeIn(float duration = -1, bool quickTransition = false)
        {
            return Fade(1f, duration, quickTransition);
        }

        public Tween FadeOut(float duration = -1, bool quickTransition = false)
        {
            return Fade(0f, duration, quickTransition);
        }

        private Tween Fade(float targetAlpha, float duration = -1, bool quickTransition = false)
        {
            duration = duration < 0 ? DefaultFadeDuration : duration;
            _currentFadeTween?.Kill();

            if (quickTransition)
            {
                _blackScreen.gameObject.SetActive(true);
                _loadingCanvas.gameObject.SetActive(false);

                return _currentFadeTween = _blackScreen.DOFade(targetAlpha, duration)
                    .SetUpdate(UpdateType.Normal, true)
                    .OnStart(() =>
                    {
                        _blackScreen.color = new Color(0, 0, 0, 1 - targetAlpha);
                    })
                    .OnComplete(() =>
                    {
                        if (targetAlpha <= 0) _blackScreen.gameObject.SetActive(false);
                    });
            }
            else
            {
                _loadingCanvas.gameObject.SetActive(true);
                _blackScreen.gameObject.SetActive(false);

                return _currentFadeTween = _loadingCanvas.DOFade(targetAlpha, duration)
                    .SetUpdate(UpdateType.Normal, true)
                    .OnStart(() =>
                    {
                        _loadingCanvas.alpha = 1 - targetAlpha;
                    })
                    .OnComplete(() =>
                    {
                        if (targetAlpha <= 0) _loadingCanvas.gameObject.SetActive(false);
                    });
            }
        }

        public void LoadScene(string sceneName, bool quickTransition = false)
        {
            StartCoroutine(LoadSceneCoroutine(sceneName, quickTransition));
        }

        private IEnumerator LoadSceneCoroutine(string sceneName, bool quickTransition)
        {
            if (_isFirstLoad && _immediateFadeOnFirstLoad)
            {
                if (quickTransition)
                {
                    _blackScreen.color = new Color(0, 0, 0, 1);
                    _blackScreen.gameObject.SetActive(true);
                }
                else
                {
                    _loadingCanvas.alpha = 1;
                    _loadingCanvas.gameObject.SetActive(true);
                }

                _isFirstLoad = false;
            }
            else
            {
                var fadeIn = FadeIn(DefaultFadeDuration, quickTransition);

                yield return fadeIn.WaitForCompletion(true);
            }

            float loadStartTime = Time.realtimeSinceStartup;
            _loadingOperation = SceneManager.LoadSceneAsync(sceneName);
            _loadingOperation.allowSceneActivation = false;

            while (!_loadingOperation.isDone)
            {
                if (!quickTransition)
                {
                    float progress = Mathf.Clamp01(_loadingOperation.progress / 0.9f);
                    _progressSlider.value = progress;
                }

                if (Time.realtimeSinceStartup - loadStartTime >= (quickTransition ? 0 : _minimumLoadTime))
                {
                    _loadingOperation.allowSceneActivation = true;
                }

                yield return null;
            }

            if (!quickTransition)
            {
                float elapsedTime = Time.realtimeSinceStartup - loadStartTime;
                float remainingTime = _minimumLoadTime - elapsedTime;

                if (remainingTime > 0)
                {
                    yield return new WaitForSecondsRealtime(remainingTime);
                }
            }

            Time.timeScale = 1f;
            var fadeOut = FadeOut(DefaultFadeDuration, quickTransition);

            yield return fadeOut.WaitForCompletion(true);
        }

        private void OnDestroy()
        {
            _currentFadeTween?.Kill();
        }
    }
}
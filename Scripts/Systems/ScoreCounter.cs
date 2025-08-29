using System.Collections;
using DG.Tweening;
using Scripts.Enums;
using Scripts.Player;
using Scripts.Systems.Audio;
using TMPro;
using UnityEngine;

namespace Scripts.Systems
{
    public class ScoreCounter : MonoBehaviour
    {
        [Header("Score Settings")]
        [SerializeField] private int _currentScore;
        [SerializeField] private int _constantScoreGrowValue = 1;
        [SerializeField] private float _constantScoreGrowRate = 0.1f;
        [SerializeField] private float _highscoreTextColorChangeSpeed = 0.25f;

        [Header("UI References")]
        [SerializeField] private TMP_Text _scoreText;
        [SerializeField] private TMP_Text _highscoreText;
        [SerializeField] private TMP_Text _finalScoreText;
        [SerializeField] private TextMeshProUGUI _newHighscoreText;

        [Header("Emphasize Animation")]
        [SerializeField] private float _emphasizeDuration = 0.1f;
        [SerializeField] private float _emphasizeScale = 1.2f;

        [Header("DOTween Settings")]
        [SerializeField] private Ease _emphasisEase = Ease.OutBack;
        [SerializeField] private Ease _returnEase = Ease.InOutSine;

        [Header("Dependencies")]
        [SerializeField] private PlayerCore _player;

        private Tween _highscoreTextColorTween;
        private Vector3 _originalScoreScale;
        private Coroutine _scoreGrowthRoutine;
        private WaitForSeconds _scoreCountDelay;
        private int _currentScoreMultiplier = 1;

        public static ScoreCounter Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;

            _originalScoreScale = _scoreText.rectTransform.localScale;

            _scoreCountDelay = new WaitForSeconds(_constantScoreGrowRate);
        }

        private void Start()
        {
            _scoreGrowthRoutine = StartCoroutine(ConstantScoreGrowth());
        }

        public void AddScore(int amount)
        {
            _currentScore += amount * _currentScoreMultiplier;

            UpdateScoreDisplay();
        }

        public void TrySettingHighscore()
        {
            GlobalData.Instance.TrySettingHighScore(_currentScore);
        }

        public void UpdateEndScoreDisplay()
        {
            _finalScoreText.text = _currentScore.ToString();

            if (_currentScore > GlobalData.Instance.HighScore)
            {
                GlobalData.Instance.TrySettingHighScore(_currentScore);
                EnableNewHighscoreAnimation();
            }

            _highscoreText.text = GlobalData.Instance.HighScore.ToString();
        }

        public void EmphasizeScoreText()
        {
            _scoreText.rectTransform.DOKill();
            _scoreText.DOKill();

            _scoreText.rectTransform.localScale = _originalScoreScale;

            Sequence emphasizeSequence = DOTween.Sequence();

            emphasizeSequence.Join(
                _scoreText.rectTransform.DOScale(_originalScoreScale * _emphasizeScale,
                _emphasizeDuration).SetEase(_emphasisEase));

            emphasizeSequence.Join(
                _scoreText.rectTransform.DOScale(_originalScoreScale,
                _emphasizeDuration).SetEase(_returnEase));
        }

        private void EnableNewHighscoreAnimation()
        {
            if (_newHighscoreText == null) return;

            _newHighscoreText.gameObject.SetActive(true);
            AudioManager.Instance.PlaySound(SoundType.NewHighscore);

            AnimateNewHighscoreText();
        }

        private void AnimateNewHighscoreText()
        {
            _highscoreTextColorTween?.Kill();

            _highscoreTextColorTween = _newHighscoreText.DOColor(Color.yellow, _highscoreTextColorChangeSpeed)
                .From(Color.red)
                .SetLoops(-1, LoopType.Yoyo)
                .SetEase(Ease.InOutQuad);
        }

        private IEnumerator ConstantScoreGrowth()
        {
            while (enabled)
            {
                while (!_player.IsRunning && enabled)
                {
                    yield return null;
                }

                AddScore(_constantScoreGrowValue);

                yield return _scoreCountDelay;
            }
        }

        private void UpdateScoreDisplay()
        {
            _scoreText.text = _currentScore.ToString();
        }
    }
}

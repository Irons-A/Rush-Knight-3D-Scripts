using Scripts.Enums;
using Scripts.Systems.Audio;
using UnityEngine;
using UnityEngine.UI;

namespace Scripts.UI.Buttons.Game
{
    [RequireComponent(typeof(Button))]
    public class GameButtons : MonoBehaviour
    {
        [SerializeField] private GameplayScreenControl _gameplayScreenControl;
        [SerializeField] private GameplayButtonTypes _buttonType;

        private Button _button;

        private void Awake()
        {
            _button = GetComponent<Button>();
            _gameplayScreenControl = FindObjectOfType<GameplayScreenControl>();
        }

        protected void OnEnable() =>
            _button.onClick.AddListener(OnButtonClicked);

        protected void OnDisable() =>
            _button.onClick.RemoveListener(OnButtonClicked);

        protected void OnButtonClicked()
        {
            AudioManager.Instance.PlaySound(SoundType.UIclick);

            switch (_buttonType)
            {
                case GameplayButtonTypes.Continue:
                    _gameplayScreenControl.OnContinueClicked();
                    break;
                case GameplayButtonTypes.Exit:
                    _gameplayScreenControl.OnExitClicked();
                    break;
                case GameplayButtonTypes.Pause:
                    _gameplayScreenControl.OnPauseButtonClicked();
                    break;
                case GameplayButtonTypes.Restart:
                    _gameplayScreenControl.OnRestartClicked();
                    break;
                case GameplayButtonTypes.ResultOK:
                    _gameplayScreenControl.OnResultOKClicked();
                    break;
                case GameplayButtonTypes.Revive:
                    _gameplayScreenControl.OnReviveClicked();
                    break;
                case GameplayButtonTypes.CancelRevive:
                    _gameplayScreenControl.OnReviveCancelClicked();
                    break;
            }
        }
    }
}
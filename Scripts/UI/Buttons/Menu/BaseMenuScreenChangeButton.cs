using Scripts.Enums;
using Scripts.Systems.Audio;
using UnityEngine;
using UnityEngine.UI;

namespace Scripts.UI.Buttons.Menu
{
    [RequireComponent(typeof(Button))]
    public abstract class BaseMenuScreenChangeButton : MonoBehaviour
    {
        private Button _button;

        protected abstract MenuScreenControl MenuControl { get; }

        protected virtual void Awake()
        {
            _button = GetComponent<Button>();
        }

        protected virtual void OnEnable() =>
            _button.onClick.AddListener(OnButtonClicked);

        protected virtual void OnDisable() =>
            _button.onClick.RemoveListener(OnButtonClicked);

        protected virtual void OnButtonClicked()
        {
            AudioManager.Instance.PlaySound(SoundType.UIclick);
        }
    }
}

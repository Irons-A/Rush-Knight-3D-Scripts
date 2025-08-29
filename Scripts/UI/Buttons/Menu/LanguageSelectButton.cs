using Scripts.Systems;
using UnityEngine;
using UnityEngine.UI;

namespace Scripts.UI.Buttons.Menu
{
    [RequireComponent(typeof(Button))]
    public class LanguageSelectButton : MonoBehaviour
    {
        [SerializeField] private int _buttonLocaleIndex = 0;

        private Button _button;

        private void Awake()
        {
            _button = GetComponent<Button>();
        }

        private void OnEnable()
        {
            _button.onClick.AddListener(SetLanguageLocale);
        }

        private void OnDisable()
        {
            _button.onClick.RemoveListener(SetLanguageLocale);
        }

        private void SetLanguageLocale()
        {
            GlobalData.Instance.SetLanguage(_buttonLocaleIndex);
        }
    }
}

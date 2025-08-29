using Scripts.Enums;
using Scripts.Systems.Audio;
using UnityEngine;
using UnityEngine.UI;

namespace Scripts.UI.Buttons.Menu
{
    [RequireComponent(typeof(Button))]
    public class CancelAuthorizationButton : MonoBehaviour
    {
        [SerializeField] private GameObject AuthorizationCanvas;

        private Button _button;

        private void Awake()
        {
            _button = GetComponent<Button>();
        }

        private void OnEnable()
        {
            _button.onClick.AddListener(OnClick);
        }

        private void OnDisable()
        {
            _button.onClick.RemoveListener(OnClick);
        }

        private void OnClick()
        {
            AudioManager.Instance.PlaySound(SoundType.UIclick);
            AuthorizationCanvas.SetActive(false);
        }
    }
}

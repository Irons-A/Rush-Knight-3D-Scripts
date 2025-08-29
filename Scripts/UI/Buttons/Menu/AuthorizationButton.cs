using Scripts.Enums;
using Scripts.Systems.Audio;
using UnityEngine;
using UnityEngine.UI;
using YG;

namespace Scripts.UI.Buttons.Menu
{
    [RequireComponent(typeof(Button))]
    public class AuthorizationButton : MonoBehaviour
    {
        [SerializeField] private GameObject AuthorizationCanvas;

        private Button _button;

        private void Awake()
        {
            _button = GetComponent<Button>();
        }

        private void OnEnable()
        {
            _button.onClick.AddListener(OpenAuthorizationCanvas);
            YG2.onGetSDKData += VerifyVisibility;

            VerifyVisibility();
        }

        private void OnDisable()
        {
            _button.onClick.RemoveListener(OpenAuthorizationCanvas);
            YG2.onGetSDKData -= VerifyVisibility;
        }

        private void VerifyVisibility()
        {
            if (YG2.player.auth == true)
            {
                gameObject.SetActive(false);
            }
        }

        private void OpenAuthorizationCanvas()
        {
            AudioManager.Instance.PlaySound(SoundType.UIclick);
            AuthorizationCanvas.SetActive(true);
        }
    }
}

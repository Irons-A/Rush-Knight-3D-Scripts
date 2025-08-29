using Scripts.Enums;
using Scripts.Systems.Audio;
using Scripts.Systems;
using UnityEngine;
using UnityEngine.UI;

namespace Scripts.UI.Buttons.Menu
{
    [RequireComponent(typeof(Button))]
    public class MenuGoToLevelButton : MonoBehaviour
    {
        [SerializeField] private SceneNames _targetScene;

        private string _targetSceneName;
        private Button _button;

        private void Awake()
        {
            _button = GetComponent<Button>();
        }

        private void Start()
        {
            bool isTutorialCompleted = GlobalData.Instance.TutorialCompleted;

            if (_targetScene == SceneNames.Game)
            {
                if (isTutorialCompleted == false)
                {
                    _targetSceneName = SceneNames.Tutorial.ToString();
                }
                else
                {
                    _targetSceneName = _targetScene.ToString();
                }
            }
            else if (_targetScene == SceneNames.Tutorial)
            {
                _targetSceneName = _targetScene.ToString();
            }
        }

        private void OnEnable()
        {
            _button.onClick.AddListener(OnButtonClicked);
        }

        private void OnDisable()
        {
            _button.onClick.RemoveListener(OnButtonClicked);
        }

        private void OnButtonClicked()
        {
            if (_targetSceneName != null)
            {
                AudioManager.Instance.PlaySound(SoundType.UIclick);
                LoadingScreen.Instance.LoadScene(_targetSceneName);
            }
        }
    }
}

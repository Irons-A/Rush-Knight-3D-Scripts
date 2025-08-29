using Scripts.Systems;
using UnityEngine;

namespace Scripts.Player
{
    public class MenuPlayerControl : MonoBehaviour
    {
        [SerializeField] private GameObject _characterSelectionCanvas;
        [SerializeField] private Transform[] _characterModels;

        private int _currentPreviewIndex = 0;
        private bool _usePreview;

        private void Start()
        {
            _currentPreviewIndex = GlobalData.Instance.SelectedCharacter;
            UpdateCharacterDisplay();
        }

        private void Update()
        {
            _usePreview = _characterSelectionCanvas.activeInHierarchy;
            UpdateCharacterDisplay();
        }

        public void UpdateCharacterDisplay()
        {
            for (int i = 0; i < _characterModels.Length; i++)
            {
                bool shouldShow = _usePreview ?
                    (i == _currentPreviewIndex) :
                    (i == GlobalData.Instance.SelectedCharacter);

                _characterModels[i].gameObject.SetActive(shouldShow);
            }
        }

        public void CycleCharacter(int direction)
        {
            _currentPreviewIndex = (_currentPreviewIndex + direction + _characterModels.Length) % _characterModels.Length;
            UpdateCharacterDisplay();
        }

        public void SetPreviewIndex(int index)
        {
            _currentPreviewIndex = index;
            UpdateCharacterDisplay();
        }
    }
}
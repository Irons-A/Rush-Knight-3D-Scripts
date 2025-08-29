using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace Scripts.Tutorial
{
    public class TutorialControl : MonoBehaviour
    {
        [Header("Tutorial Settings")]
        [SerializeField] private List<TextMeshProUGUI> _tutorialTexts;
        [SerializeField] private float _tutorialTextSizeIncrease = 1.25f;
        [SerializeField] private float _tutorialTextGrowPhaseLength = 0.25f;

        private void Start()
        {
            DisableTutorialTexts();
        }

        public void EnableTutorialText(int index)
        {
            DisableTutorialTexts();

            TextMeshProUGUI targetText = _tutorialTexts[index];
            targetText.gameObject.SetActive(true);

            AnimateTutorialText(targetText);
        }

        private void DisableTutorialTexts()
        {
            foreach (var text in _tutorialTexts)
            {
                text.gameObject.SetActive(false);
            }
        }

        private void AnimateTutorialText(TextMeshProUGUI text)
        {
            RectTransform rectTransform = text.rectTransform;

            rectTransform.localScale = Vector3.one;

            Sequence seq = DOTween.Sequence();
            seq.Append(rectTransform.DOScale(_tutorialTextSizeIncrease, _tutorialTextGrowPhaseLength));
            seq.Append(rectTransform.DOScale(1f, _tutorialTextGrowPhaseLength));
        }
    }
}

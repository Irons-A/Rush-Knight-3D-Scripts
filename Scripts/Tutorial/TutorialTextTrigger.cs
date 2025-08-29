using Scripts.Player;
using UnityEngine;

namespace Scripts.Tutorial
{
    [RequireComponent(typeof(BoxCollider))]
    public class TutorialTextTrigger : MonoBehaviour
    {
        [SerializeField] private TutorialControl _tutorialControl;
        [SerializeField] private int _textIndex;

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out PlayerCore _))
            {
                _tutorialControl.EnableTutorialText(_textIndex);
            }
        }
    }
}

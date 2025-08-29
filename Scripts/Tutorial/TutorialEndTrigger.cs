using Scripts.Player;
using Scripts.UI;
using UnityEngine;

namespace Scripts.Tutorial
{
    [RequireComponent(typeof(BoxCollider))]
    public class TutorialEndTrigger : MonoBehaviour
    {
        [SerializeField] private GameplayScreenControl _screenControl;

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out PlayerCore _))
            {
                _screenControl.GoToGame();
            }
        }
    }
}

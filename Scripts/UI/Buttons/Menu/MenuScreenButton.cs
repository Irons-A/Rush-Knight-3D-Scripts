using UnityEngine;

namespace Scripts.UI.Buttons.Menu
{
    public class MenuScreenButton : BaseMenuScreenChangeButton
    {
        [SerializeField] private MenuScreenControl _menuControl;

        protected override MenuScreenControl MenuControl => _menuControl;

        protected override void OnButtonClicked()
        {
            if (_menuControl != null)
            {
                _menuControl.ShowMenuCanvas();
            }

            base.OnButtonClicked();
        }
    }
}

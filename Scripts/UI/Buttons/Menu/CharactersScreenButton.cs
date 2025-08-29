using UnityEngine;

namespace Scripts.UI.Buttons.Menu
{
    public class CharactersScreenButton : BaseMenuScreenChangeButton
    {
        [SerializeField] private MenuScreenControl _menuControl;

        protected override MenuScreenControl MenuControl => _menuControl;

        protected override void OnButtonClicked()
        {
            if (_menuControl != null)
            {
                _menuControl.ShowCharactersCanvas();
            }

            base.OnButtonClicked();
        }
    }
}

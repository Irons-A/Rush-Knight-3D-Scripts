using Scripts.Enums;
using System.Collections;
using UnityEngine;
using YG;

namespace Scripts.Systems
{
    public class InitializationLoader : MonoBehaviour
    {
        [SerializeField] private GameObject _globalSystems;

        private IEnumerator Start()
        {
            yield return new WaitUntil(() => YG2.isSDKEnabled);

            _globalSystems.SetActive(true);

            LoadingScreen.Instance.LoadScene(SceneNames.MainMenu.ToString());
        }
    }
}

using Scripts.Enums;
using Scripts.Systems.Audio;
using Scripts.Systems;
using UnityEngine;
using UnityEngine.UI;

namespace Scripts.UI
{
    [RequireComponent(typeof(Slider))]
    public class VolumeSlider : MonoBehaviour
    {
        [SerializeField] private SoundSliderType _type = SoundSliderType.Sounds;

        private Slider _slider;

        private void Awake()
        {
            _slider = GetComponent<Slider>();
        }

        private void OnEnable()
        {
            _slider.onValueChanged.AddListener(ChangeVolume);
        }

        private void OnDisable()
        {
            _slider.onValueChanged.RemoveListener(ChangeVolume);
        }

        private void Start()
        {
            if (_type == SoundSliderType.Sounds)
            {
                _slider.value = GlobalData.Instance.SoundVolume;
            }
            else if (_type == SoundSliderType.Music)
            {
                _slider.value = GlobalData.Instance.MusicVolume;
            }
        }

        private void ChangeVolume(float value)
        {
            if (_type == SoundSliderType.Sounds)
            {
                AudioManager.Instance.SetSFXVolume(_slider.value);
            }
            else if (_type == SoundSliderType.Music)
            {
                AudioManager.Instance.SetMusicVolume(_slider.value);
            }
        }
    }
}

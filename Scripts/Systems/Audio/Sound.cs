using Scripts.Enums;
using UnityEngine;

namespace Scripts.Systems.Audio
{
    [System.Serializable]
    public class Sound
    {
        public SoundType Type;
        public AudioClip Clip;
        [Range(0f, 1f)] public float BaseVolume = 1f;
    }
}
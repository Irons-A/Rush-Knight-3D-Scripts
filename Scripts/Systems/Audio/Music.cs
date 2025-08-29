using UnityEngine;

namespace Scripts.Systems.Audio
{
    [System.Serializable]
    public class Music
    {
        public AudioClip Clip;
        [Range(0f, 1f)] public float BaseVolume = 1f;
    }
}

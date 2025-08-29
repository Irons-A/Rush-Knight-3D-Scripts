using UnityEngine;

namespace Config.Gems
{
    [CreateAssetMenu(fileName = "GemType", menuName = "Scriptable Objects/Gem Type", order = 51)]
    public class GemType : ScriptableObject
    {
        public Material material;
        public int scoreValue;
        public ParticleSystem collectEffect;
    }
}

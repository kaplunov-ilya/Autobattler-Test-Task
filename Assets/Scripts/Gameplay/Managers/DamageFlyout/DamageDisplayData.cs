using UnityEngine;

namespace Configs
{
    [CreateAssetMenu(menuName = "Config/Damage Display Data", fileName = "DamageDisplayData", order = -1010)]
    public sealed class DamageDisplayData : ScriptableObject
    {
        /// <summary>
        /// Насроенный партикл для текста
        /// </summary>
        [field:Header("Вставить настроенный партикл для текста")]
        [field: SerializeField] public ParticleSystem TextParticleSystem { get; private set; }
        
        /// <summary>
        /// Основной размер частицы
        /// </summary>
        [field:Header("Основной размер частицы")]
        [field: SerializeField] public float DefaultSizeForParticle { get; private set; } = 0.3f;
        
        [field:Header("Ширина одного символа")]
        [field: SerializeField] public float CharWidth { get; private set; } = 1;

        
    }
}
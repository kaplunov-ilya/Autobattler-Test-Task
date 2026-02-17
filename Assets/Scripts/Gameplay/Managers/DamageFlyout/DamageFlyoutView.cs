using Domain.Gameplay.MessagesDTO.UI.DamageFlyout;
using UnityEngine;

namespace UI.Gameplay.Views.DamageFlyout
{
    public sealed class DamageFlyoutView : MonoBehaviour
    {
        private readonly RendererParticleSystem _textSystem = new();

        public void SetTextSystem(ParticleSystem particle) => _textSystem.SetParticleSystem(particle);
        
        public void ShowText(DamageDisplaySettings settings) => Show(_textSystem, settings);
        
        private void Show(RendererParticleSystem system, DamageDisplaySettings settings)
        {
            system.SpawnParticle(settings.Position, settings.Message, settings.Color, settings.Size);
        }
    }
}
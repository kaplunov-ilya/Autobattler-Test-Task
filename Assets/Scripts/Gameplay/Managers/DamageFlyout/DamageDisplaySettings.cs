using UnityEngine;

namespace Domain.Gameplay.MessagesDTO.UI.DamageFlyout
{
    public struct DamageDisplaySettings
    {
        public DamageDisplaySettings(Vector3 position, string message, float size)
        {
            Position = position;
            Message = message;
            Color = Color.white;
            Size = size;
        }

        public Vector3 Position { get; }
        public string Message { get; }
        public Color Color { get; }
        public float Size { get; }
    }
}
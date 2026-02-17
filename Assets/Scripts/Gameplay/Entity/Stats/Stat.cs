using UniRx;

namespace Gameplay.Entity.Stats
{
    public sealed class Stat
    {
        private readonly ReactiveProperty<float> _property = new();

        public IReadOnlyReactiveProperty<float> Property => _property;
        
        public EStat StatType { get; }

        public Stat(EStat statType, float value)
        {
            StatType = statType;
            _property.Value = value;
        }

        public float Value => _property.Value;
        
        public void SetNewValue(float value) => _property.Value = value;

        public void ModifyValue(float value) =>  _property.Value += value;
    }
}
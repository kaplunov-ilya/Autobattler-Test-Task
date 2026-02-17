using Gameplay.Entity.ActorEntity;
using Gameplay.Entity.Stats;
using UnityEngine;

namespace Gameplay.Entity.Behaviour.Defence
{
    public sealed class DefenceBehaviour : Behaviour, IDefenceBehaviour
    {
        private Stat _health;
        
        public override void SetActor(Actor actor)
        {
            base.SetActor(actor);
            
            actor.Stats.TryGetValue(EStat.Health, out _health);
        }

        public void TakeDamage(float damage)
        {
            if (_health == null)
            {
                Debug.LogError($"[{nameof(DefenceBehaviour)}.{nameof(TakeDamage)}] health is null");
                return;
            }
            
            var healthValue = _health.Value;

            if (healthValue - damage < 0)
            {
                damage = healthValue;
            }
            
            Debug.Log($"[{nameof(DefenceBehaviour)}.{nameof(TakeDamage)}] damage: {damage}");
            
            _health.ModifyValue(-damage);
        }
    }
}
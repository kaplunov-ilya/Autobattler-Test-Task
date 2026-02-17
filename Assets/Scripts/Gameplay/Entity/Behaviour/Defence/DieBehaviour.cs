using System;
using Gameplay.Entity.ActorEntity;
using Gameplay.Entity.Stats;
using UniRx;
using UnityEngine;

namespace Gameplay.Entity.Behaviour.Defence
{
    public sealed class DieBehaviour : Behaviour, IDieBehaviour, IDisposable
    {
        private Stat _health;
        
        private readonly CompositeDisposable _disposables = new();
        public override void SetActor(Actor actor)
        {
            base.SetActor(actor);
            
            actor.Stats.TryGetValue(EStat.Health, out _health);

            BindHealth();
        }

        private void BindHealth()
        {
            if(_health == null)
            {
                Debug.LogError($"[{nameof(DieBehaviour)}.{nameof(BindHealth)}] health is null");
                return;
            }
            
            _health.Property
                   .Subscribe(CheckAlive)
                   .AddTo(_disposables);
        }

        private void CheckAlive(float value)
        {
            if (value <= 0)
                SetDie();
        }

        private void SetDie()
        {
            Actor.IsAlive.Value = false;
        }

        public void Dispose()
        {
            _disposables?.Dispose();
        }
    }
}
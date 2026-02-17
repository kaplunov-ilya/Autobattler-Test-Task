using Configs;
using Domain.Gameplay.MessagesDTO.UI.DamageFlyout;
using System;
using System.Globalization;
using Gameplay.Managers.Models;
using UI.Gameplay.Views.DamageFlyout;
using UniRx;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using Object = UnityEngine.Object;

namespace UI.Gameplay.Presenters.DamageFlyout
{
    public sealed class DamageFlyoutPresenter : IInitializable, IDisposable
    {
        [Inject] private readonly DamageFlyoutView _view;
        [Inject] private readonly DamageDisplayData _data;
        [Inject] private readonly GameData _gameData;
        
        private ParticleSystem _particles;

        private Transform _parentForParticles;

        private CompositeDisposable _disposables = new();

        public void Initialize()
        {
            _parentForParticles = new GameObject("ParticleParent").transform;
            
            _particles = Object.Instantiate(_data.TextParticleSystem, _parentForParticles);
            
            _gameData.Attack
                     .Subscribe(context => Handle(context.Target.Position, context.Damage)).AddTo(_disposables)
                     .AddTo(_disposables);
            
            _view.SetTextSystem(_particles);
        }

        private void Handle(Transform position, float damage) 
        {
            Vector3 pos = position.position;
            string textToSpawn = damage.ToString(CultureInfo.InvariantCulture);
            
            var textSize = _data.DefaultSizeForParticle ;

            _view.ShowText(CreateSettings(pos, textToSpawn, textSize));
        }

        private DamageDisplaySettings CreateSettings(Vector3 position, string message, float size)
        {
            return new(new Vector3(position.x,position.y + 2,position.z), message, size);
        }
        
        public void Dispose()
        {
            _disposables?.Dispose();
            
            if (_parentForParticles != null)
            {
                Object.Destroy(_parentForParticles.gameObject);
                _parentForParticles = null;
            }
        }
    }
}
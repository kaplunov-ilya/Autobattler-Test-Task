using System.Collections.Generic;
using System.Linq;
using Configs;
using Gameplay.Entity.ActorEntity;
using Gameplay.Managers;
using Gameplay.Managers.Models;
using Gameplay.Managers.Spawner;
using UI;
using UI.GameHud;
using UI.Gameplay.Presenters.DamageFlyout;
using UI.Gameplay.Views.DamageFlyout;
using UI.Menu;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using Object = UnityEngine.Object;

namespace Core
{
    public sealed class CoreLifetimeScope : LifetimeScope
    {
        [SerializeField] private SpawnerPoints _spawnerPoints;
        [SerializeField] private List<ActorConfig> _actorConfigs;
        [SerializeField] private List<BaseView> _views;
        [SerializeField] private Transform _viewParent;
        [SerializeField] private DamageFlyoutView _damageFlyoutView;
        [SerializeField] private DamageDisplayData _damageDisplayData;
        
         protected override void Configure(IContainerBuilder builder)
        {
            BuildConfigs(builder);
            BindViews(builder);
            BuildModels(builder);
            BuildPresenters(builder);
            BuildManagers(builder);

            builder.RegisterEntryPoint<GameManager>();
        }

        private void BindViews(IContainerBuilder builder)
        {
            builder.RegisterInstance(_damageFlyoutView);
            
            foreach (var prefab in _views)
            {
                var instance = Object.Instantiate(prefab, _viewParent);
                instance.name = prefab.name;
                instance.Hide();

                builder.RegisterInstance(instance)
                       .As(
                            instance.GetType().GetInterfaces()
                                    .Where(i => typeof(IView).IsAssignableFrom(i) && i != typeof(IView))
                                    .ToArray()
                            );
            }
        }

        private void BuildConfigs(IContainerBuilder builder)
        {
            builder.RegisterInstance(_actorConfigs);
            builder.RegisterInstance(_damageDisplayData);
            builder.Register<ActorFactory>(Lifetime.Singleton);
        }

        private void BuildModels(IContainerBuilder builder)
        {
            builder.Register<GameData>(Lifetime.Singleton);
            builder.Register<GameStateData>(Lifetime.Singleton);
        }
        
        private void BuildPresenters(IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<MenuPresenter>();
            builder.RegisterEntryPoint<DamageFlyoutPresenter>();
            builder.RegisterEntryPoint<HudPresenter>();
        }

        private void BuildManagers(IContainerBuilder builder)
        {
            builder.RegisterInstance(_spawnerPoints);
            builder.RegisterEntryPoint<SpawnerManager>();
            
            builder.Register<TargetingManager>(Lifetime.Singleton);
            builder.RegisterEntryPoint<FightManager>();
        }
    }
}
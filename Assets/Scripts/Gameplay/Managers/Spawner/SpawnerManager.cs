using System;
using System.Collections.Generic;
using Gameplay.Entity.ActorEntity;
using Gameplay.Managers.Models;
using UniRx;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using Object = UnityEngine.Object;

namespace Gameplay.Managers.Spawner
{
    public sealed class SpawnerManager : IInitializable,  IDisposable
    {
        [Inject] private readonly GameData _gameData;
        [Inject] private readonly GameStateData _gameStateData;
        [Inject] private readonly List<ActorConfig> _listActorConfig;
        [Inject] private readonly ActorFactory _actorFactory;
        [Inject] private readonly SpawnerPoints _spawnerPoints;
        
        private readonly CompositeDisposable _disposable = new();
        
        public void Initialize()
        {
            _gameStateData.GameState
                          .Where(s => s == EGameState.SpawnUnits)
                          .Subscribe(_ => SpawnRandomActors())
                          .AddTo(_disposable);
            
            _gameStateData.RequestRespawnUnitsCommand
                          .Subscribe(_ => SpawnRandomActors())
                          .AddTo(_disposable);
        }
        
        private void SpawnRandomActors()
        {
            RemoveAllActors();
            SpawnAllActors();
        }

        private void RemoveAllActors()
        {
            var allActors = new List<Actor>();
            allActors.AddRange(_gameData.TeamA);
            allActors.AddRange(_gameData.TeamB);
            
            foreach (var actor in allActors)
            {
                Object.Destroy(actor.ActorObject.gameObject);
            }
            
            _gameData.TeamA.Clear();
            _gameData.TeamB.Clear();
            
            _spawnerPoints.SetAllIsFree();
        }
        
        private void SpawnAllActors()
        {
            System.Random rnd = new System.Random();

            SpawnTeam(_gameData.TeamA, ETeam.TeamA, rnd);
            SpawnTeam(_gameData.TeamB, ETeam.TeamB, rnd);
        }

        private void SpawnTeam(List<Actor> actors, ETeam team, System.Random rnd)
        {
            for (int i = 0; i <= 9; i++)
            {
                var point = _spawnerPoints.GetFreePoint(team);

                if (point == null)
                {
                    Debug.LogError($"[{nameof(SpawnerManager)}.{nameof(SpawnTeam)}] point is null");
                    return;
                }
                
                var randomConfig = _listActorConfig[rnd.Next(_listActorConfig.Count)];

                var actor = _actorFactory.Factory(randomConfig);

                actor.ActorObject.gameObject.transform.position = point.position;
                actor.Team = team;
                
                actors.Add(actor);
            }
        }

        public void Dispose()
        {
            _disposable?.Dispose();
        }
    }
}
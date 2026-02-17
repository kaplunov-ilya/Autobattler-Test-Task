using System;
using Gameplay.Entity.ActorEntity;
using Gameplay.Managers.Models;
using UniRx;
using VContainer;
using VContainer.Unity;
using Object = UnityEngine.Object;

namespace Gameplay.Managers
{
    public sealed class FightManager : IInitializable, IDisposable
    {
        [Inject] private readonly GameData _gameData;
        [Inject] private readonly GameStateData _gameStateData;
        [Inject] private readonly TargetingManager _targetingManager;
        
        private readonly CompositeDisposable _disposable = new();
        
        private CompositeDisposable _fightDisposable = new();

        public void Initialize()
        {
            _gameStateData.GameState
                          .Subscribe(StartGame)
                          .AddTo(_disposable);
        }

        private void StartGame(EGameState state)
        {
            if(state != EGameState.Fight)
                return;

            var allActors = _gameData.AllActors;

            _fightDisposable = new();

            foreach (var actor in allActors)
            {
                actor.IsAlive.Value = true;
                
                actor.IsAlive
                     .Where(a => !a)
                     .Subscribe(_ => DieActor(actor))
                     .AddTo(_fightDisposable);
                
                actor.Attack
                     .Subscribe(context => _gameData.Attack.Execute(context))
                     .AddTo(_fightDisposable);
            }
            
            _targetingManager.StartFight();
        }

        private void DieActor(Actor actor)
        {
            foreach (var behavioursValue in actor.Behaviours.Values)
            {
                behavioursValue.SetDisable();
            }

            _gameData.TeamA.Remove(actor);
            _gameData.TeamB.Remove(actor);
            
            actor.ActorObject.gameObject.SetActive(false);

            CheckWin();
        }

        private void CheckWin()
        {
            if (_gameData.TeamA.Count == 0 || _gameData.TeamB.Count == 0)
                EndGame();
        }

        private void EndGame()
        {
            _fightDisposable?.Dispose();
            _targetingManager.StopFight();

            var allActors = _gameData.AllActors;

            foreach (var actor in allActors)
            {
                foreach (var behavioursValue in actor.Behaviours.Values)
                {
                    behavioursValue.SetDisable();
                }
            }

            _gameStateData.RequestEndGameCommand.Execute();
        }

        public void Dispose()
        {
            _disposable?.Dispose();
            _fightDisposable?.Dispose();
        }
    }
}
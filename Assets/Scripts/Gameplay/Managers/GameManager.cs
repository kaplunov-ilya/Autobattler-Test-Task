using System;
using Gameplay.Managers.Models;
using UniRx;
using VContainer;
using VContainer.Unity;
using Application = UnityEngine.Application;

namespace Gameplay.Managers
{
    public sealed class GameManager : IInitializable, IDisposable
    {
        [Inject] private readonly GameStateData _gameStateData;
        
        private readonly CompositeDisposable _disposable = new();
        
        public void Initialize()
        {
            OpenMenu();
            
            _gameStateData.RequestStartGameCommand
                          .Subscribe(_ => OpenGameplay())
                          .AddTo(_disposable);
            
            _gameStateData.RequestStartFightCommand
                          .Subscribe(_ => StartFight())
                          .AddTo(_disposable);
            
            _gameStateData.RequestEndGameCommand
                          .Subscribe(_ => OpenMenu())
                          .AddTo(_disposable);
            
            _gameStateData.RequestExitGameCommand
                          .Subscribe(_ => ExitGame())
                          .AddTo(_disposable);
        }

        private void OpenMenu()
        {
            _gameStateData.GameState.Value = EGameState.MainMenu;
        }

        private void OpenGameplay()
        {
            _gameStateData.GameState.Value = EGameState.SpawnUnits;
        }

        private void StartFight()
        {
            _gameStateData.GameState.Value = EGameState.Fight;
        }

        private void ExitGame()
        {
            Application.Quit();
        }

        public void Dispose()
        {
            _disposable?.Dispose();
        }
    }
}
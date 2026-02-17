using System;
using Gameplay.Managers.Models;
using UniRx;
using VContainer;
using VContainer.Unity;
using Unit = UniRx.Unit;

namespace UI.Menu
{
    public sealed class MenuPresenter : IInitializable, IDisposable
    {
        [Inject] private readonly IMenuView _menuView;
        [Inject] private readonly GameStateData _gameStateData;
        
        private readonly CompositeDisposable _disposable = new();

        public void Initialize()
        {
            _gameStateData.GameState
                          .Subscribe(SetActiveWindow)
                          .AddTo(_disposable);
            
            _menuView.StartButton.onClick.
                      AsObservable()
                     .Subscribe(SendRequestStartGame)
                     .AddTo(_disposable);
            
            _menuView.ExitButton.onClick.
                      AsObservable()
                     .Subscribe(SendRequestExitGame)
                     .AddTo(_disposable);
        }

        private void SetActiveWindow(EGameState state)
        {
            if(state == EGameState.MainMenu)
                _menuView.Show();
            else
                _menuView.Hide();
        }

        private void SendRequestStartGame(Unit unit)
        {
            _gameStateData.RequestStartGameCommand.Execute();
        }
        
        private void SendRequestExitGame(Unit unit)
        {
            _gameStateData.RequestExitGameCommand.Execute();
        }

        public void Dispose()
        {
            _disposable?.Dispose();
        }
    }
}
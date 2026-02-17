using System;
using Gameplay.Managers.Models;
using UniRx;
using VContainer;
using VContainer.Unity;
using Unit = UniRx.Unit;

namespace UI.GameHud
{
    public sealed class HudPresenter : IInitializable, IDisposable
    {
        [Inject] private readonly IHudView _hudView;
        [Inject] private readonly GameStateData _gameStateData;
        
        private readonly CompositeDisposable _disposable = new();
        
        public void Initialize()
        {
            _gameStateData.GameState
                          .Subscribe(SetActiveWindow)
                          .AddTo(_disposable);
            
            _hudView.StartFightButton.onClick.
                     AsObservable()
                    .Subscribe(StartFight)
                    .AddTo(_disposable);
            
            _hudView.RespawnButton.onClick.
                     AsObservable()
                    .Subscribe(RespawnUnits)
                    .AddTo(_disposable);
        }
        
        private void SetActiveWindow(EGameState state)
        {
            if(state == EGameState.SpawnUnits)
                _hudView.Show();
            else
                _hudView.Hide();
        }

        private void StartFight(Unit unit)
        {
            _gameStateData.RequestStartFightCommand.Execute();
        }

        private void RespawnUnits(Unit unit)
        {
            _gameStateData.RequestRespawnUnitsCommand.Execute();
        }
        
        public void Dispose()
        {
            _disposable?.Dispose();
        }
    }
}
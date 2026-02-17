using UniRx;

namespace Gameplay.Managers.Models
{
    public sealed class GameStateData
    {
        public ReactiveProperty<EGameState> GameState { get; } = new();
        
        public ReactiveCommand RequestStartGameCommand { get; } = new();
        public ReactiveCommand RequestStartFightCommand { get; } = new();
        public ReactiveCommand RequestEndGameCommand { get; } = new();
        public ReactiveCommand RequestExitGameCommand { get; } = new();
        public ReactiveCommand RequestRespawnUnitsCommand { get; } = new();
    }
}
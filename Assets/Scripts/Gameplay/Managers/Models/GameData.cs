using System.Collections.Generic;
using Gameplay.Entity.ActorEntity;
using Gameplay.Entity.Behaviour.Attack;
using UniRx;

namespace Gameplay.Managers.Models
{
    public sealed class GameData
    {
        public List<Actor> TeamA { get; } = new();
        public List<Actor> TeamB { get; } = new();

        public ReactiveCommand<AttackContext> Attack { get; } = new();
        
        public List<Actor> AllActors
        {
            get
            {
                var list = new List<Actor>();
                
                list.AddRange(TeamA);
                list.AddRange(TeamB);
                return list;
            }
        }
    }
}
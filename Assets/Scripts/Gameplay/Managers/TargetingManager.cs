using System;
using System.Collections.Generic;
using Gameplay.Entity.ActorEntity;
using Gameplay.Entity.Behaviour.Attack;
using Gameplay.Entity.Behaviour.Defence;
using Gameplay.Managers.Models;
using UniRx;
using VContainer;
using Random = UnityEngine.Random;

namespace Gameplay.Managers
{
    public sealed class TargetingManager : IDisposable
    {
        [Inject] private readonly GameData _gameData;
        
        private CompositeDisposable _fightDisposable = new();

        public void StartFight()
        {
            _fightDisposable = new();
            
            var allActors = _gameData.AllActors;
            
            foreach (var actor in allActors)
            {
                actor.IsAlive
                     .Where(a => !a)
                     .Subscribe(_ => SetEnemy())
                     .AddTo(_fightDisposable);
            }

            SetEnemy();
        }

        public void StopFight()
        {
            _fightDisposable?.Dispose();
        }

        private void SetEnemy()
        {
            var allActors = _gameData.AllActors;
            
            foreach (var actor in allActors)
            {
                if(!actor.Behaviours.TryGetValue(typeof(IAttackBehaviour), out var behaviour))
                    return;

                if(behaviour is not AttackBehaviour attack)
                    return;

                if (attack.Target != null && attack.Target.Actor.IsAlive.Value == true)
                    continue;
                
                var enemy = GetEnemyList(actor.Team);
                if(enemy ==  null || enemy.Count == 0) 
                    return;

                var enemyActor = enemy[Random.Range(0, enemy.Count)];
                enemyActor.Behaviours.TryGetValue(typeof(IDefenceBehaviour), out var defence);
                attack.SetTarget(defence as DefenceBehaviour);
            }
        }

        private List<Actor> GetEnemyList(ETeam team)
        {
            switch (team)
            {
                case   ETeam.TeamA:
                    return _gameData.TeamB;
                case   ETeam.TeamB:
                    return _gameData.TeamA;
            }
            
            return null;
        }

        public void Dispose() => _fightDisposable?.Dispose();
    }
}
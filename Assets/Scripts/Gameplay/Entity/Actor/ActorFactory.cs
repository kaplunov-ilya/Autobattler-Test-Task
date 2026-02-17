using System.Collections.Generic;
using Gameplay.Entity.Behaviour.Attack;
using Gameplay.Entity.Behaviour.Defence;
using Gameplay.Entity.Behaviour.Move;
using Gameplay.Entity.Stats;
using UnityEngine;

namespace Gameplay.Entity.ActorEntity
{
    public sealed class ActorFactory
    {
        public Actor Factory(ActorConfig config)
        {
            var actor = new Actor();
            var actorView = Object.Instantiate(config.ActorViewPrefab);
            actor.ActorObject = actorView;

            SetStats(config, actor);
            Modify(config, actorView, actor.Stats);
            SetBehaviours(actor);

            return actor;
        }

        private void Modify(ActorConfig config, ActorObject actorView, Dictionary<EStat, Stat> stats)
        {
            foreach (var modifier in config.Modifiers)
            {
                modifier.SetModify(actorView, stats);
            }
        }

        private void SetStats(ActorConfig config, Actor actor)
        {
            var stats = new Dictionary<EStat, Stat>();

            foreach (var statConfig in config.Stats)
            {
                stats.Add(statConfig.StatType, new Stat(statConfig.StatType, statConfig.Value));
            }

            actor.Stats = stats;
        }

        private void SetBehaviours(Actor actor)
        {
            actor.Behaviours.Add(typeof(IAttackBehaviour), new AttackBehaviour());            
            actor.Behaviours.Add(typeof(IDefenceBehaviour), new DefenceBehaviour());            
            actor.Behaviours.Add(typeof(IMoveBehaviour), new MoveBehaviour());
            actor.Behaviours.Add(typeof(IDieBehaviour), new DieBehaviour());

            foreach (var behaviour in actor.Behaviours.Values)
            {
                behaviour.SetActor(actor);
            }
        }
    }
}
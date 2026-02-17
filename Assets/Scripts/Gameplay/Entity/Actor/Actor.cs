using System;
using System.Collections.Generic;
using Gameplay.Entity.Behaviour;
using Gameplay.Entity.Behaviour.Attack;
using Gameplay.Entity.Stats;
using Gameplay.Managers.Models;
using UniRx;
using UnityEngine;

namespace Gameplay.Entity.ActorEntity
{
    public sealed class Actor
    {
        public ETeam Team { get; set; }
        public Dictionary<Type, IBehaviour> Behaviours { get; set; } = new();
        public Dictionary<EStat, Stat> Stats { get; set; } = new();
        
        public ActorObject ActorObject { get; set; }
        
        public ReactiveProperty<bool> IsAlive { get; } = new();
        
        public Transform Position => ActorObject.PositionPoint;
        
        public ReactiveCommand<AttackContext> Attack { get; } = new();
    }
}
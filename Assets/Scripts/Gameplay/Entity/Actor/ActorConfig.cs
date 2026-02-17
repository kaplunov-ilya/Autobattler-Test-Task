using System;
using System.Collections.Generic;
using Gameplay.Entity.Selectors.Config;
using Gameplay.Entity.Stats;
using UnityEngine;

namespace Gameplay.Entity.ActorEntity
{
    [CreateAssetMenu(fileName = "ActorConfig", menuName = "Config/ActorConfig")]
    public sealed class ActorConfig : ScriptableObject
    {
        [field: SerializeField] public ActorObject ActorViewPrefab { get; private set; }
        [field: SerializeField] public List<ActorModifierConfig> Modifiers { get; private set; }
        [field: SerializeField] public List<StatConfig> Stats { get; private set; }
    }
}
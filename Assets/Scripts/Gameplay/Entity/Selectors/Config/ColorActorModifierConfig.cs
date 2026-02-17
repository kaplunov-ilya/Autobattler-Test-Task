using System;
using System.Collections.Generic;
using Gameplay.Entity.ActorEntity;
using Gameplay.Entity.Stats;
using UnityEngine;

namespace Gameplay.Entity.Selectors.Config
{
    [CreateAssetMenu(fileName = "ColorActorModifierConfig", menuName = "Config/ColorActorModifierConfig")]
    public sealed class ColorActorModifierConfig : ActorModifierConfig
    {
        [field: SerializeField] public ColorActor Color { get; private set; }

        public override void SetModify(ActorObject actor, Dictionary<EStat, Stat> stats)
        {
            base.SetModify(actor, stats);
            actor.MeshRenderer.material = Color.Material;
        }
        
        [Serializable]
        public sealed class ColorActor
        {
            [field: SerializeField] public Material Material { get; private set; }
        }
    }
}
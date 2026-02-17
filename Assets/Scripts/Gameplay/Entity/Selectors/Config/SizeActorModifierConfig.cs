using System;
using System.Collections.Generic;
using Gameplay.Entity.ActorEntity;
using Gameplay.Entity.Stats;
using UnityEngine;

namespace Gameplay.Entity.Selectors.Config
{
    [CreateAssetMenu(fileName = "SizeActorModifierConfig", menuName = "Config/SizeActorModifierConfig")]
    public sealed class SizeActorModifierConfig : ActorModifierConfig
    {
        [field: SerializeField] public SizeActor Size { get; private set; }
        
        public override void SetModify(ActorObject actor, Dictionary<EStat, Stat> stats)
        {
            base.SetModify(actor, stats);
            actor.MeshFilter.gameObject.transform.localScale = new Vector3(1, Size.YSize, 1);
        }
        
        [Serializable]
        public sealed class SizeActor
        {
            [field: SerializeField, Range(0.5f, 2)] public float YSize { get; private set; }
        }
    }
}
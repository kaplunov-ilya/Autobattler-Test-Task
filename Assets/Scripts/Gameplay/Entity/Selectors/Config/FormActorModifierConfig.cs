using System;
using System.Collections.Generic;
using Gameplay.Entity.ActorEntity;
using Gameplay.Entity.Stats;
using UnityEngine;

namespace Gameplay.Entity.Selectors.Config
{
    [CreateAssetMenu(fileName = "FormActorModifierConfig", menuName = "Config/FormActorModifierConfig")]
    public sealed class FormActorModifierConfig : ActorModifierConfig
    {
        [field: SerializeField] public FormActor Form { get; private set; }
        
        public override void SetModify(ActorObject actor, Dictionary<EStat, Stat> stats)
        {
            base.SetModify(actor, stats);
            actor.MeshFilter.mesh = Form.FormObject;
        }
        
        [Serializable]
        public sealed class FormActor
        {
            [field: SerializeField] public Mesh FormObject { get; private set; }
        }
    }
}
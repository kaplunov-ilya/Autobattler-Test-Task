using System.Collections.Generic;
using Gameplay.Entity.ActorEntity;
using Gameplay.Entity.Stats;
using UnityEngine;

namespace Gameplay.Entity.Selectors.Config
{
    public abstract class ActorModifierConfig : ScriptableObject
    {
        [field: SerializeField] public List<StatConfig> ModifyStats { get; private set; }

        public virtual void SetModify(ActorObject actor, Dictionary<EStat, Stat> stats)
        {
            foreach (var modifyStat in ModifyStats)
            {
                stats.TryGetValue(modifyStat.StatType, out var existingStat);

                if (existingStat != null)
                {
                    existingStat.ModifyValue(modifyStat.Value);
                }
                else
                {
                    stats.Add(modifyStat.StatType, new Stat( modifyStat.StatType, modifyStat.Value));
                }
            }
        }
    }
}
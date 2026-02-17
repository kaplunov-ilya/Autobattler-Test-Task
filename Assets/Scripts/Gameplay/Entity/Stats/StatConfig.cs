using System;
using Gameplay.Entity.Stats;
using UnityEngine;

namespace Gameplay.Entity.Stats
{
    [Serializable]
    public sealed class StatConfig
    {
        [field: SerializeField] public EStat StatType { get; set; }
        [field: SerializeField] public float Value { get; set; }
    }
}
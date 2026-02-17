using UnityEngine;

namespace Gameplay.Entity.Behaviour.Defence
{
    public interface IDefenceBehaviour : IBehaviour
    {
        void TakeDamage(float damage);
    }
}
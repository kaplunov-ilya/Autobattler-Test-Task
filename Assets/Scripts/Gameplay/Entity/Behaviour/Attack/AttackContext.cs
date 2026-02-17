using Gameplay.Entity.ActorEntity;

namespace Gameplay.Entity.Behaviour.Attack
{
    public readonly struct AttackContext
    {
        public AttackContext(float damage, Actor target)
        {
            Damage = damage;
            Target = target;
        }

        public float Damage { get; }
        public Actor Target { get; }
    }
}
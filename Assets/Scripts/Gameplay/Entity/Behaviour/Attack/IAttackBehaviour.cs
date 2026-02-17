using Gameplay.Entity.Behaviour.Defence;

namespace Gameplay.Entity.Behaviour.Attack
{
    public interface IAttackBehaviour : IBehaviour
    {
        IDefenceBehaviour Target { get; }
        void SetTarget(IDefenceBehaviour target);
        void CancelAttack();
    }
}
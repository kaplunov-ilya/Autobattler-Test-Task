using Gameplay.Entity.ActorEntity;

namespace Gameplay.Entity.Behaviour
{
    public interface IBehaviour
    {
        Actor Actor { get; }

        void SetActor(Actor actor);

        void SetEnable();
        void SetDisable();
    }
}
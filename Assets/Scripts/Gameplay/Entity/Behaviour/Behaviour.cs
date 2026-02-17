using Gameplay.Entity.ActorEntity;

namespace Gameplay.Entity.Behaviour
{
    public abstract class Behaviour : IBehaviour
    {
        public Actor  Actor { get; set; }
        
        public virtual void SetActor(Actor actor)
        {
            Actor = actor;
        }

        public virtual void SetEnable() {}
        public virtual void SetDisable() {}
    }
}
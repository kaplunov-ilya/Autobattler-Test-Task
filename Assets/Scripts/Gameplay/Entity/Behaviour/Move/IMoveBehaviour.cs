using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Gameplay.Entity.Behaviour.Move
{
    public interface IMoveBehaviour : IBehaviour
    { 
        UniTask MoveTo(Transform target);
        void CancelMove();
    }
}
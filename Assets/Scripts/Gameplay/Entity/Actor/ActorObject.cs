using UnityEngine;

namespace Gameplay.Entity.ActorEntity
{
    public sealed class ActorObject : MonoBehaviour
    {
        [field: SerializeField] public Transform PositionPoint { get; private set; }
        [field: SerializeField] public MeshFilter MeshFilter { get; private set; }
        [field: SerializeField] public MeshRenderer MeshRenderer { get; private set; }
    }
}
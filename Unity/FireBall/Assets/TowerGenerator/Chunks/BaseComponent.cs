
using UnityEngine;

namespace TowerGenerator
{
    interface IComponent
    {
        bool IsValid();
        GroupsController GroupsController { get; set; }
        ChunkBase Chunk { get; set; }
    }

    public abstract class BaseComponent : MonoBehaviour, IComponent
    {
        public abstract bool IsValid();
        public GroupsController GroupsController { get; set; }
        public ChunkBase Chunk { get; set; }
    }
}
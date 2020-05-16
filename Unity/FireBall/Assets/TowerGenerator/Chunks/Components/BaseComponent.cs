
using UnityEngine;

namespace TowerGenerator
{
    interface IComponent
    {
        bool IsValid();
    }

    public abstract class BaseComponent : MonoBehaviour, IComponent
    {
        public virtual bool IsValid()
        {
            return GroupsController != null && Chunk != null;
        }

        [HideInInspector]
        public RootGroupsController GroupsController;

        [HideInInspector]
        public ChunkBase Chunk;

        [HideInInspector]
        public Group OwnerGroup;
    }
}
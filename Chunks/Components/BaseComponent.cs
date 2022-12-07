
using NaughtyAttributes;
using UnityEngine;

namespace TowerGenerator
{
    interface IComponent
    {
        bool IsValid();
    }

    public abstract class BaseComponent : MonoBehaviour, IComponent
    {
        public virtual void Initialize()
        {
        }

        public virtual bool IsValid()
        {
            return ChunkController != null;
        }

        [ReadOnly]
        public ChunkControllerBase ChunkController;
    }
}

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
            return ChunkController != null;
        }

        [HideInInspector]
        public ChunkControllerBase ChunkController;


        [HideInInspector]
        public Group InfluenceGroup;
    }
}
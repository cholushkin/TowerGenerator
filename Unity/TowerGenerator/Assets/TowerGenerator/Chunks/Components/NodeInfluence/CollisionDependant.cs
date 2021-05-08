using UnityEngine;

namespace TowerGenerator
{
    // todo: add scale for collision geometry
    public class CollisionDependant : BaseComponent
    {
        public enum CollisionCheckMode
        {
            MeshBased,
            AABBBased
        }

        public CollisionCheckMode CollisionCheck;
    }
}

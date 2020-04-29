using UnityEngine;

namespace TowerGenerator
{
    // todo: add scale for collision geometry
    public class CollisionDependent : MonoBehaviour, INodeValidation
    {
        public enum CollisionCheckMode
        {
            MeshBased,
            AABBBased
        }

        

        public CollisionCheckMode CollisionCheck;

        public bool IsValid()
        {
            throw new System.NotImplementedException();
        }

        public void Fix()
        {
            throw new System.NotImplementedException();
        }
    }
}

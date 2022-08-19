using UnityEngine.Assertions;

namespace TowerGenerator
{
    // Dominant fragment always suppress Submissive fragment. If other collision object is not a CollisionDependent it always has higher priority.
    // If both collision happened in current chunk and both fragments have same Relation than more priority will be given to the fragment that is already activated
    // If collision fragments belong to different chunk and they have same Relation value than more priority will be given to the fragment of a chunk that is already spawned before (exists in hierarchy tree)
    public class CollisionDependent : BaseComponent
    {
        public enum FragmentRelation
        {
            Dominant,
            Submissive
        }

        public FragmentRelation FragmentDomination;

        public void ProcessCollision()
        {
            Assert.IsTrue(gameObject.activeSelf && gameObject.activeInHierarchy);
            ChunkController.SetNodeActiveState(transform, false);
        }
    }
}

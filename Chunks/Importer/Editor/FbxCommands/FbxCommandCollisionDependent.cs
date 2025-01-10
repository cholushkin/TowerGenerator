#if UNITY_EDITOR
using TowerGenerator.ChunkImporter;
using UnityEngine;
using UnityEngine.Assertions;

namespace TowerGenerator.FbxCommands
{
    // Adds CollisionDependent component to the node. With CollisionDependent attached node will disable itself if collision happens or disable other object.
    // Dominant fragment always suppress Submissive fragment. If other collision object is not a CollisionDependent it always has higher priority.
    // If both collision happened in current chunk and both fragments have same Relation than more priority will be given to the fragment that is already activated
    // If collision fragments belong to different chunk and they have same Relation value than more priority will be given to the fragment of a chunk that is already spawned before (exists in hierarchy tree)
    // Examples:
    // CollisionDependent( ) // by default FragmentDomination will be Submissive
    // CollisionDependent(Dominant ) // make fragment disable other collided segments
    public class FbxCommandCollisionDependent: FbxCommandBase
    {
        public CollisionDependent.FragmentRelation FragmentDomination;

        public FbxCommandCollisionDependent(string fbxCommandName, int executionPriority) : base(fbxCommandName, executionPriority)
        {
        }

        public override void ParseParameters(string parameters, GameObject gameObject)
        {
            Assert.IsNotNull(gameObject, $"There must be an object for the command '{GetFbxCommandName()}'");
            
            // set default values first
            FragmentDomination = CollisionDependent.FragmentRelation.Submissive;
            
            if (string.IsNullOrWhiteSpace(parameters))
                return;
            FragmentDomination = ConvertEnum<CollisionDependent.FragmentRelation>(parameters);
        }

        public override void Execute(GameObject gameObject, ChunkImportState importState)
        {
            Assert.IsNotNull(gameObject, $"There must be an object for the command '{GetFbxCommandName()}'");
            Assert.IsNotNull(importState);
            Assert.IsNull(gameObject.GetComponent<CollisionDependent>());
            var comp = gameObject.AddComponent<CollisionDependent>();
            comp.FragmentDomination = FragmentDomination;
            importState.Inc("CollisionDependentAmount");
        }
    }
}
#endif
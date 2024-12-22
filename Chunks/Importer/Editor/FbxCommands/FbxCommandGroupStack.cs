#if UNITY_EDITOR
using GameLib.Alg;
using TowerGenerator.ChunkImporter;
using UnityEngine;
using UnityEngine.Assertions;

namespace TowerGenerator.FbxCommands
{
    // Adds GroupStack component to the node.
    // Nested objects are treated as stack levels which could be turned on sequentially 
    public class FbxCommandGroupStack : FbxCommandBase
    {
        public FbxCommandGroupStack(string fbxCommandName, int executionPriority) : base(fbxCommandName, executionPriority)
        {
        }

        public override void ParseParameters(string parameters, GameObject gameObject)
        {
            Assert.IsNotNull(gameObject, $"There must be an object for the command '{GetFbxCommandName()}'");
            Assert.IsTrue(string.IsNullOrWhiteSpace(parameters));
        }

        public override void Execute(GameObject gameObject, ChunkImportState importState)
        {
            Assert.IsNotNull(gameObject, $"There must be an object for the command '{GetFbxCommandName()}'");
            Assert.IsNotNull(importState);
            
            
            Assert.IsNull(gameObject.GetComponent<GroupStack>());
            var groupStack = gameObject.AddComponent<GroupStack>();
            Assert.IsTrue(groupStack.IsValid(), $"{gameObject.transform.GetDebugName()} {GetFbxCommandName()} is invalid");
            importState.GroupStackAmount++;
        }
    }
}
#endif
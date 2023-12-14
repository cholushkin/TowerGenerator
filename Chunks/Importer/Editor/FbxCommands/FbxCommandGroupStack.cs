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
        // Parameters

        // -1 no restriction ( could be zero elements enabled)
        // 0 and greater: index of stack level which is enabled by default ( for 0: 0001, for 1: 0011, for 2: 0111 )
        public int MinIndexSelected; 


        public FbxCommandGroupStack(string fbxCommandName, int executionPriority) : base(fbxCommandName, executionPriority)
        {
        }

        public override void ParseParameters(string parameters, GameObject gameObject)
        {
            Assert.IsNotNull(gameObject, $"There must be an object for the command '{GetFbxCommandName()}'");

            // Set default values for parameters
            MinIndexSelected = -1;

            if (string.IsNullOrWhiteSpace(parameters))
                return; // Keep default values if there is no parameters

            var actualParams = parameters.Split(',');
            Assert.IsTrue(actualParams.Length == 1, "The number of arguments should be 0, 1");

            MinIndexSelected = ConvertInt(parameters);
        }

        public override void Execute(GameObject gameObject, ChunkCooker.ChunkImportState importState)
        {
            Assert.IsNotNull(gameObject, $"There must be an object for the command '{GetFbxCommandName()}'");
            Assert.IsNotNull(importState);
            Assert.IsNull(gameObject.GetComponent<GroupStack>(), "no GroupStack should be attached before");
            var groupStack = gameObject.AddComponent<GroupStack>();
            Assert.IsTrue(groupStack.IsValid(), $"{gameObject.transform.GetDebugName()} {GetFbxCommandName()} is invalid");
            importState.GroupStackAmount++;
        }
    }
}

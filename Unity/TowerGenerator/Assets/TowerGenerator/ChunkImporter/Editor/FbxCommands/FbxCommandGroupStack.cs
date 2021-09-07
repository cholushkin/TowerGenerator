using Assets.Plugins.Alg;
using TowerGenerator.ChunkImporter;
using UnityEngine;
using UnityEngine.Assertions;

namespace TowerGenerator.FbxCommands
{
    // Adds GroupStack component to the node.
    // Nested objects are treated as stack levels which could be turned on sequentially 
    public class FbxCommandGroupStack : FbxCommandBase
    {
        public override string GetFbxCommandName()
        {
            return "GroupStack";
        }

        public override void ParseParameters(string parameters, GameObject gameObject)
        {
            Assert.IsNotNull(gameObject, $"There must be an object for the command '{GetFbxCommandName()}'");
            Assert.IsTrue(string.IsNullOrWhiteSpace(parameters), "There should not be parameters for the command 'AddGroupStack'");
        }

        public override void Execute(GameObject gameObject, ChunkCooker.ChunkImportInformation importInformation)
        {
            Assert.IsNotNull(gameObject, $"There must be an object for the command '{GetFbxCommandName()}'");
            Assert.IsNotNull(importInformation);
            Assert.IsNull(gameObject.GetComponent<GroupStack>(), "no GroupStack should be attached before");
            var groupStack = gameObject.AddComponent<GroupStack>();
            Assert.IsTrue(groupStack.IsValid(), $"{gameObject.transform.GetDebugName()} {GetFbxCommandName()} is invalid");
            importInformation.GroupStackAmount++;
        }
    }
}

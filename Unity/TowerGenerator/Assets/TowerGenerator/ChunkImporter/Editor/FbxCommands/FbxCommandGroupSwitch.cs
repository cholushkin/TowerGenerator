using Assets.Plugins.Alg;
using TowerGenerator.ChunkImporter;
using UnityEngine;
using UnityEngine.Assertions;

namespace TowerGenerator.FbxCommands
{
    // Adds GroupSwitch component to the node.GroupSwitch component allows the node to randomly switch between one of its children.
    // No parameters
    public class FbxCommandGroupSwitch : FbxCommandBase
    {
        public FbxCommandGroupSwitch(string fbxCommandName) : base(fbxCommandName)
        {
        }

        public override void ParseParameters(string parameters, GameObject gameObject)
        {
            Assert.IsNotNull(gameObject, $"There must be an object for the command '{GetFbxCommandName()}'");
            Assert.IsTrue(string.IsNullOrWhiteSpace(parameters), $"There should not be any parameters for the command '{GetFbxCommandName()}' but you have: '{parameters}' ");
        }

        public override void Execute(GameObject gameObject, ChunkCooker.ChunkImportInformation importInformation)
        {
            Assert.IsNotNull(gameObject, $"There must be an object for the command '{GetFbxCommandName()}'");
            Assert.IsNotNull(importInformation);
            Assert.IsNull(gameObject.GetComponent<GroupSwitch>(), "no objects should be attached before");
            var groupSwitch = gameObject.AddComponent<GroupSwitch>();
            Assert.IsTrue(groupSwitch.IsValid(), $"{gameObject.transform.GetDebugName()} {GetFbxCommandName()} is invalid");
            importInformation.GroupSwitchAmount++;
        }
    }
}
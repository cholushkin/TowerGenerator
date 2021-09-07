using TowerGenerator.ChunkImporter;
using UnityEngine;
using UnityEngine.Assertions;

namespace TowerGenerator.FbxCommands
{
    // By default all objects in fbx which have geometry will attach collider on import.
    // By using this command you can cancel addition of the collider.

    public class FbxCommandIgnoreAddCollider : FbxCommandBase
    {
        public override string GetFbxCommandName()
        {
            return "IgnoreAddCollider";
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
            Assert.IsNull(gameObject.GetComponent<IgnoreAddCollider>());
            var ignoreAddCollider = gameObject.AddComponent<IgnoreAddCollider>();
            importInformation.IgnoreAddColliderAmount++;
        }
    }
}
using TowerGenerator.ChunkImporter;
using UnityEngine;
using UnityEngine.Assertions;

namespace TowerGenerator.FbxCommands
{
    public class FbxCommandDimensionsIgnorant: FbxCommandBase
    {
        public FbxCommandDimensionsIgnorant(string fbxCommandName) : base(fbxCommandName)
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
            Assert.IsNull(gameObject.GetComponent<DimensionsIgnorant>());
            gameObject.AddComponent<DimensionsIgnorant>();
            importInformation.DimensionsIgnorantAmount++;
        }
    }
}

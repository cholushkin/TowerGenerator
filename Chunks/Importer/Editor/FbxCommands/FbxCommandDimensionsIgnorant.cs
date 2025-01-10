#if UNITY_EDITOR
using TowerGenerator.ChunkImporter;
using UnityEngine;
using UnityEngine.Assertions;

namespace TowerGenerator.FbxCommands
{
    public class FbxCommandDimensionsIgnorant: FbxCommandBase
    {
        public FbxCommandDimensionsIgnorant(string fbxCommandName, int executionPriority) : base(fbxCommandName, executionPriority)
        {
        }

        public override void ParseParameters(string parameters, GameObject gameObject)
        {
            Assert.IsNotNull(gameObject, $"There must be an object for the command '{GetFbxCommandName()}'");
            Assert.IsTrue(string.IsNullOrWhiteSpace(parameters), $"There should not be any parameters for the command '{GetFbxCommandName()}' but you have: '{parameters}' ");
        }

        public override void Execute(GameObject gameObject, ChunkImportState importState)
        {
            Assert.IsNotNull(gameObject, $"There must be an object for the command '{GetFbxCommandName()}'");
            Assert.IsNotNull(importState);
            Assert.IsNull(gameObject.GetComponent<DimensionsIgnorant>());
            gameObject.AddComponent<DimensionsIgnorant>();
            importState.Inc("DimensionsIgnorantAmount");
        }
    }
}
#endif
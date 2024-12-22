#if UNITY_EDITOR
using TowerGenerator.ChunkImporter;
using UnityEngine;
using UnityEngine.Assertions;

namespace TowerGenerator.FbxCommands
{
    // Chunk generation helps to filter set of all available chunks by the specific group id (generation).
    // Later on when new generation of chunks added to the build the old consistency of seed based random result will not be broken. 
    // Because the process of generation always works on a specified range of generation IDs currently available in a build.
    // However if the result of random generation has to include all available chunks then generation filter should be disabled.
    // Note: There are 2 generation words in one context, this could be confusing. Generation as a noun is an group id of the chunks ( FbxCommandChunkGeneration )
    // Generation as a verb is a process of procedural creation of a tower.
    // Example:
    // Generation(2)

    public class FbxCommandChunkGeneration : FbxCommandBase
    {
        public uint Generation;

        public FbxCommandChunkGeneration(string fbxCommandName, int executionPriority) : base(fbxCommandName, executionPriority)
        {
        }

        public override void ParseParameters(string parameters, GameObject gameObject)
        {
            Assert.IsNotNull(gameObject, $"There must be an object for the command '{GetFbxCommandName()}'");
            Assert.IsFalse(string.IsNullOrEmpty(parameters));
            Generation = ConvertUInt(parameters);
        }

        public override void Execute(GameObject gameObject, ChunkImportState importState)
        {
            Assert.IsNotNull(gameObject, $"There must be an object for the command '{GetFbxCommandName()}'");
            Assert.IsNotNull(importState);
            var chunkController = gameObject.GetComponent<ChunkControllerBase>();
            Assert.IsNotNull(chunkController);

            importState.Generation = Generation;
        }
    }
}
#endif
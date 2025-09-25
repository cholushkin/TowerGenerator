using TowerGenerator.ChunkImporter;
using UnityEngine;
using UnityEngine.Assertions;

namespace TowerGenerator.FbxCommands
{
    // Override default Meta type for the chunk
    // Examples:
    // Meta(MetaCastleChunk)

    public class FbxCommandMeta : FbxCommandBase
    {
        public string MetaType;
        public FbxCommandMeta(string fbxCommandName, int executionPriority) : base(fbxCommandName, executionPriority)
        {
            
        }

        public override void ParseParameters(string parameters, GameObject gameObject)
        {
            Assert.IsNotNull(gameObject, $"There must be an object for the command '{GetFbxCommandName()}'");
            if (string.IsNullOrWhiteSpace(parameters))
                return;
            Assert.IsFalse(string.IsNullOrEmpty(parameters));
            MetaType = parameters.Trim();
        }

        public override void Execute(GameObject gameObject, ChunkImportState importState)
        {
            importState.MetaType = MetaType;
        }
    }
}
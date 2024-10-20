#if UNITY_EDITOR
using TowerGenerator.ChunkImporter;
using UnityEngine;
using UnityEngine.Assertions;

namespace TowerGenerator.FbxCommands
{
    // Attaches Suppression component to the node.
    // Make this node suppress other nodes on activation using suppression labels. Suppressed nodes are specified by SuppressedBy.
    // If there is no any label name passed then Suppression node will suppress all SuppressedBy nodes
    // Example:
    // Suppression(Tube,Pipe)
    // Suppression() // suppress all SuppressedBy nodes without parameters
    public class FbxCommandSuppression: FbxCommandBase
    {
        public string[] SuppressionLabels;
        public FbxCommandSuppression(string fbxCommandName, int executionPriority) : base(fbxCommandName, executionPriority)
        {
        }

        public override void ParseParameters(string parameters, GameObject gameObject)
        {
            Assert.IsNotNull(gameObject, $"There must be an object for the command '{GetFbxCommandName()}'");
            if(string.IsNullOrWhiteSpace(parameters))
                return;
            SuppressionLabels = parameters.Split(',');
        }

        public override void Execute(GameObject gameObject, ChunkCooker.ChunkImportState importState)
        {
            Assert.IsNotNull(gameObject, $"There must be an object for the command '{GetFbxCommandName()}'");
            Assert.IsNotNull(importState);
            Assert.IsNull(gameObject.GetComponent<Suppression>());


            var comp = gameObject.AddComponent<Suppression>();
            comp.SuppressionLabels = SuppressionLabels;
            importState.SuppressionAmount++;
        }
    }
}
#endif
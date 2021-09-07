using TowerGenerator.ChunkImporter;
using UnityEngine;
using UnityEngine.Assertions;

namespace TowerGenerator.FbxCommands
{
    // Make this node being suppressed by Suppression nodes with corresponding labels.
    // SuppressionLabels is a List of suppression labels by which this node will be suppressed.
    // If the list is empty then get suppressed by all Suppression nodes without parameters
    // Examples:
    // SuppressedBy( Tube,Pipe)
    // SuppressedBy() // be default get suppressed by all Suppression nodes without labels
    public class FbxCommandSuppressedBy : FbxCommandBase
    {
        public string[] SuppressionLabels;
        public override string GetFbxCommandName()
        {
            return "SuppressedBy";
        }

        public override void ParseParameters(string parameters, GameObject gameObject)
        {
            Assert.IsNotNull(gameObject, $"There must be an object for the command '{GetFbxCommandName()}'");
            if (string.IsNullOrWhiteSpace(parameters))
                return;
            SuppressionLabels = parameters.Split(',');
        }

        public override void Execute(GameObject gameObject, ChunkCooker.ChunkImportInformation importInformation)
        {
            Assert.IsNotNull(gameObject, $"There must be an object for the command '{GetFbxCommandName()}'");
            Assert.IsNotNull(importInformation);
            Assert.IsNull(gameObject.GetComponent<SuppressedBy>());


            var comp = gameObject.AddComponent<SuppressedBy>();
            comp.SuppressionLabels = SuppressionLabels;
            importInformation.SuppressedByAmount++;
        }
    }
}
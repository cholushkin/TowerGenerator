using TowerGenerator.ChunkImporter;
using UnityEngine;
using UnityEngine.Assertions;

namespace TowerGenerator.FbxCommands
{
    // Make this node induct other nodes on activation using induction labels.
    // Induced nodes are specified by InducedBy.
    // string[] InductionLabels is a list of induced labels which this node will activate on its own activation.
    // If the list is empty then all InducedBy nodes without labels will be activated
    // Examples:
    // Induction(Tube, Pipe)
    // Induction() // induces all InducedBy nodes without labels"

    public class FbxCommandInduction: FbxCommandBase
    {
        public string[] InductionLabels;
        public FbxCommandInduction(string fbxCommandName, int executionPriority) : base(fbxCommandName, executionPriority)
        {
        }

        public override void ParseParameters(string parameters, GameObject gameObject)
        {
            Assert.IsNotNull(gameObject, $"There must be an object for the command '{GetFbxCommandName()}'");
            if (string.IsNullOrWhiteSpace(parameters))
                return;
            InductionLabels = parameters.Split(',');
        }

        public override void Execute(GameObject gameObject, ChunkCooker.ChunkImportState importState)
        {
            Assert.IsNotNull(gameObject, $"There must be an object for the command '{GetFbxCommandName()}'");
            Assert.IsNotNull(importState);
            Assert.IsNull(gameObject.GetComponent<Induction>());


            var comp = gameObject.AddComponent<Induction>();
            comp.InductionLabels = InductionLabels;
            importState.InductionAmount++;
        }
    }
}
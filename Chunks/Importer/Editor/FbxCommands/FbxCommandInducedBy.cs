#if UNITY_EDITOR
using TowerGenerator.ChunkImporter;
using UnityEngine;
using UnityEngine.Assertions;

namespace TowerGenerator.FbxCommands
{
    // Make this node being induced by Induction nodes with corresponding labels.
    // string[] InductionLabels is a list of induction labels by which this node will be induced.
    // If the list is empty then get induced by all Induction nodes without parameters
    // Examples:
    // InducedBy(Tube,Pipe,LargePillar)
    // InducedBy(Pipe)
    // InducedBy() // induced by every Induction node without parameters
    
    public class FbxCommandInducedBy : FbxCommandBase
    {
        public string[] InductionLabels;
        public FbxCommandInducedBy(string fbxCommandName, int executionPriority) : base(fbxCommandName, executionPriority)
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
            Assert.IsNull(gameObject.GetComponent<InducedBy>());


            var comp = gameObject.AddComponent<InducedBy>();
            comp.InductionLabels = InductionLabels;
            importState.InducedByAmount++;
        }
    }
}
#endif
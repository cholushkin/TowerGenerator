#if UNITY_EDITOR
using GameLib.Alg;  
using TowerGenerator.ChunkImporter;
using UnityEngine;
using UnityEngine.Assertions;

namespace TowerGenerator.FbxCommands
{
    // Adds GroupSet component to the node which makes the node to be able to randomly enable some subset of its children. 
    // Parameters:
    // int MinObjectsSelected - The minimum amount of items this group could enable. Default value is 0.
    // int MaxObjectsSelected - The maximum amount of items this group could enable. Default value is a number of the children of the node
    // Example:
    // GroupSet( )
    // GroupSet( 0, 2)
    // GroupSet( 1 ) // 1 child selected minimum. Maximum is MaxChildrenNumber
    public class FbxCommandGroupSet : FbxCommandBase
    {
        // parameters
        public int MinObjectsSelected;
        public int MaxObjectsSelected;

        public FbxCommandGroupSet(string fbxCommandName, int executionPriority) : base(fbxCommandName, executionPriority)
        {
        }

        public override void ParseParameters(string parametersString, GameObject gameObject)
        {
            Assert.IsNotNull(gameObject, $"There must be an object for the command '{GetFbxCommandName()}'");

            // Set default values for parameters
            MinObjectsSelected = 0;
            MaxObjectsSelected = gameObject.transform.childCount;

            if (string.IsNullOrWhiteSpace(parametersString))
                return; // Keep default values if there is no parameters

            var actualParams = parametersString.Split(',');
            Assert.IsTrue(actualParams.Length == 1 || actualParams.Length == 2, "The number of arguments should be 0, 1 or 2");

            if (actualParams.Length >= 1)
                MinObjectsSelected = ConvertInt(actualParams[0]);
            if (actualParams.Length >= 2)
                MaxObjectsSelected = ConvertInt(actualParams[1]);
        }

        public override void Execute(GameObject gameObject, ChunkImportState importState)
        {
            Assert.IsNotNull(gameObject, $"There must be an object for the command '{GetFbxCommandName()}'");
            Assert.IsNotNull(importState);

            Assert.IsNull(gameObject.GetComponent<GroupSet>());
            var groupSet = gameObject.AddComponent<GroupSet>();
            groupSet.MaxObjectsSelected = MaxObjectsSelected;
            groupSet.MinObjectsSelected = MinObjectsSelected;
            Assert.IsTrue(groupSet.IsValid(), $"{gameObject.transform.GetDebugName()} {GetFbxCommandName()} is invalid");
            importState.GroupSetAmount++;
        }
    }
}
#endif
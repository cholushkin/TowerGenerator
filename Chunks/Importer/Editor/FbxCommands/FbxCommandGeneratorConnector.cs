using TowerGenerator.ChunkImporter;
using TowerGenerator.FbxCommands;
using UnityEngine;
using UnityEngine.Assertions;

public class FbxCommandGeneratorConnector : FbxCommandBase
{
    // Example:
    // GeneratorConnector(Creature or Decoration)
    public string[] ConnectExpressions;

    public FbxCommandGeneratorConnector(string fbxCommandName) : base(fbxCommandName)
    {
    }

    public override void ParseParameters(string parameters, GameObject gameObject)
    {
        Assert.IsNotNull(gameObject, $"There must be an object for the command '{GetFbxCommandName()}'");
        Assert.IsFalse(string.IsNullOrWhiteSpace(parameters));
        ConnectExpressions = parameters.Split(',');
    }

    public override void Execute(GameObject gameObject, ChunkCooker.ChunkImportState importState)
    {
        Assert.IsNotNull(gameObject, $"There must be an object for the command '{GetFbxCommandName()}'");
        Assert.IsNotNull(importState);
        //Assert.IsNull(gameObject.GetComponent<GeneratorConnector>());

        //var connector = gameObject.AddComponent<GeneratorConnector>();
        //connector.ConnectExpressions = ConnectExpressions;
        //importInformation.GeneratorConnectorAmount++;
    }
}
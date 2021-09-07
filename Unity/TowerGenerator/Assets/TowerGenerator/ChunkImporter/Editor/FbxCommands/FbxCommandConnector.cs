using TowerGenerator;
using TowerGenerator.ChunkImporter;
using TowerGenerator.FbxCommands;
using UnityEngine;
using UnityEngine.Assertions;

public class FbxCommandConnector : FbxCommandBase
{
    public string[] SupportedTags;
    public override string GetFbxCommandName()
    {
        return "Connector";
    }

    public override void ParseParameters(string parameters, GameObject gameObject)
    {
        Assert.IsNotNull(gameObject, $"There must be an object for the command '{GetFbxCommandName()}'");
        Assert.IsFalse(string.IsNullOrWhiteSpace(parameters));
        SupportedTags = parameters.Split(',');
    }

    public override void Execute(GameObject gameObject, ChunkCooker.ChunkImportInformation importInformation)
    {
        Assert.IsNotNull(gameObject, $"There must be an object for the command '{GetFbxCommandName()}'");
        Assert.IsNotNull(importInformation);
        Assert.IsNull(gameObject.GetComponent<Connector>());

        var connector = gameObject.AddComponent<Connector>();
        connector.SupportedTags = SupportedTags;
        importInformation.ConnectorAmount++;
    }
}
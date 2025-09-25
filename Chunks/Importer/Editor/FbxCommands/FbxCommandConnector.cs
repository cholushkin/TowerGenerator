using TowerGenerator;
using TowerGenerator.ChunkImporter;
using TowerGenerator.FbxCommands;
using UnityEngine;
using UnityEngine.Assertions;

public class FbxCommandConnector : FbxCommandBase
{
    // Example:
    // Connector(In|Out, ChunkStandard, ArchitectureNeutral, BiomeNeutral)
    public Connector.ConnectorType ConnectorType;
    public string[] ConnectExpressions;
    public FbxCommandConnector(string fbxCommandName, int executionPriority) : base(fbxCommandName, executionPriority)
    {
    }

    public override void ParseParameters(string parameters, GameObject gameObject)
    {
        Assert.IsNotNull(gameObject, $"There must be an object for the command '{GetFbxCommandName()}'");
        Assert.IsFalse(string.IsNullOrWhiteSpace(parameters));
        var paramsSeparated = parameters.Split(',');
        ConnectorType = ConvertEnum<Connector.ConnectorType>(paramsSeparated[0]);
        if(paramsSeparated.Length > 1)
            paramsSeparated.CopyTo(ConnectExpressions, 1);

    }

    public override void Execute(GameObject gameObject, ChunkImportState importState)
    {
        Assert.IsNotNull(gameObject, $"There must be an object for the command '{GetFbxCommandName()}'");
        Assert.IsNotNull(importState);
        Assert.IsNull(gameObject.GetComponent<Connector>());

        var connector = gameObject.AddComponent<Connector>();
        connector.ConnectorMode = ConnectorType;
        connector.ConnectExpressions = ConnectExpressions;
        importState.Inc("ConnectorAmount");
    }
}
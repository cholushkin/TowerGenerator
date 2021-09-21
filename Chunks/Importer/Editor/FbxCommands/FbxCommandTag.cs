using TowerGenerator.ChunkImporter;
using UnityEngine;
using UnityEngine.Assertions;

namespace TowerGenerator.FbxCommands
{
    // Tag value for specified node inside that chunk(including chunk itself )
    // Parameters:
    // string TagName - name of the tag in the CamelCase
    // float TagValue - value of the tag from 0 to 1
    // Examples:
    // Tag(Setting.Tech, 0.5) // 50% of tech setting for this chunk
    // Tag(TreasureChest) // could be applied to the connector to make it spawn treasure chests only (decoration)
    public class FbxCommandTag : FbxCommandBase
    {
        public string TagName;
        public float TagValue;
        public FbxCommandTag(string fbxCommandName) : base(fbxCommandName)
        {
        }

        public override void ParseParameters(string parameters, GameObject gameObject)
        {
            Assert.IsNotNull(gameObject, $"There must be an object for the command '{GetFbxCommandName()}'");
            
            // set default values for parameters first
            TagName = "";
            TagValue = 1f;

            Assert.IsTrue(!string.IsNullOrWhiteSpace(parameters));

            var actualParams = parameters.Split(',');
            Assert.IsTrue(actualParams.Length == 1 || actualParams.Length == 2);

            if (actualParams.Length >= 1)
                TagName = actualParams[0];
            if (actualParams.Length >= 2)
                TagValue = ConvertFloat01(actualParams[1]);
        }

        public override void Execute(GameObject gameObject, ChunkCooker.ChunkImportInformation importInformation)
        {
            Assert.IsNotNull(gameObject, $"There must be an object for the command '{GetFbxCommandName()}'");
            Assert.IsNotNull(importInformation);

            var tagHolder = gameObject.GetComponent<TagHolder>();
            if (tagHolder == null)
                tagHolder = gameObject.AddComponent<TagHolder>();

            tagHolder.TagSet.SetTag(TagName, TagValue);
        }
    }
}

using GameLib;
using TowerGenerator.ChunkImporter;
using UnityEngine;
using UnityEngine.Assertions;

namespace TowerGenerator.FbxCommands
{
    // Adds ChunkController to the node. ChunkController mostly controls randomization of the chunk.
    // Examples:
    // ChunkController(BasicChunkController, ChunkBasement)
    // ChunkController() // by default parameters are: BasicChunkController, null
    // ChunkController(BasicChunkController, ChunkTopEar, ChunkPeak, BiomeJungle, ArchitectureCubism)
    public class FbxCommandChunkController : FbxCommandBase
    {
        public ChunkControllerBase.ChunkController ChunkControllerType;
        public TagSet ChunkTagSet;
        public FbxCommandChunkController(string fbxCommandName) : base(fbxCommandName)
        {
        }

        public override void ParseParameters(string parameters, GameObject gameObject)
        {
            Assert.IsNotNull(gameObject, $"There must be an object for the command '{GetFbxCommandName()}'");

            // set defaults parameters first
            ChunkControllerType = ChunkControllerBase.ChunkController.BasicChunkController;
            ChunkTagSet = new TagSet();

            if (string.IsNullOrWhiteSpace(parameters))
                return; // keep default values if there is no parameters

            var actualParams = parameters.Split(',');
            Assert.IsTrue(actualParams.Length == 1 || actualParams.Length == 2);
            if (actualParams.Length >= 1) // we have first parameter
            {
                ChunkControllerType = ConvertEnum<ChunkControllerBase.ChunkController>(actualParams[0]);
                Assert.IsTrue(ChunkControllerType != ChunkControllerBase.ChunkController.Undefined, $"Can't parse 'ChunkTopologyType' parameter from value '{actualParams[0]}'");
                Assert.IsTrue(Numbers.CountBits((uint)ChunkControllerType) == 1, "More than one flag set");
            }

            if (actualParams.Length >= 2) // we have second parameter
            {
                for (int i = 1; i < actualParams.Length; ++i)
                {
                    var tagName = actualParams[i];
                    ChunkTagSet.SetTag(tagName, 1f);
                }
            }
        }

        public override void Execute(GameObject gameObject, ChunkCooker.ChunkImportInformation importInformation)
        {
            Assert.IsNotNull(gameObject, $"There must be an object for the command '{GetFbxCommandName()}'");
            Assert.IsNotNull(importInformation);

            ChunkControllerBase chunkController = null;

            Assert.IsNull(gameObject.GetComponent<ChunkControllerBase>(), "no ChunkController should be attached before");

            if (ChunkControllerType == ChunkControllerBase.ChunkController.BasicChunkController)
                chunkController = gameObject.AddComponent<ChunkControllerBase>();

            Assert.IsNotNull(chunkController, "chunk controller is null");

            importInformation.ChunkControllerType = ChunkControllerType;
            importInformation.ChunkTagSet = ChunkTagSet;
        }
    }
}
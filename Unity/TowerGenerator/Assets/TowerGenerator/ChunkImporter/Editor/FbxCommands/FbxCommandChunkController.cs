using GameLib;
using TowerGenerator.ChunkImporter;
using UnityEngine;
using UnityEngine.Assertions;

namespace TowerGenerator.FbxCommands
{
    // Adds ChunkController to the node. ChunkController mostly controls randomization of the chunk.
    // Examples:
    // ChunkController(ChunkFoundation, GrowingChunkController)
    // ChunkController() // by default parameters are: ChunkStd, BasicChunkController
    // ChunkController(ChunkFoundation) // first parameter is ChunkFoundation, second by default is BasicChunkController
    // ChunkController(ChunkTopEar|ChunkPeak)
    public class FbxCommandChunkController : FbxCommandBase
    {
        public TopologyType ChunkTopologyType;
        public ChunkConformationType ChunkConformationType;
        public override string GetFbxCommandName()
        {
            return "ChunkController";
        }

        public override void ParseParameters(string parameters, GameObject gameObject)
        {
            Assert.IsNotNull(gameObject, $"There must be an object for the command '{GetFbxCommandName()}'");

            // set defaults parameters first
            ChunkTopologyType = TopologyType.ChunkStd;
            ChunkConformationType = ChunkConformationType.BasicChunkController;

            if (string.IsNullOrWhiteSpace(parameters))
                return; // keep default values if there is no parameters

            var actualParams = parameters.Split(',');
            Assert.IsTrue(actualParams.Length is 1 or 2);
            if (actualParams.Length >= 1) // we have first parameter
            {
                ChunkTopologyType = ConvertEnum<TopologyType>(actualParams[0]);
                Assert.IsTrue(ChunkTopologyType != TopologyType.Undefined, $"Can't parse 'ChunkTopologyType' parameter from value '{actualParams[0]}'");
                Assert.IsTrue(Numbers.CountBits((uint)ChunkTopologyType) == 1, "More than one flag set");
            }

            if (actualParams.Length >= 2) // we have second parameter
            {
                ChunkConformationType = ConvertEnum<ChunkConformationType>(actualParams[1]);
                Assert.IsTrue(ChunkConformationType != ChunkConformationType.Undefined, $"Can't parse 'ChunkConformationType' parameter from value '{actualParams[1]}'");
            }
        }

        public override void Execute(GameObject gameObject, ChunkCooker.ChunkImportInformation importInformation)
        {
            Assert.IsNotNull(gameObject, $"There must be an object for the command '{GetFbxCommandName()}'");
            Assert.IsNotNull(importInformation);

            ChunkControllerBase chunkController = null;

            if (ChunkConformationType == ChunkConformationType.BasicChunkController)
                chunkController = gameObject.AddComponent<ChunkControllerCombinatorial>();
            //else if (ChunkConformationType == ChunkConformationType.DimensionsBased)
            //    chunkController = gameObject.AddComponent<ChunkControllerDimensionsBased>();
            //else if (ChunkConformationType == ChunkConformationType.DynamicGrow)
            //    chunkController = gameObject.AddComponent<ChunkControllerDynamicGrow>();
            //else if (ChunkConformationType == ChunkConformationType.Stretchable)
            //    chunkController = gameObject.AddComponent<ChunkControllerStretchable>();

            Assert.IsNotNull(chunkController, "chunk controller is null");
            Assert.IsNull(gameObject.GetComponent<ChunkControllerBase>(), "no ChunkController should be attached before");


            chunkController.TopologyType = ChunkTopologyType;
            chunkController.ConformationType = ChunkConformationType;

            importInformation.ChunkControllerAmount++;
            if (!importInformation.ConformationType.ContainsKey(ChunkConformationType))
                importInformation.ConformationType.Add(ChunkConformationType, 0);
            importInformation.ConformationType[ChunkConformationType]++;
        }
    }
}
using System.Collections.Generic;
using Assets.Plugins.Alg;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;

namespace TowerGenerator.ChunkImporter
{
    public static class ChunkMetaCooker
    {
        public static MetaBase Cook(GameObject chunkObject, string dir, ChunkCooker.ChunkImportInformation importInformation)
        {
            var chunkController = chunkObject.GetComponent<ChunkControllerBase>();
            Assert.IsNotNull(chunkController, "chunk must have a controller");

            var metaAsset = ScriptableObject.CreateInstance<MetaBase>();
            string assetPathAndName = dir + "/" + importInformation.ChunkName + ".m.asset";

            metaAsset.ChunkName = importInformation.ChunkName;
            metaAsset.ChunkClassName = importInformation.ChunkClass;
            metaAsset.ChunkConformation = chunkController.ConformationType;
            metaAsset.TopologyType = chunkController.TopologyType;
            metaAsset.Generation = importInformation.Generation;
            // todo: TagSet

            // AABBs
            metaAsset.AABBs = new List<Vector3>();
            if (chunkController is ChunkControllerDimensionsBased dimBaseController)
            {
                Assert.IsNotNull(dimBaseController.DimensionStack, "DimensionStack isn't attached");
                var aabbsAmount = dimBaseController.DimensionStack.GetItemsCount();
                for (int i = 0; i < aabbsAmount; ++i)
                {
                    dimBaseController.SetDimensionIndex(i);
                    metaAsset.AABBs.Add(dimBaseController.CalculateDimensionAABB().size);
                }
            }
            else if (chunkController is ChunkControllerCombinatorial)
                metaAsset.AABBs.Add(chunkController.CalculateDimensionAABB().size);
            
            AssetDatabase.CreateAsset(metaAsset, assetPathAndName);
            AssetDatabase.SaveAssets();
            return metaAsset;
        }
    }
}
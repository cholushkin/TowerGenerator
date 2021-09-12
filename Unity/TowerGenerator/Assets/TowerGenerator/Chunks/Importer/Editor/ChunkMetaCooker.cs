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
            metaAsset.ChunkControllerType = chunkController.ChunkControllerType;
            metaAsset.Generation = chunkController.Generation;
            metaAsset.TagSet = chunkController.ChunkTagSet;//new TagSet(chunkController.ChunkTagSet);
            metaAsset.AABB = chunkController.CalculateDimensionAABB().size;
            
            AssetDatabase.CreateAsset(metaAsset, assetPathAndName);
            AssetDatabase.SaveAssets();
            return metaAsset;
        }
    }
}
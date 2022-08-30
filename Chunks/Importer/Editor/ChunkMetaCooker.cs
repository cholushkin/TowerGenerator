using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;

namespace TowerGenerator.ChunkImporter
{
    public static class ChunkMetaCooker
    {
        public static MetaBase Cook(GameObject chunkObject, ChunkImportSource importSource, ChunkCooker.ChunkImportState importState)
        {
            Debug.Log($"Cooking meta for {chunkObject.name}");
            var chunkController = chunkObject.GetComponent<ChunkControllerBase>();
            Assert.IsNotNull(chunkController, "chunk must have a controller");

            var metaAsset = ScriptableObject.CreateInstance<MetaBase>();

            string assetPathAndName = importSource.MetasOutputPath + "/" + importState.ChunkName + ".cmeta.asset";
            importSource.ImportedMetas.Add(assetPathAndName);

            chunkController.Meta = metaAsset;
            metaAsset.ChunkName = importState.ChunkName;
            metaAsset.ChunkControllerType = importState.ChunkControllerType;
            metaAsset.Generation = importState.Generation;
            metaAsset.TagSet = importState.ChunkTagSet;
            metaAsset.ChunkMargin = 1f; // todo: FbxCommand ChunkMargin(0)
            metaAsset.AABB = chunkController.CalculateDimensionAABB().size;
            
            AssetDatabase.CreateAsset(metaAsset, assetPathAndName);
            AssetDatabase.SaveAssets();
            return metaAsset;
        }
    }
}
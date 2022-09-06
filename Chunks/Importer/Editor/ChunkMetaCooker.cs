using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;

namespace TowerGenerator.ChunkImporter
{
    public static class ChunkMetaCooker
    {
        public static MetaBase Cook(GameObject chunkObject, ChunkImportSource importSource, ChunkCooker.ChunkImportState importState)
        {
            var chunkController = chunkObject.GetComponent<ChunkControllerBase>();
            Assert.IsNotNull(chunkController, "chunk must have a controller");

            string assetPathAndName = importSource.MetasOutputPath + "/" + importState.ChunkName + ".cmeta.asset";
            var metaAsset = AssetDatabase.LoadAssetAtPath<MetaBase>(assetPathAndName); // Try to load existing asset first to keep references to the asset alive
            var isCreated = false;
            if (metaAsset == null)
            {
                metaAsset = ScriptableObject.CreateInstance<MetaBase>();
                isCreated = true;
            }

            chunkController.Meta = metaAsset;
            metaAsset.ChunkName = importState.ChunkName;
            metaAsset.ChunkControllerType = importState.ChunkControllerType;
            metaAsset.Generation = importState.Generation;
            metaAsset.TagSet = importState.ChunkTagSet;
            metaAsset.ChunkMargin = 1f; // todo: FbxCommand ChunkMargin(0)
            metaAsset.AABB = chunkController.CalculateDimensionAABB().size;
            metaAsset.ImportSource = importSource;

            if(isCreated)
                AssetDatabase.CreateAsset(metaAsset, assetPathAndName);
            AssetDatabase.SaveAssets();
            Debug.Log($"Import meta: {metaAsset}");
            return metaAsset;
        }
    }
}
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;

namespace TowerGenerator.ChunkImporter
{
    public static class ChunkMetaCooker
    {
        public static MetaBase Cook(GameObject chunkObject, TowerGeneratorSettings.Source source, ChunkCooker.ChunkImportInformation importInformation)
        {
            Debug.Log($"Cooking meta for {chunkObject.name}");
            var chunkController = chunkObject.GetComponent<ChunkControllerBase>();
            Assert.IsNotNull(chunkController, "chunk must have a controller");

            var metaAsset = ScriptableObject.CreateInstance<MetaBase>();
            var settings = ScriptableObjectUtility.GetInstanceOfSingletonScriptableObject<TowerGeneratorSettings>();
            Assert.IsNotNull(settings);

            string assetPathAndName = settings.MetasPath + "/" + importInformation.ChunkName + ".m.asset";

            chunkController.Meta = metaAsset;
            metaAsset.Source = source;
            metaAsset.ChunkName = importInformation.ChunkName;
            metaAsset.ChunkControllerType = importInformation.ChunkControllerType;
            metaAsset.Generation = importInformation.Generation;
            metaAsset.TagSet = importInformation.ChunkTagSet;
            metaAsset.ChunkMargin = 1f; // todo: FbxCommand ChunkMargin(0)
            metaAsset.AABB = chunkController.CalculateDimensionAABB().size;
            
            AssetDatabase.CreateAsset(metaAsset, assetPathAndName);
            AssetDatabase.SaveAssets();
            return metaAsset;
        }
    }
}
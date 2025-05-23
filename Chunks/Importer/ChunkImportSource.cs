using System;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;


namespace TowerGenerator
{
    [CreateAssetMenu(fileName = "ChunkImportSource",
        menuName = "GameLib/TowerGenerator/ChunkImportSource", order = 1)]
    public class ChunkImportSource : ScriptableObject
    {
        [Tooltip("List of paths this ChunkImportSource will work with. Example 'Assets\\Game\\FBX")]
        public string[] FbxInputPath;

        [Tooltip("Override for default ChunkCooker. If not specified the default one will be used.")]
        public string ChunkCooker;

        public bool EnableMetaGeneration; // Turns on/off meta generation for this source of import

        [Tooltip(
            "Variant chunk prefab should be used to assign user scripts. Original prefab is considered as visual representation of the chunk. Variant is considered as logical part. Also I found no way to keep attached scripts retained on every reexport of the chunk")]
        public bool GenerateVariant = true;

        public bool EnableImport = true; // Enable/disable importing for this import source
        public bool EnableCleanupFbxRoot = true; // Enable/disable fbx cleanup

        public bool AddColliders = true; // Enable/disable adding collider to each Renderer
        public bool CastShadows = true;
        public bool ApplyMaterials; // Apply default TowerGenerator material ColorScheme
        
        [Tooltip("Default material to apply to all meshes of the chunk. To override default material for specified mesh use FbxCommandMaterial")]
        public Material DefaultMaterial;

        [Tooltip("List of materials for 'Material' FBX-command")]
        public Material[] Materials; 

        public string MetasOutputPath =>
            string.IsNullOrEmpty(_metasOutputPath)
                ? ChunksOutputPath
                : _metasOutputPath; // All generated metas of this import source will be saved to this directory. If not specified then ChunksOutputPath will be used

        public string ChunksOutputPath =>
            string.IsNullOrEmpty(_chunksOutputPath)
                ? "Assets/"
                : _chunksOutputPath; // All imported chunks are going to be saved to this directory. If not specified "Assets" will be used
        
        public string ChunksVariantsOutputPath =>
            string.IsNullOrEmpty(_chunksVariantsOutputPath)
                ? ChunksOutputPath
                : _chunksVariantsOutputPath; // All imported chunks variants are going to be saved to this directory. If not specified ChunksOutputPath will be used

        public float Scale = 1f; // Additionally scales imported chunks by this value

        [SerializeField] private string _metasOutputPath;
        [SerializeField] private string _chunksOutputPath;
        [SerializeField] private string _chunksVariantsOutputPath;


        public bool IsMyAsset(string assetPath)
        {
            var normalizedAssetPath = Path.GetDirectoryName(assetPath)?.Replace("\\", "/") ?? string.Empty;
            return FbxInputPath.Select(inputPath => inputPath.Replace("\\", "/"))
                .Any(normalizedInputPath => normalizedAssetPath.StartsWith(normalizedInputPath));
        }
    }


    public static class ChunkImportSourceHelper
    {
        public static string GetPathInResources(string fullPath)
        {
            Assert.IsTrue(fullPath.Contains("Resources"));
            return fullPath.Substring(fullPath.IndexOf("Resources", StringComparison.Ordinal) + 10);
        }
    }
}
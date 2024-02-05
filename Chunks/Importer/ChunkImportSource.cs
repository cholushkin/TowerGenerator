using System;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;


namespace TowerGenerator
{
    [CreateAssetMenu(fileName = "ChunkImportSource",
        menuName = "ScriptableObjects/ChunkImportSource", order = 1)]

    public class ChunkImportSource : ScriptableObject
    {
        [Tooltip("List of paths this ChunkImportSource will work with. Example 'Assets\\Game\\FBX")]
        public string[] FbxInputPath;
        public bool EnableMetaGeneration; // Turns on/off meta generation for this source of import
        public bool IsPack = true; // TRUE - FBX contains multiple chunks. FALSE - one FBX contains one chunk inside
        public bool EnableImport = true; // Enable/disable importing for this import source
        public bool EnableCleanupFbxRoot = true; // Enable/disable fbx cleanup

        public bool AddColliders = true; // Enable/disable adding collider to each Renderer
        public bool ApplyMaterials; // Apply default TowerGenerator material ColorScheme

        public string MetasOutputPath => string.IsNullOrEmpty(_metasOutputPath) ? ChunksOutputPath : _metasOutputPath; // All generated metas of this import source will be saved to this directory. If not specified then ChunksOutputPath

        public string ChunksOutputPath => string.IsNullOrEmpty(_chunksOutputPath) ? "Assets/" : _chunksOutputPath; // All imported chunks are going to be saved to this directory. If not specified "Assets" will be used

        public float Scale = 1f; // Additionally scales imported chunks by this value

        [SerializeField]
        private string _metasOutputPath;
        [SerializeField]
        private string _chunksOutputPath;

        public bool IsMyAsset(string assetPath)
        {
            return FbxInputPath.Contains(Path.GetDirectoryName(assetPath));
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
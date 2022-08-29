using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;


namespace TowerGenerator
{
    // You can specify different import settings per Fbx or for group of Fbx with the same base name. 
    // ChunkImportSettings is also referenced as "chunk import source" in comments
    public class ChunkImportSettings
    {
        public bool EnableMetaGeneration; // Turns on/off meta generation for this source of import
        public bool IsPack = true; // TRUE - FBX contains multiple chunks. FALSE - one FBX contains one chunk inside
        public bool EnableImport = true; // Enable/disable importing for this import source
        public bool EnableCleanupFbxRoot = true; // Enable/disable fbx cleanup

        public bool AddColliders; // Enable/disable adding collider to each Renderer
        public bool ApplyMaterials; // Apply default TowerGenerator material ColorScheme

        public string MetasOutputPath; // All generated metas of this import source will be saved to this directory. If not specified then ChunksOutputPath
        public string ChunksOutputPath; // All imported chunks are going to be saved to this directory. If not specified "Assets" will be used

        public float Scale = 1f; // Additionally scales imported chunks by this value
    }


    [InitializeOnLoad]
    public static class ChunkImportSettingsManager
    {
        private static Dictionary<string, ChunkImportSettings> _registeredChunkImportSettings;

        // Register Import Settings for any fbx which name starts from nameStartFrom
        public static void RegisterChunkImportSettings(string nameStartFrom, ChunkImportSettings entry)
        {
            if (_registeredChunkImportSettings == null)
                _registeredChunkImportSettings = new Dictionary<string, ChunkImportSettings>();

            if (_registeredChunkImportSettings.ContainsKey(nameStartFrom) && _registeredChunkImportSettings[nameStartFrom] != null)
            {
                Debug.LogWarning($"Settings for {nameStartFrom} is already registered");
                return;
            }

            Debug.Log($"Registering ChunkImportSettings for '{nameStartFrom}'");
            _registeredChunkImportSettings.Add(nameStartFrom, entry);
        }

        public static ChunkImportSettings GetImportSettingsByPath(string assetPath)
        {
            var ext = Path.GetExtension(assetPath);
            if (ext != ".blend" && ext != ".fbx")
                return null;

            var fileNameNoExt = Path.GetFileNameWithoutExtension(assetPath);
            var fileNameNoIndexes = fileNameNoExt.Split('.')[0];

            return GetImportSettings(fileNameNoIndexes);
        }

        public static ChunkImportSettings GetImportSettings(string assetName) // without dir and extension
        {
            if (string.IsNullOrEmpty(assetName))
                return null;
            if (_registeredChunkImportSettings != null && _registeredChunkImportSettings.TryGetValue(assetName, out var settings))
                return settings;
            return null;
        }
    }
}
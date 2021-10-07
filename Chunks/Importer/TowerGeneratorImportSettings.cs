using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;


namespace TowerGenerator
{
    public class ChunkImportSettings
    {
        public bool EnableMetaGeneration;
        public bool EnableChunkGeneration = true; // enable/disable importing for this source
        public bool EnableCleanupFbx = true; // enable/disable fbx cleanup

        public bool AddColliders;
        public bool ApplyMaterials; // apply default TowerGenerator material ColorScheme

        public string MetasOutputPath; // generate metas to this directory
        public string ChunksOutputPath = "Assets/Libs/TowerGenerator/Prefabs/Chunks"; // generate chunks to this directory

        public float Scale = 1f;
    }


    [InitializeOnLoad]
    public static class ChunkImportSettingsManager
    {
        private static Dictionary<string, ChunkImportSettings> _registeredChunkImportSettings;

        public static void RegisterChunkImportSettings(string assetName, ChunkImportSettings entry)
        {
            if (_registeredChunkImportSettings == null)
                _registeredChunkImportSettings = new Dictionary<string, ChunkImportSettings>();

            if (_registeredChunkImportSettings.ContainsKey(assetName) && _registeredChunkImportSettings[assetName] != null)
            {
                Debug.LogWarning($"Settings for {assetName} is already registered");
                return;
            }

            Debug.Log($"Registering settings for {assetName}");
            _registeredChunkImportSettings.Add(assetName, entry);
        }

        public static ChunkImportSettings GetImportSettingsByPath(string assetPath)
        {
            var ext = Path.GetExtension(assetPath);
            if (ext != ".blend" && ext != ".fbx")
                return null;
            return GetImportSettings(Path.GetFileNameWithoutExtension(assetPath));
        }

        public static ChunkImportSettings GetImportSettings(string assetName) // without dir and extension
        {
            if (string.IsNullOrEmpty(assetName))
                return null;
            if (_registeredChunkImportSettings.TryGetValue(assetName, out var settings))
                return settings;
            return null;
        }
    }
}
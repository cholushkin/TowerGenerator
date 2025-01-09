#if UNITY_EDITOR

#define USE_ANTI_CRASH_HACK
using System.Collections;
using System.IO;
using System.Text.RegularExpressions;
using Unity.EditorCoroutines.Editor;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;


namespace TowerGenerator.ChunkImporter
{
    public class ChunkImporter : AssetPostprocessor
    {
        private static void OnPostprocessAllAssets(
            string[] importedAssets,
            string[] deletedAssets,
            string[] movedAssets,
            string[] movedFromAssetPaths)
        {
            // Process chunk importing
            foreach (string assetPath in importedAssets)
            {
                var source = ChunkImportSourceManager.GetChunkImportSource(assetPath);

                if (source == null)
                    continue;
                if (!source.EnableImport)
                    continue;

                if (source.IsPack)
                {
                    Debug.Log($"Importing content pack: '{assetPath}'");
                    var packName = Path.GetFileNameWithoutExtension(assetPath);
                    var assetObj = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
                    if (assetObj == null)
                    {
                        Debug.LogError($"Error: can't load asset at path {assetPath}");
                        continue;
                    }

                    // Extracting all chunks from the pack
                    ExtractChunks(assetObj, packName, source);
                }
                else
                {
                    Debug.Log($"Importing chunk: '{assetPath}'");
                    var chunkName = Path.GetFileNameWithoutExtension(assetPath);
                    var assetObj = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
                    if (assetObj == null)
                    {
                        Debug.LogError($"Error: can't load asset at path {assetPath}");
                        continue;
                    }

                    ExtractChunk(assetObj, chunkName, source);
                }
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            // process deleted chunk packs
            foreach (string deletedAssetPath in deletedAssets)
            {
                // todo:
            }

            AssetDatabase.Refresh();
        }


        private static void ExtractChunks(GameObject assetObject, string packName, ChunkImportSource importSource)
        {
            // process all chunks inside fbx
            foreach (Transform ent in assetObject.transform)
            {
                var fullEntName = $"{packName}.{CleanName(ent.gameObject.name)}";
                ExtractChunk(ent.gameObject, fullEntName, importSource);
            }
        }

        private static void DeleteChunk(string chunkName, ChunkImportSource importSource)
        {
            var fullPath = Path.Combine(importSource.ChunksOutputPath, chunkName + ".prefab");
            AssetDatabase.DeleteAsset(fullPath);
        }

        private static void DeleteMeta(string chunkName, ChunkImportSource importSource)
        {
            var fullPath = Path.Combine(importSource.ChunksOutputPath, chunkName + "cmeta..prefab");
            AssetDatabase.DeleteAsset(fullPath);
        }

        // write patterns, add design-time stuff, do hierarchy reorganizations
        private static void ExtractChunk(GameObject chunkSource, string chunkName, ChunkImportSource importSource)
        {
            EditorCoroutineUtility.StartCoroutineOwnerless(InstantiateAndConfigureChunk(chunkSource, chunkName,
                importSource));
        }

        private static IEnumerator InstantiateAndConfigureChunk(GameObject chunkSource,
            string chunkName, ChunkImportSource importSource)
        {
            // Instantiate output chunk
            var chunk = Object.Instantiate(chunkSource);
            yield return null;

            // Cook the chunk (execute FBX command, apply colliders, and materials)
            var importInformation = new ChunkImportState(chunkName, importSource);
            chunk.name = chunkName;

            var chunkCooker = ChunkCookerFactory.CreateChunkCooker(importSource.ChunkCooker);
            yield return EditorCoroutineUtility.StartCoroutineOwnerless(chunkCooker.Cook(importSource, chunk, importInformation));

            chunk.transform.localScale *= importSource.Scale; // Apply additional scale
            chunk.transform.position = Vector3.zero;

            // Generate meta if enabled
            if (importSource.EnableMetaGeneration)
                ChunkMetaCooker.Cook(chunk, importSource, importInformation);
            yield return null;

            // Save the chunk as a new Prefab (since you cannot modify the original Model Prefab directly)
            var fullPath = Path.Combine(importSource.ChunksOutputPath, chunkName + ".prefab");

#if USE_ANTI_CRASH_HACK
            HackAvoidEditorCrash(fullPath);
#endif

            // Save the modified chunk as a new prefab
            PrefabUtility.SaveAsPrefabAsset(chunk, fullPath);
            AssetDatabase.ImportAsset(fullPath);
            yield return null; // Ensure all processes are completed before destruction

            Debug.Log($"Import chunk: {importInformation}");

            // Save the variant prefab if it does not already exist
            if (importSource.GenerateVariant)
            {
                var variantName = chunkName + "Usr"; // meaning "user" prefab with user scripts and modifications 
                var variantFullPath = Path.Combine(importSource.ChunksVariantsOutputPath, variantName + ".prefab");

                // Check if the variant prefab already exists
                if (AssetDatabase.LoadAssetAtPath<GameObject>(variantFullPath) == null)
                {
                    // Load the saved base prefab
                    var basePrefab = AssetDatabase.LoadAssetAtPath<GameObject>(fullPath);

                    // Instantiate the base prefab
                    var chunkVariant = PrefabUtility.InstantiatePrefab(basePrefab) as GameObject;
                    chunkVariant.name = variantName;

                    // Save as a new variant prefab, connecting it to the base prefab
                    var chunkVariantPrefab = PrefabUtility.SaveAsPrefabAssetAndConnect(chunkVariant, variantFullPath,
                        InteractionMode.AutomatedAction);
                    yield return null; // Ensure everything is processed

                    Debug.Log($"Variant chunk saved: {variantName}");

                    // Clean up the instantiated chunk variant
                    Object.DestroyImmediate(chunkVariant);
                }
                else
                {
                    Debug.Log($"Variant prefab '{variantName}' already exists, skipping creation.");
                }
            }

            // Clean up the instantiated chunk
            Object.DestroyImmediate(chunk);
        }


        private static void HackAvoidEditorCrash(string fullPath)
        {
            // hack: on some chunks if there are some combination of attached components on existing prefab
            // (which we are going to override) editor will crash after calling SaveAsPrefabAsset.
            // 0x00007FF8D9559CF7 (Unity) FindSimilarComponent  <--- in this native method
            // 0x00007FF8D9565876 (Unity) MatchComponents
            // 0x00007FF8D95507F3 (Unity) BuildReplaceMapByGameObjectName
            // 0x00007FF8D9574535 (Unity) SetupFileIDsFromExistingPrefabAssetNameBased
            // 0x00007FF8D95726D5 (Unity) SavePrefab_Internal
            // 0x00007FF8D957177C (Unity) SaveAsPrefabAsset
            // 0x00007FF8D90E9838 (Unity) PrefabUtilityBindings::SaveAsPrefabAsset_Internal
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(fullPath);
            if (prefab == null)
                return;

            GameObject loadedPrefab = PrefabUtility.LoadPrefabContents(fullPath);
            if (loadedPrefab == null)
                return;

            foreach (Transform child in loadedPrefab.transform)
                Object.DestroyImmediate(child.gameObject);
            PrefabUtility.SaveAsPrefabAsset(loadedPrefab, fullPath);
        }

        // removes <> from name
        private static string CleanName(string entNameInFbx)
        {
            string RemoveBetween(string s, char begin, char end)
            {
                Regex regex = new Regex($"\\{begin}.*?\\{end}");
                return regex.Replace(s, string.Empty);
            }

            return RemoveBetween(entNameInFbx, '<', '>');
        }

        private static string GetRelativePath(string absPath)
        {
            var normalizedPath = Path.GetFullPath(absPath).Replace("\\", "/");
            if (normalizedPath.StartsWith(Application.dataPath))
                absPath = "Assets" + normalizedPath.Substring(Application.dataPath.Length);
            return absPath;
        }
    }
}
#endif
#define USE_ANTI_CRASH_HACK
using System.Collections;
using System.IO;
using System.Text.RegularExpressions;
using Cysharp.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;
using Object = UnityEngine.Object;


namespace TowerGenerator.ChunkImporter
{
    public class ChunkImporter : AssetPostprocessor
    {
        static UniTask _importQueue = UniTask.CompletedTask;

        private static void OnPostprocessAllAssets(
            string[] importedAssets,
            string[] deletedAssets,
            string[] movedAssets,
            string[] movedFromAssetPaths)
        {
            // Process chunk importing
            foreach (var assetPath in importedAssets)
            {
                var source = ChunkImportSourceManager.GetChunkImportSource(assetPath);
                if (source == null || !source.EnableImport) continue;

                var chunkName = Path.GetFileNameWithoutExtension(assetPath);
                var assetObj = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
                if (assetObj == null)
                {
                    Debug.LogError($"Can't load {assetPath}");
                    continue;
                }

                // If the chain is in a bad state, reset it.
                if (_importQueue.Status.IsCompleted() || _importQueue.Status.IsCanceled() || _importQueue.Status.IsFaulted())
                    _importQueue = UniTask.CompletedTask;
                
                _importQueue = _importQueue.ContinueWith(async () =>
                {
                    try
                    {
                        await InstantiateAndConfigureChunkAsync(assetObj, chunkName, source);
                    }
                    catch (System.Exception ex)
                    {
                        Debug.LogException(ex); // swallow so the chain doesn't fault
                    }
                });
            }

            // process deleted chunk packs
            foreach (string deletedAssetPath in deletedAssets)
            {
                // todo:
            }
            AssetDatabase.SaveAssets();
        }
        
        [InitializeOnLoadMethod]
        static void ResetQueueOnLoad()
        {
            _importQueue = UniTask.CompletedTask;
        }

        private static string GetDependencyHash(string assetPath)
        {
            return AssetDatabase.GetAssetDependencyHash(assetPath).ToString();
        }

        void OnPreprocessModel()
        {
            // todo: define in ImportSource

            ModelImporter modelImporter = (ModelImporter)assetImporter;

            var source = ChunkImportSourceManager.GetChunkImportSource(assetPath);

            if (source == null)
                return;
            if (!source.EnableImport)
                return;

            // Set the material creation mode
            modelImporter.materialImportMode = ModelImporterMaterialImportMode.None;
            modelImporter.importAnimation = false;
            modelImporter.animationType = ModelImporterAnimationType.None;
            modelImporter.importBlendShapes = false;
            modelImporter.importVisibility = false;
            modelImporter.importCameras = false;
            modelImporter.importLights = false;
            modelImporter.meshCompression = ModelImporterMeshCompression.Medium;
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

        private static async UniTask InstantiateAndConfigureChunkAsync(
            GameObject chunkSource, string chunkName, ChunkImportSource importSource)
        {
            await UniTask.SwitchToMainThread();

            var fullPath = Path.Combine(importSource.ChunksOutputPath, chunkName + ".prefab");
            var prevPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(fullPath);

            var importInformation = new ChunkImportState(chunkName, importSource);

            GameObject chunk = null;
            try
            {
                chunk = PrefabUtility.InstantiatePrefab(chunkSource) as GameObject;
                chunk.name = chunkName;

                await UniTask.Yield();

                var cooker = ChunkCookerFactory.CreateChunkCooker(importSource.ChunkCooker);
                await cooker.CookAsync(importSource, chunk, importInformation);

                chunk.transform.localScale *= importSource.Scale;
                chunk.transform.position = Vector3.zero;

                if (importSource.EnableMetaGeneration)
                    ChunkMetaCooker.Cook(chunk, importSource, importInformation);

                await UniTask.Yield();
                
#if USE_ANTI_CRASH_HACK
                HackAvoidEditorCrash(fullPath); // now leak-free
#endif

                PrefabUtility.SaveAsPrefabAsset(chunk, fullPath);
                // AssetDatabase.ImportAsset(fullPath); // usually unnecessary after SaveAsPrefabAsset
                await UniTask.Yield();

                Debug.Log($"Import chunk: {importInformation}");

                if (importSource.GenerateVariant)
                {
                    var variantName = chunkName + "Usr";
                    var variantFullPath = Path.Combine(importSource.ChunksVariantsOutputPath, variantName + ".prefab");

                    if (AssetDatabase.LoadAssetAtPath<GameObject>(variantFullPath) == null)
                    {
                        var basePrefab = AssetDatabase.LoadAssetAtPath<GameObject>(fullPath);
                        var chunkVariant = PrefabUtility.InstantiatePrefab(basePrefab) as GameObject;
                        chunkVariant.name = variantName;

                        try
                        {
                            PrefabUtility.SaveAsPrefabAssetAndConnect(
                                chunkVariant, variantFullPath, InteractionMode.AutomatedAction);
                        }
                        finally
                        {
                            Object.DestroyImmediate(chunkVariant);
                        }

                        await UniTask.Yield();
                        Debug.Log($"Variant chunk saved: {variantName}");
                    }
                }
            }
            finally
            {
                if (chunk != null) Object.DestroyImmediate(chunk);
            }
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
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(fullPath);
            if (prefab == null) return;

            GameObject loadedPrefab = PrefabUtility.LoadPrefabContents(fullPath);
            if (loadedPrefab == null) return;

            try
            {
                foreach (Transform child in loadedPrefab.transform)
                    Object.DestroyImmediate(child.gameObject);

                PrefabUtility.SaveAsPrefabAsset(loadedPrefab, fullPath);
            }
            finally
            {
                // This is the critical line you were missing.
                PrefabUtility.UnloadPrefabContents(loadedPrefab);
            }
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
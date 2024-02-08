using System;
using System.IO;
using System.Text.RegularExpressions;
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
                
                if(source == null)
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
            var chunk = Object.Instantiate(chunkSource);
            try
            {
                var importInformation = new ChunkCooker.ChunkImportState(chunkName, importSource);
                chunk.name = chunkName;
                chunk = ChunkCooker.Cook(importSource, chunk, importInformation);

                chunk.transform.localScale *= importSource.Scale;
                chunk.transform.position = Vector3.zero;

                var fullPath = Path.Combine(importSource.ChunksOutputPath, chunkName + ".prefab");
                
                if (importSource.EnableMetaGeneration)
                    ChunkMetaCooker.Cook(chunk, importSource, importInformation);
                
                PrefabUtility.SaveAsPrefabAsset(chunk, fullPath);
                AssetDatabase.ImportAsset(fullPath);
                Debug.Log($"Import chunk: {importInformation}");
            }
            catch (Exception e)
            {
                Debug.LogError($"Error during extracting chunk '{chunk}'");
                Debug.LogError($"Error details: '{e}'");
            }
            finally
            {
                Object.DestroyImmediate(chunk);
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
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
                var settings = ChunkImportSettingsManager.GetImportSettingsByPath(assetPath);
                
                if(settings == null)
                    continue;
                if (!settings.EnableImport) 
                    continue;

                if (settings.IsPack)
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
                    ExtractChunks(assetObj, packName, settings);
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
                    ExtractChunk(assetObj, chunkName, settings);
                }
            }
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();


            // process deleted chunk packs
            foreach (string deletedAssetPath in deletedAssets)
            {
                var settings = ChunkImportSettingsManager.GetImportSettingsByPath(deletedAssetPath);
                if (settings == null)
                    continue;

                var assetObjDelete = AssetDatabase.LoadAssetAtPath<GameObject>(deletedAssetPath);

                if (settings.IsPack)
                {
                    var packName = Path.GetFileNameWithoutExtension(deletedAssetPath);
                    DeleteChunks(settings, packName);
                    DeleteMetas(settings, packName);
                }
                else
                {
                    var chunkName = Path.GetFileNameWithoutExtension(deletedAssetPath);
                    DeleteChunk(chunkName, settings);
                    DeleteMeta(chunkName, settings);
                }
            }
            AssetDatabase.Refresh();
        }

        private static void DeleteMetas(ChunkImportSettings source, string packName) // not unity meta
        {
            DirectoryInfo dir = new DirectoryInfo(source.ChunksOutputPath);
            FileInfo[] info = dir.GetFiles(packName + ".*.cmeta.asset"); // delete all chunks with name starting with packName
            foreach (FileInfo f in info)
            {
                Debug.Log($"Deleting meta {f.Name} for Pack {packName}");
                f.Delete();
            }
        }

        private static void DeleteChunks(ChunkImportSettings source, string packName)
        {
            DirectoryInfo dir = new DirectoryInfo(source.ChunksOutputPath);
            FileInfo[] info = dir.GetFiles(packName + ".*.prefab"); // delete all chunks with name starting with packName
            foreach (FileInfo f in info)
            {
                Debug.Log($"Deleting chunk {f.Name} for Pack {packName}");
                f.Delete();
            }
        }

        private static void ExtractChunks(GameObject assetObject, string packName, ChunkImportSettings source)
        {
            // process all chunks inside fbx
            foreach (Transform ent in assetObject.transform)
            {
                var fullEntName = $"{packName}.{CleanName(ent.gameObject.name)}";
                ExtractChunk(ent.gameObject, fullEntName, source);
            }
        }

        private static void DeleteChunk(string chunkName, ChunkImportSettings settings)
        {
            var fullPath = Path.Combine(settings.ChunksOutputPath, chunkName + ".prefab");
            AssetDatabase.DeleteAsset(fullPath);
        }
        
        private static void DeleteMeta(string chunkName, ChunkImportSettings settings)
        {
            var fullPath = Path.Combine(settings.ChunksOutputPath, chunkName + "cmeta..prefab");
            AssetDatabase.DeleteAsset(fullPath);
        }

        // write patterns, add design-time stuff, do hierarchy reorganizations
        private static void ExtractChunk(GameObject chunkSource, string chunkName, ChunkImportSettings source)
        {
            var chunk = Object.Instantiate(chunkSource);

            try
            {
                var importInformation = new ChunkCooker.ChunkImportState(chunkName, source);
                chunk.name = chunkName;
                chunk = ChunkCooker.Cook(source, chunk, importInformation);

                chunk.transform.localScale *= source.Scale;
                chunk.transform.position = Vector3.zero;

                if (source.EnableMetaGeneration)
                    ChunkMetaCooker.Cook(chunk, source, importInformation);

                var fullPath = Path.Combine(source.ChunksOutputPath, chunkName + ".prefab");
                PrefabUtility.SaveAsPrefabAsset(chunk, fullPath);
                AssetDatabase.ImportAsset(fullPath);
                Debug.Log($"Chunk {chunkName}(guid:{AssetDatabase.AssetPathToGUID(Path.Combine(source.ChunksOutputPath, chunkName + ".prefab"))} path:{source.ChunksOutputPath}) imported successfully: {importInformation}");
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
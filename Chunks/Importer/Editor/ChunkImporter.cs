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
            // process imported chunk packs
            foreach (string assetPath in importedAssets)
            {
                var source = ChunkImporterHelper.GetSource(assetPath);
                if (source == null)
                    continue;

                if (source.EnableChunkGeneration)
                {
                    Debug.Log($"Importing content pack: '{assetPath}'");
                    
                    var packName = Path.GetFileNameWithoutExtension(assetPath);
                    var assetObj = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
                    if (assetObj == null)
                    {
                        Debug.LogErrorFormat("Error: can't load asset at path {0}", assetPath);
                        continue;
                    }

                    // extracting entities from the pack
                    ExtractChunks(assetObj, packName, source);
                }
            }

            // process deleted chunk packs
            foreach (string deletedAssetPath in deletedAssets)
            {
                var source = ChunkImporterHelper.GetSource(deletedAssetPath);
                if (source == null)
                    continue;

                var packName = Path.GetFileNameWithoutExtension(deletedAssetPath);
                DeleteChunks(source, packName);
                DeleteMetas(source, packName);
            }

            // todo: also check metas and chunks that don't belong to any of existing sources
        }

        private static void DeleteMetas(TowerGeneratorImportSource source, string packName)
        {
            DirectoryInfo dir = new DirectoryInfo(source.ChunksOutputPath);
            FileInfo[] info = dir.GetFiles(packName + ".meta.*"); // delete all chunks with name starting with packName
            foreach (FileInfo f in info)
            {
                Debug.Log($"deleting {f.Name}");
                f.Delete();
            }
            AssetDatabase.Refresh();
        }

        private static void DeleteChunks(TowerGeneratorImportSource source, string packName)
        {
            DirectoryInfo dir = new DirectoryInfo(source.ChunksOutputPath);
            FileInfo[] info = dir.GetFiles(packName + ".*"); // delete all chunks with name starting with packName
            foreach (FileInfo f in info)
            {
                Debug.Log($"deleting {f.Name}");
                f.Delete();
            }
            AssetDatabase.Refresh();
        }


        private static void ExtractChunks(GameObject assetObject, string packName, TowerGeneratorImportSource source)
        {
            // --- delete all previous chunks and metas of this content pack
            DeleteChunks(source, packName);
            DeleteMetas(source, packName);
            
            // process all ents inside fbx
            foreach (Transform ent in assetObject.transform)
            {
                var fullEntName = $"{packName}.{CleanName(ent.gameObject.name)}";
                ExtractChunk(ent, fullEntName, source);
            }
            AssetDatabase.Refresh();
        }

        // write patterns, add design-time stuff, do hierarchy reorganizations
        private static void ExtractChunk(Transform chunkTransform, string chunkName, TowerGeneratorImportSource source)
        {
            var chunk = Object.Instantiate(chunkTransform.gameObject);

            try
            {
                ChunkCooker.ChunkImportInformation importInformation = new ChunkCooker.ChunkImportInformation(chunkName);
                chunk.name = chunkName;
                chunk = ChunkCooker.Cook(source, chunk, importInformation);

                if(source.EnableMetaGeneration)
                    ChunkMetaCooker.Cook(chunk, source, importInformation);

                PrefabUtility.SaveAsPrefabAsset(chunk, Path.Combine(source.ChunksOutputPath, chunkName + ".prefab"));
                Debug.Log($"Chunk imported successfully: {importInformation}");
            }
            catch (Exception e)
            {
                Debug.LogError($"Error during extracting chunk {chunk} : {e}");
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
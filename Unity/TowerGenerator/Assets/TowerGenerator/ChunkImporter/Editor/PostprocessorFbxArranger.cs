using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;


namespace TowerGenerator.ChunkImporter
{
    public class PostprocessorFbxArranger : AssetPostprocessor
    {
        private static void OnPostprocessAllAssets(
            string[] importedAssets,
            string[] deletedAssets,
            string[] movedAssets,
            string[] movedFromAssetPaths)
        {
            foreach (string assetPath in importedAssets)
            {
                //var root = Path.GetPathRoot(assetPath);
                //var parentDir = Path.GetDirectoryName(assetPath).Split(Path.DirectorySeparatorChar).Last();

                if (ChunkImporterHelper.IsChunkPackFbx(assetPath))
                {
                    // ImportChunks
                    //  delete old chunks with such names
                    //  extract new
                    Debug.Log($"Importing content pack: '{assetPath}'");
                    var packName = Path.GetFileNameWithoutExtension(assetPath);

                    // load prefab as asset
                    var assetObj = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
                    if (assetObj == null)
                    {
                        Debug.LogErrorFormat("Error: can't load asset at path {0}", assetPath);
                        continue;
                    }

                    // extracting entities from the pack
                    ExtractChunks(assetObj, packName);
                }
            }
            //EditorUtility.SetDirty(_metas);
            // todo: deletedAssets
            // todo: delete all ents and metas of deleted content pack
            foreach (string assetPath in deletedAssets)
            {
                //var root = Path.GetPathRoot(assetPath);
                //var parentDir = Path.GetDirectoryName(assetPath).Split(Path.DirectorySeparatorChar).Last();
                //if (ContentPacksParentDir == parentDir)
                //{
                //    Debug.Log($"{assetPath} was deleted todo: delete ents from it");
                //}
            }
        }



        private static void ExtractChunks(GameObject assetObject, string packName)
        {
            // --- delete all (previous) chunks of this content pack and their metas
            DirectoryInfo dir = new DirectoryInfo(TowerGeneratorConstants.PathChunks);
            FileInfo[] info = dir.GetFiles(packName + ".*");
            foreach (FileInfo f in info)
            {
                Debug.Log($"deleting {f.Name}");
                f.Delete();
            }
            AssetDatabase.Refresh();

            // process all ents inside fbx
            foreach (Transform ent in assetObject.transform)
            {
                var fullEntName = $"{packName}.{CleanName(ent.gameObject.name)}";
                ExtractChunk(ent, TowerGeneratorConstants.PathChunks, fullEntName);
            }
            AssetDatabase.Refresh();
        }

        // write patterns, add design-time stuff, do hierarchy reorganizations
        private static void ExtractChunk(Transform chunkTransform, string dirToImort, string chunkName)
        {
            var chunk = Object.Instantiate(chunkTransform.gameObject);

            try
            {
                ChunkCooker.ChunkImportInformation importInformation = new ChunkCooker.ChunkImportInformation(chunkName);
                chunk.name = chunkName;
                chunk = ChunkCooker.Cook(chunk, importInformation);
                ChunkMetaCooker.Cook(chunk, dirToImort, importInformation);
                PrefabUtility.SaveAsPrefabAsset(chunk, Path.Combine(dirToImort, chunkName + ".prefab"));
                Debug.LogFormat($"Chunk imported successfully: {0}", importInformation);
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
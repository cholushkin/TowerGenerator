using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;


namespace TowerGenerator
{
    public class ContentPackImporter : AssetPostprocessor
    {
        public const string ContentPacksParentDir = "ContentPacks";

        //private static Metas _metas;

        private static void OnPostprocessAllAssets(
            string[] importedAssets,
            string[] deletedAssets,
            string[] movedAssets,
            string[] movedFromAssetPaths)
        {
            foreach (string assetPath in importedAssets)
            {
                var root = Path.GetPathRoot(assetPath);
                var parentDir = Path.GetDirectoryName(assetPath).Split(Path.DirectorySeparatorChar).Last();

                if (ContentPacksParentDir == parentDir)
                {
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
                    ExtractEntities(assetObj, packName);
                }
            }
            //EditorUtility.SetDirty(_metas);
            // todo: deletedAssets
            // todo: delete all ents and metas of deleted content pack
            foreach (string assetPath in deletedAssets)
            {
                var root = Path.GetPathRoot(assetPath);
                var parentDir = Path.GetDirectoryName(assetPath).Split(Path.DirectorySeparatorChar).Last();
                if (ContentPacksParentDir == parentDir)
                {
                    Debug.Log($"{assetPath} was deleted todo: delete ents from it");
                }
            }
        }



        private static void ExtractEntities(GameObject assetObject, string packName)
        {
            // --- delete all (previous) ents of this content pack and their metas
            DirectoryInfo dir = new DirectoryInfo(TowerGeneratorConstants.PathEnts);
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
                ExtractEnt(ent, TowerGeneratorConstants.PathEnts, fullEntName);
            }
            AssetDatabase.Refresh();
        }

        // write patterns, add design-time stuff, do hierarchy reorganizations
        private static void ExtractEnt(Transform ent, string dirToImort, string entName)
        {
            var entInst = Object.Instantiate(ent.gameObject);
            entInst.name = entName;

            entInst = EntCooker.Cook(entInst);
            EntCooker.CreateMeta(entInst, dirToImort, entName);

            PrefabUtility.SaveAsPrefabAsset(entInst, Path.Combine(dirToImort, entName + ".prefab"));
            Object.DestroyImmediate(entInst);
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
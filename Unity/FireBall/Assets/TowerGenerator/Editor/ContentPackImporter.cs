using System.IO;
using System.Linq;
using FireBall.Game;
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
            //_metas = GetGlobalMetas();
            //Assert.IsNotNull(_metas);
            foreach (string assetPath in importedAssets)
            {
                var root = Path.GetPathRoot(assetPath);
                var parentDir = Path.GetDirectoryName(assetPath).Split(Path.DirectorySeparatorChar).Last();

                if (ContentPacksParentDir == parentDir)
                {
                    Debug.Log($"Importing content pack: '{assetPath}'");
                    
                    // load prefab as asset
                    var assetObj = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
                    if (assetObj == null)
                    {
                        Debug.LogErrorFormat("Error: can't load asset at path {0}", assetPath);
                        continue;
                    }

                    // extracting entities from the pack
                    {
                        ExtractEntites(assetObj);
                    }
                }
            }
            //EditorUtility.SetDirty(_metas);
            // todo: deletedAssets
            // delete all patterns of deleted patternpack
        }



        private static void ExtractEntites(GameObject assetObject)
        {
            //// --- delete all (previous) patterns of this pack and their patMetas
            //DirectoryInfo dir = new DirectoryInfo(GameConstants.PathPatterns);
            //FileInfo[] info = dir.GetFiles("Pat." + packName + "*");
            //foreach (FileInfo f in info)
            //{
            //    if (f.Name.EndsWith(".meta")) // keep unity metas for unity
            //        continue;
            //    f.Delete();
            //    _metas.RemoveMeta(Path.GetFileNameWithoutExtension(f.Name));
            //}

            //// get all patterns to extrude in resources
            //{
            //    var patterns = assetObject.GetComponentsInChildren<Pattern>();

            //    // write patterns, add design-time stuff, do hierarchy reorganizations
            //    foreach (var pattern in patterns)
            //    {
            //        var fullPatternName = string.Format(PatternNameFormatString, packName, pattern.gameObject.name);
            //        ExtractPattern(pattern, GameConstants.PathPatterns, fullPatternName);
            //    }
            //}
            //AssetDatabase.Refresh();


        }

    




    }
}
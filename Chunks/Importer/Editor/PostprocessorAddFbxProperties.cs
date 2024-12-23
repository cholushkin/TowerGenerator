﻿#if UNITY_EDITOR
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;

namespace TowerGenerator.ChunkImporter
{
    public class PostprocessorAddFbxProperties : AssetPostprocessor
    {
        // Due to a bug in unity that doesn't allow attach more than one script to game object ("IDENTIFIER UNIQUENESS VIOLATION" WARNING POPS UP WHEN ADDING MULTIPLE MONOBEHAVIOUR TO MODEL PREFAB ROOT VIA ASSETPOSTPROCESSOR)
        // warning appears in the console - "Identifier uniqueness violation: 'xxx'. Scripted Importers do not guarantee that subsequent imports of this asset will properly re-link to these targets."
        public void OnPostprocessGameObjectWithUserProperties(GameObject gObj, string[] names, System.Object[] values)
        {
            var settings = ChunkImportSourceManager.GetChunkImportSource(assetImporter.assetPath);
            if(settings == null)
                return;

            if (!settings.EnableImport)
                return;

            ProcessAddingFbxProps(gObj, names, values);
        }

        private void ProcessAddingFbxProps(GameObject gObj, string[] names, object[] values)
        {
            var isIgnore = names.FirstOrDefault(x => x == "IgnoreImport") != null;
            if (isIgnore)
            {
                Object.DestroyImmediate(gObj);
                return;
            }

            Assert.IsTrue(names.Length == values.Length);
            Assert.IsTrue(names.Length > 0);
            Assert.IsNotNull(gObj);

            var fbxProps = gObj.AddComponent<FbxProps>();

            for (int i = 0; i < names.Length; i++)
            {
                fbxProps.AddProperty(names[i], values[i].ToString());
            }

            // if chunk (need to have ChunkController) has no Meta command add the default one
            if (fbxProps.Properties.FirstOrDefault(x => x.Name == "ChunkController") != null)
                if (fbxProps.Properties.FirstOrDefault(x => x.Name == "Meta") == null)
                    fbxProps.AddProperty("Meta", "MetaBase");

            Assert.IsTrue(fbxProps.Properties.Count > 0);
        }

        public override int GetPostprocessOrder()
        {
            return 0;
        }
    }
}
#endif
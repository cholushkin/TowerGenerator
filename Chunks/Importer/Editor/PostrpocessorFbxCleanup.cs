﻿using System.IO;
using System.Linq;
using Assets.Plugins.Alg;
using UnityEditor;
using UnityEngine;

namespace TowerGenerator.ChunkImporter
{
    // Cleans up on import the FBX removing all objects which:
    // * has no FbxProp script attached to the root 
    // * has IgnoreImport property attached
    public class PostrpocessorFbxCleanup : AssetPostprocessor
    {
        void OnPostprocessModel(GameObject fbxOrBlendObject)
        {
            var settings = ChunkImportSettingsManager.GetImportSettingsByPath(assetImporter.assetPath);
            if(settings == null)
                return;
            if (!settings.EnableCleanupFbx)
                return;
            Debug.Log($"Cleaning fbx {fbxOrBlendObject.transform.GetDebugName()}");
            var safeChildrenList = fbxOrBlendObject.transform.Children().ToList();

            foreach (Transform child in safeChildrenList)
            {
                if (ChunkImporterHelper.IsObjectIgnored(child.gameObject))
                {
                    var meshFilters = child.gameObject.GetComponentsInChildren<MeshFilter>();

                    foreach (var t in meshFilters)
                        if (t.sharedMesh != null)
                            Object.DestroyImmediate(t.sharedMesh);

                    Object.DestroyImmediate(child.gameObject);
                }
            }
        }

        public override int GetPostprocessOrder()
        {
            return 1;
        }
    }
}
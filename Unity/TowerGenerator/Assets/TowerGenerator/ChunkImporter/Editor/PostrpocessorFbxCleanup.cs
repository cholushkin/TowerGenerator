using System.Linq;
using UnityEditor;
using UnityEngine;

namespace TowerGenerator.ChunkImporter
{
    // Cleans up on import the FBX removing all objects which:
    // * has no FbxProp script attached to the root 
    // * has IgnoreImport property attached
    public class PostrpocessorFbxCleanup : AssetPostprocessor
    {
        // The ModelImporter calls this function for every root transform hierarchy in the source model file.
        public void OnPostprocessMeshHierarchy(GameObject gObj)
        {
            ModelImporter modelImporter = assetImporter as ModelImporter;
            Debug.Assert(modelImporter != null, nameof(modelImporter) + " != null");
            if (ChunkImporterHelper.IsNeededToImportChunkPackFbx(modelImporter.assetPath))
            {
                if(ChunkImporterHelper.IsObjectIgnored(gObj))
                    GameObject.DestroyImmediate(gObj);
            }
        }

    }
}
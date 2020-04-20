using UnityEditor;
using UnityEngine;

namespace TowerGenerator.ChunkImporter
{
    // Cleans up on import the FBX removing all objects that has no FbxProp script attached to the root 
    public class PostrpocessorFbxCleanup : AssetPostprocessor
    {
        // The ModelImporter calls this function for every root transform hierarchy in the source model file.
        public void OnPostprocessMeshHierarchy( GameObject gObj ) 
        {
	        ModelImporter modelImporter = assetImporter as ModelImporter;
	        Debug.Assert(modelImporter != null, nameof(modelImporter) + " != null");
	        if (ChunkImporterHelper.IsChunkPackFbx(modelImporter.assetPath))
	        {
		        RemoveObjectWithoutFbxProps(gObj);
				return;
	        }
        }

        private void RemoveObjectWithoutFbxProps(GameObject gObj)
        {
			if (gObj.GetComponent<FbxProps>() == null)
				GameObject.DestroyImmediate(gObj);
		}
    }
}
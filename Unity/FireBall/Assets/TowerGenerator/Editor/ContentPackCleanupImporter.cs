using UnityEditor;
using UnityEngine;

namespace TowerGenerator
{
    public class ContentPackCleanupImporter : AssetPostprocessor
    {
        // The ModelImporter calls this function for every root transform hierarchy in the source model file.
        public void OnPostprocessMeshHierarchy( GameObject gObj ) 
        {
            if(gObj.GetComponent<FbxProps>() == null)
                GameObject.DestroyImmediate(gObj);
        }

        public override int GetPostprocessOrder()
        {
            return 1;
        }
    }
}
using UnityEngine;


namespace TowerGenerator
{
    [CreateAssetMenu(fileName = "TowerGeneratorImportSettings",
        menuName = "ScriptableObjects/TowerGeneratorSettings", order = 1)]
                                          
    public class TowerGeneratorImportSource : ScriptableObject
    {
        public bool EnableMetaGeneration;
        public bool EnableChunkGeneration; // enable/disable importing for this source
        public bool EnableCleanupFbx; // enable/disable fbx cleanup

        public bool AddColliders;
        public bool ApplyMaterials;

        public string ImportPath; // chunk pack folder with fbx or blend files
        public string MetasOutputPath; // generate metas to this directory
        public string ChunksOutputPath; // generate chunks to this directory
    }
}
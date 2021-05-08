using UnityEditor;
using UnityEngine;


namespace TowerGenerator.ChunkImporter
{
    public class PostprocessorFbxImportMode : AssetPostprocessor
    {
        void OnPreprocessModel()
        {
            ModelImporter modelImporter = assetImporter as ModelImporter;
            Debug.Assert(modelImporter != null, nameof(modelImporter) + " != null");
            if (ChunkImporterHelper.IsChunkPackFbx(modelImporter.assetPath))
            {
                SetImportModeForChunksFbx(modelImporter);
                return;
            }
        }

        private void SetImportModeForChunksFbx(ModelImporter modelImporter)
        {
            modelImporter.importAnimation = false;
            modelImporter.materialImportMode = ModelImporterMaterialImportMode.None;
        }
    }
}
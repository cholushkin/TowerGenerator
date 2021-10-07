using System.Linq;
using UnityEngine;

namespace TowerGenerator.ChunkImporter
{
    public static class ChunkImporterHelper
    {
        public static bool IsObjectIgnored(GameObject obj)
        {
            var fbxProbs = obj.GetComponent<FbxProps>();
            return (fbxProbs == null || (fbxProbs.Properties.FirstOrDefault(x => x.Name == "IgnoreImport") != null));
        }
    }
}


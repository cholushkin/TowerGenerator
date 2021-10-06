using System.IO;
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

        public static TowerGeneratorImportSource GetSource(string assetFullPath)
        {
            var extension = Path.GetExtension(assetFullPath);
            if (!(extension.Equals(".blend") || extension.Equals(".fbx")))
                return null;


            var sources = ScriptableObjectUtility.GetInstancesOfScriptableObject<TowerGeneratorImportSource>();
            if (sources == null)
                return null;
            foreach (var src in sources)
            {
                if (assetFullPath.StartsWith(src.ImportPath))
                    return src;
            }
            return null;
        }
    }
}

